using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Configuration;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.SemanticFunctions;
using System.Reflection;
using System.Security.Cryptography;

namespace AleF.SemanticBots.Bots;

/// <summary>
/// This class implements the base functionality for a chat bot. 
/// It implements the interface <see cref="ISemanticBot"/>.
/// </summary>
public abstract class SemanticBot : ISemanticBot
{
    public static class Params
    {
        public const string History = "history";
        public const string HumanInput = "human_input"; 
    }

    public static class Tags
    {
        public const string Human = "human";
    }

    readonly protected IKernel Kernel;
    readonly protected SemanticBotOptions BotOptions;
    readonly protected NLPServiceOptions NLPOptions;
    readonly protected ILogger Logger;
    readonly protected ISKFunction[]? Functions;
    readonly protected ContextVariables Variables;

    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticBot"/> class.
    /// </summary>
    public SemanticBot(
        ILogger logger,
        IOptions<NLPServiceOptions> nlpOptions, 
        IOptions<SemanticBotOptions> botOptions)
    {
        BotOptions = botOptions.Value; // TODO: validate options
        NLPOptions = nlpOptions.Value; // TODO: validate options
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        Kernel = CreateKernel() ?? throw new ArgumentNullException(nameof(Kernel));
        Functions = CreateFunctions() ?? throw new ArgumentNullException(nameof(Functions)); // TODO: ArgumentNullException?

        Variables = new ();
        _history = string.Empty;        
    }

    string _history;
    /// <inheritdoc/>
    public async Task<string> Send(string prompt, CancellationToken cancellationToken)
    {
        prompt = prompt?.Trim() ?? throw new ArgumentNullException(nameof(prompt));
        var funcs = Functions?.ToArray() ?? throw new ArgumentNullException(nameof(Functions));

        // Ask the question        
        Variables.Set(Params.HumanInput, prompt);
        Variables.Set(Params.History, _history);

        SKContext answer = await Kernel.RunAsync(Variables, cancellationToken, funcs);
        var cleanAnswer = answer.ToString().Trim();

        _history += $"\n{Tags.Human}: {prompt}\n{GetType().Name}: {answer.ToString()}\n";

        // TODO: find out how to get each individual token back before the full answer is completed.
        // TODO: raise an OnTokenReceived event or something.

        return cleanAnswer;
    }    

    protected virtual void ConfigureKernel(KernelConfig config)
    {
        config.AddOpenAITextCompletion("davinci", NLPOptions.Model, NLPOptions.ApiKey, NLPOptions.Organization);
    }
    protected IKernel CreateKernel() 
    {
        IKernel kernel = Microsoft.SemanticKernel.Kernel.Builder
            .WithLogger(Logger)
            .Configure(ConfigureKernel)
            // TODO: several other WithXXX to look into... Maybe too restrictive?
            .Build();

        return kernel;
    }

    protected virtual string GetPrompt() => @$"
{GetType().Name} can have a conversation with you about any topic.
It can give explicit instructions or say 'I don't know' if it does not have an answer.

{{${Params.History}}}
{{{Tags.Human}}}: {{${Params.HumanInput}}}
{GetType().Name}:";

    protected virtual ISKFunction[] CreateFunctions()
    {
        var promptConfig = new PromptTemplateConfig
        {
            Completion =
            {
                MaxTokens = BotOptions.MaxTokens,
                Temperature = BotOptions.Temperature,
                TopP = BotOptions.TopP,
            }
        }; 

        var promptTemplate = new PromptTemplate(GetPrompt(), promptConfig, Kernel);
        var functionConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);
        var chatFunction = Kernel.RegisterSemanticFunction(GetType().Name, "Chat", functionConfig);

        var funcs = new ISKFunction[]
        {
            chatFunction
        };

        return funcs;
    }   
}
