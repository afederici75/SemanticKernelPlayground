using System.Data.SqlTypes;
using System.Diagnostics;
using static Microsoft.SemanticKernel.AI.ChatCompletion.ChatHistory;

namespace AleF.SemanticBots.Bots;

public class SemanticBot : ISemanticBot
{
    readonly IKernel _kernel;
    readonly ILogger _logger;
    readonly IChatCompletion _chatCompletion;
    readonly OpenAIChatHistory _chatHistory;
    readonly List<ChatInteraction> _interactions;

    public SemanticBot(
        ILogger<SemanticBot> logger,
        IOptions<NLPServiceOptions> options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _kernel = BuildKernel(options.Value);

        _chatCompletion = _kernel.GetService<IChatCompletion>();
        _chatHistory = (OpenAIChatHistory)_chatCompletion.CreateNewChat(GetPrompt());

        _interactions = new List<ChatInteraction>();
    }

    protected virtual string GetPrompt()
    {
        return "You are a friendly, intelligent, and curious assistant who is good at conversation.Your name is Orko.";
    }

    protected virtual IKernel BuildKernel(NLPServiceOptions options)
    {
        var kernel = Kernel.Builder
            .WithLogger(_logger) // TODO: maybe I should have used a LoggerFactory?
            .Configure(cfg =>
            {
                // IMPORTANT: if the model is not "gpt-3.5-turbo" we get errors in the HTTP calls later.
                cfg.AddOpenAIChatCompletionService("chat", options.ChatModel, options.ApiKey);                
            })
            // TODO: several other WithXXX to look into...
            //.WithMemoryStorage(new VolatileMemoryStore())
            .Build();

        AddSkills(kernel);

        return kernel;
    }

    protected virtual void AddSkills(IKernel kernel)
    {
        // No need for any at this moment
    }

    protected virtual ChatRequestSettings GetRequestSettings()
    {
        var result = new ChatRequestSettings()
        {
            MaxTokens = 1500,
            Temperature = 0.7, // 0.0 - 1.0
            FrequencyPenalty = 0, // 0.0 - 2.0
            PresencePenalty = 0, // 0.0 - 2.0                                 
        };

        return result;
    }

    public async Task<string> Send(string prompt, ChatRequestSettings? settings = default, CancellationToken cancellationToken = default)
    {
        var answer = string.Empty;
        settings = settings ?? GetRequestSettings();

        try
        {
            // The chat history is used as context for the prompt.
            _chatHistory.AddUserMessage(prompt);
            
            // Calls OpenAI
            var sw = Stopwatch.StartNew();
            answer = await _chatCompletion.GenerateMessageAsync(_chatHistory, settings);
            sw.Stop();

            // Updates the context with the response.
            _chatHistory.AddAssistantMessage(answer);

            // Add a new ChatInteraction
            _interactions.Add(new ChatInteraction(prompt, answer, sw.Elapsed));
        }
        catch (AIException aiex)
        {
            // Reply with the error message if there is one
            answer = $"Semantic Kernel returned an error ({aiex.Message}). Please try again.";
        }

        return answer;
    }

    public IEnumerable<ChatInteraction> GetHistory() => _interactions;
}
