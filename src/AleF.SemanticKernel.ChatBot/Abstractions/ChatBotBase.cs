using System.Diagnostics;
using System.Text;

namespace AleF.SemanticKernel.ChatBot.Abstractions;

/// <summary>
/// This class implements the base functionality for a chat bot. 
/// It implements the interface <see cref="IChatBot"/>.
/// </summary>
public abstract class ChatBotBase : IChatBot
{
    public static class Params
    {
        public const string FactsParam = "facts";
        public const string HistoryParam = "history";
        public const string UserInputParam = "userInput";
    }

    readonly IKernel Kernel;
    readonly ChatBotOptions ChatOptions;
    readonly ISKFunction ChatFunction;
    SKContext? Context;
    readonly List<ChatInteraction> _iteractions = new List<ChatInteraction>();

    /// <inheritdoc/>
    public ChatBotBase(IKernel kernel, IOptions<ChatBotOptions> chatOptions)
    {
        ChatOptions = chatOptions.Value; // TODO: validate options        

        // Configure kernel
        Kernel = ConfigureKernel(kernel ?? throw new ArgumentNullException(nameof(kernel)));

        // Configure function
        ChatFunction = kernel.CreateSemanticFunction(
            promptTemplate: GetPrompt(),
            maxTokens: ChatOptions.MaxTokens,
            temperature: ChatOptions.Temperature);
    }

    /// <inheritdoc/>
    public event AnswerReceivedEventHandler AnswerReceived;

    /// <inheritdoc/>
    public IEnumerable<ChatInteraction> GetHistory(int? max = null)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task Send(string prompt)
    {
        prompt = prompt?.Trim() ?? throw new ArgumentNullException(nameof(prompt));
        var timer = Stopwatch.StartNew();

        if (Context is null)
            Context = await CreateContext(Kernel, ChatOptions);

        // Ask the question
        Context[Params.UserInputParam] = prompt;
        var answer = await ChatFunction.InvokeAsync(Context);
        var answerText = answer.ToString().Trim();

        // Append the new interaction to the chat history
        Context[Params.HistoryParam] += $"\nUser: '{prompt}', ChatBot: '{answerText}'";

        // Stores the local history
        var newInteraction = new ChatInteraction(Ask: prompt, Answer: answerText, Duration: timer.Elapsed);
        _iteractions.Add(newInteraction);

        // Invokes the callback
        if (AnswerReceived is not null)
            await AnswerReceived.Invoke(this, newInteraction);
    }

    /// <summary>
    /// Allows to configure the kernel before using it. Descendants can override this method 
    /// to add custom skills.
    /// </summary>
    /// <param name="kernel">The kernel to configure.</param>
    /// <returns>A configured kernel.</returns>
    protected virtual IKernel ConfigureKernel(IKernel kernel) => kernel;

    protected abstract string GetPrompt();

    protected virtual async Task<SKContext> CreateContext(IKernel kernel, ChatBotOptions chatOptions)
    {
        const string MemoryCollectionName = "FactsCollection";

        var context = kernel.CreateNewContext();
        context[Params.HistoryParam] = string.Empty;

        if (chatOptions.Facts is not null)
        {
            var idx = 0;
            var sb = new StringBuilder();
            foreach (var fact in chatOptions.Facts)
            {
                // Adds to the memory but it does not seem to affect the response
                // Does this work only with Search? What is the purpose of this?
                await kernel.Memory.SaveInformationAsync(
                        collection: MemoryCollectionName,
                        id: $"info{idx++}",
                        text: fact.Question);

                // Adds a fact to the facts
                sb.AppendLine(fact.Question + ' ' + fact.Answer);
            }
            context[Params.FactsParam] = sb.ToString();
        }

        return context;
    }
}
