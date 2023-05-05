using AleF.SemanticBots.Bots;
using Microsoft.SemanticKernel.Connectors.OpenAI.ChatCompletion;
using Microsoft.SemanticKernel.SkillDefinition;
using System.Collections.Specialized;
using System.Diagnostics;

namespace AleF.SemanticBots.Skills;

public class ChatSkill
{
    readonly IKernel _kernel;
    readonly ILogger _logger;
    readonly IChatCompletion _chatCompletion;
    readonly ChatHistory _chatHistory;
    readonly List<ChatInteraction> _interactions;

    public ChatSkill(
        ILogger<ChatSkill> logger,
        IKernel kernel,
        IOptions<NLPServiceOptions> options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));        
        _kernel.Config.AddOpenAIChatCompletionService(
            "chat", options.Value.ChatModel, options.Value.ApiKey);

        _chatCompletion = _kernel.GetService<IChatCompletion>();
        _chatHistory = _chatCompletion.CreateNewChat(
            // TODO: make this an option
            "You are a friendly, intelligent, and curious assistant who is good at conversation.Your name is Orko."
            );

        _interactions = new List<ChatInteraction>();

        AddSkills(kernel);
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

    const string SystemRole = "system";
    const string AssistantRole = "assistant";
    const string UserRole = "user";

    [SKFunction("Raises an event in MediatR.")]
    [SKFunctionName(nameof(SendUserMessage))]
    public async Task<string> PromptProcessed(string prompt)
    {
        return "";
    }

    [SKFunction("Raises an event in MediatR.")]
    [SKFunctionName(nameof(SendUserMessage))]
    public async Task<string> PromptFailed(string prompt, Exception exception)
    {
        return "";
    }

    [SKFunction("Send a user message (e.g. a prompt) to the LLM.")]
    [SKFunctionName(nameof(SendUserMessage))]
    public async Task<string> SendUserMessage(string prompt, ChatRequestSettings? settings = default, CancellationToken cancellationToken = default)
    {
        settings = settings ?? GetRequestSettings();
        
        string answer;
        try
        {
            // The chat history is used as context for the prompt.
            _chatHistory.AddMessage(UserRole, prompt);

            // Calls OpenAI
            var sw = Stopwatch.StartNew();
            answer = await _chatCompletion.GenerateMessageAsync(_chatHistory, settings, cancellationToken);
            sw.Stop();
            
            // Updates the context with the response.
            _chatHistory.AddMessage(AssistantRole, answer);

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