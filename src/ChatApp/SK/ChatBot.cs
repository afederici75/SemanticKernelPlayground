using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.CoreSkills;
using Microsoft.SemanticKernel.KernelExtensions;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SemanticFunctions;

namespace ChatApp.ChatBox;

// https://github.com/microsoft/semantic-kernel/blob/main/samples/notebooks/dotnet/6-memory-and-embeddings.ipynb

public class ChatBot : IChatBot
{
    const string HistoryParam = "history";
    const string UserInputParam = "userInput";

    const string Prompt = @"
ChatBot can have a conversation with you about any topic.
It can give explicit instructions or say 'I don't know' if it does not have an answer.

Information about me, from previous conversations:

- I am 47 years old.
- My Name is Alessandro

Chat:
{{$" + HistoryParam + @"}}
User: {{$" + UserInputParam + @"}}
ChatBot: ";

// TODO (bug): If I put this in the prompt the chatbot will 'freeze' when executing.
//- {{$fact1}} {{recall $fact1}}
//- {{$fact2}} {{recall $fact2}}
//- {{$fact3}} {{recall $fact3}}
//- {{$fact4}} {{recall $fact4}}
//- {{$fact5}} {{recall $fact5}}


    readonly IKernel Kernel;
    readonly ChatBotOptions ChatOptions;
    readonly ISKFunction ChatFunction;
    SKContext? Context;
        
    public ChatBot(IKernel kernel, IOptions<ChatBotOptions> chatOptions)
    {
        ChatOptions = chatOptions.Value;

        // Configure kernel
        Kernel = ConfigureKernel(kernel ?? throw new ArgumentNullException(nameof(kernel)));

        // Configure function
        ChatFunction = kernel.CreateSemanticFunction(
            promptTemplate: Prompt, 
            maxTokens: ChatOptions.MaxTokens, 
            temperature: ChatOptions.Temperature);        
    }

    public string GetHistory() => (Context is null) ? string.Empty : Context[HistoryParam];

    public async Task<string> Chat(string userInput)
    {
        if (string.IsNullOrEmpty(userInput))
            throw new ArgumentException($"'{nameof(userInput)}' cannot be null or empty.", nameof(userInput));        

        if (Context is null)
            Context = await CreateContext(Kernel, ChatOptions);        
        
        Context["userInput"] = userInput;
                
        // Ask the question
        var answer = await ChatFunction.InvokeAsync(Context);

        // Append the new interaction to the chat history
        Context["history"] += $"\nUser: '{userInput.Trim()}', ChatBot: '{answer.ToString().Trim()}'"; ;

        return Context.ToString();        
    }

    #region Private ---------------------------------------
    IKernel ConfigureKernel(IKernel kernel)
    {
        kernel.ImportSkill(new TextMemorySkill());

        return kernel;
    }    

    async Task<SKContext> CreateContext(IKernel kernel, ChatBotOptions chatOptions)
    {
        const string MemoryCollectionName = "aboutMe";

        var context = kernel.CreateNewContext();
        context[HistoryParam] = string.Empty;

        kernel.ImportSkill(new TextMemorySkill());
        context[TextMemorySkill.RelevanceParam] = chatOptions.Relevance.ToString(); ;
        context[TextMemorySkill.CollectionParam] = MemoryCollectionName;

        if (chatOptions.Facts is not null)
        {
            var idx = 0;
            foreach (var fact in chatOptions.Facts)
            {
                await kernel.Memory.SaveInformationAsync(
                        collection: MemoryCollectionName,
                        id: $"info{idx++}",
                        text: fact.Question);

                context[$"fact{idx}"] = fact.Answer;
            }
        }
        
        return context;
    }
    #endregion
}