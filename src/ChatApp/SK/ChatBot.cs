using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.CoreSkills;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SemanticFunctions;

namespace ChatApp.ChatBox;

// https://github.com/microsoft/semantic-kernel/blob/main/samples/notebooks/dotnet/6-memory-and-embeddings.ipynb

public class ChatBot : IChatBot
{
    readonly IKernel Kernel;
    readonly ChatBotOptions ChatOptions;
    readonly ISKFunction ChatFunction;
    readonly SKContext Context;
        
    public ChatBot(IKernel kernel, IOptions<ChatBotOptions> chatOptions)
    {
        ChatOptions = chatOptions.Value;

        // Configure kernel
        Kernel = ConfigureKernel(kernel ?? throw new ArgumentNullException(nameof(kernel)));

        // Configure context
        Context = CreateContext(Kernel, ChatOptions).Result;
        
        // Configure function
        ChatFunction = Kernel.RegisterSemanticFunction("ChatBot", "Chat", 
            ConfigureChatFunction(ChatOptions));        
    }

    public async Task<string> Chat(string userInput)
    {
        Context["userInput"] = userInput;
        
        // Ask the question
        var answer = await ChatFunction.InvokeAsync(Context);

        // Append the new interaction to the chat history
        Context["history"] += $"\nUser: {userInput}\nChatBot: {answer}\n"; ;

        return Context.ToString();        
    }

    static IKernel ConfigureKernel(IKernel kernel)
    {
        kernel.ImportSkill(new TextMemorySkill());

        return kernel;
    }

    const string MemoryCollectionName = "aboutMe";

    async Task<SKContext> CreateContext(IKernel kernel, ChatBotOptions chatOptions)
    {
        var context = kernel.CreateNewContext();
        context["history"] = string.Empty;

        context[TextMemorySkill.RelevanceParam] = "0.8";
        context[TextMemorySkill.CollectionParam] = MemoryCollectionName;

        if (chatOptions.Facts is null)
            return context;

        var idx = 0;
        var tasks = chatOptions.Facts.Select(async h =>
            await kernel.Memory.SaveInformationAsync(
                collection: MemoryCollectionName,
                id: $"info{idx++}",
                text: h.Value));

        await Task.WhenAll(tasks);

        for (idx = 0; idx < chatOptions.Facts.Length; idx++)
        {
            context[$"fact{idx}"] = chatOptions.Facts[idx].Key;
        }
        
        return context;
    }

    SemanticFunctionConfig ConfigureChatFunction(ChatBotOptions chatOptions)
    {
        const string skPrompt = @"
ChatBot can have a conversation with you about any topic.
It can give explicit instructions or say 'I don't know' if it does not have an answer.

Information about me, from previous conversations:
- {{$fact1}} {{recall $fact1}}
- {{$fact2}} {{recall $fact2}}
- {{$fact3}} {{recall $fact3}}
- {{$fact4}} {{recall $fact4}}
- {{$fact5}} {{recall $fact5}}

Chat:
{{$history}}
User: {{$userInput}}
ChatBot: ";

        var promptTmplConfiguration = new PromptTemplateConfig
        {
            Completion =
            {
                MaxTokens = chatOptions.MaxTokens,
                Temperature = chatOptions.Temperature,
                TopP = chatOptions.TopP,
            }
        };
        var promptTemplate = new PromptTemplate(skPrompt, promptTmplConfiguration, Kernel);
        var functionConfig = new SemanticFunctionConfig(promptTmplConfiguration, promptTemplate);
        return functionConfig;
    }      
}