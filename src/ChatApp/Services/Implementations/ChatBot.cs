using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.CoreSkills;
using Microsoft.SemanticKernel.KernelExtensions;
using Microsoft.SemanticKernel.Orchestration;
using System.Text;

namespace ChatApp.Services.Implementations;

// https://github.com/microsoft/semantic-kernel/blob/main/samples/notebooks/dotnet/6-memory-and-embeddings.ipynb

public class ChatBot : IChatBot
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

    public ChatBot(IKernel kernel, IOptions<ChatBotOptions> chatOptions)
    {
        ChatOptions = chatOptions.Value; // TODO: validate options        

        // Configure kernel
        Kernel = ConfigureKernel(kernel ?? throw new ArgumentNullException(nameof(kernel)));

        // Configure function
        ChatFunction = kernel.CreateSemanticFunction(
            promptTemplate: BuildPrompt(),
            maxTokens: ChatOptions.MaxTokens,
            temperature: ChatOptions.Temperature);
    }

    public event AnswerReceivedEventHandler? AnswerReceived;

    public string GetHistory() => Context is null ? string.Empty : Context[Params.HistoryParam];

    public async Task Send(string userInput)
    {
        userInput = userInput?.Trim() ?? throw new ArgumentNullException(nameof(userInput));

        if (Context is null)
            Context = await CreateContext(Kernel, ChatOptions);

        // Ask the question
        Context[Params.UserInputParam] = userInput;
        var answer = await ChatFunction.InvokeAsync(Context);
        var answerText = answer.ToString().Trim();

        // Append the new interaction to the chat history
        Context[Params.HistoryParam] += $"\nUser: '{userInput}', ChatBot: '{answerText}'"; ;

        // Invokes the callback
        if (AnswerReceived is not null)
            await AnswerReceived.Invoke(userInput, answerText);
    }


    #region Private ---------------------------------------
    
    string BuildPrompt()
    {
        const string Prompt = @"
ChatBot can have a conversation with you about any topic.
It says 'I don't know' if it does not have an answer.

Information about me, from previous conversations:
{{$" + Params.FactsParam + @"}}

Chat:
{{$" + Params.HistoryParam + @"}}

User: {{$" + Params.UserInputParam + @"}}

ChatBot: ";

        #region Issues with TextMemorySkill
        // TODO (bug): If I put this in the prompt the chatbot will 'freeze' when executing. Maybe buggy TextMemorySkill?
        //- {{$fact1}} {{recall $fact1}}
        //- {{$fact2}} {{recall $fact2}}
        //- {{$fact3}} {{recall $fact3}}
        //- {{$fact4}} {{recall $fact4}}
        //- {{$fact5}} {{recall $fact5}}
        #endregion

        return Prompt;
    }



    IKernel ConfigureKernel(IKernel kernel)
    {
        //kernel.ImportSkill(new TextMemorySkill());

        return kernel;
    }

    async Task<SKContext> CreateContext(IKernel kernel, ChatBotOptions chatOptions)
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

    #endregion
}