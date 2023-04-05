using Microsoft.SemanticKernel.CoreSkills;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Configuration;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SemanticFunctions;
using System.Reflection.Metadata;

namespace ChatApp.ChatBox;

// https://github.com/microsoft/semantic-kernel/blob/main/samples/notebooks/dotnet/6-memory-and-embeddings.ipynb

public class ChatBot : IChatBot
{
    readonly IKernel Kernel;
    readonly ISKFunction ChatFunction;
    readonly SKContext Context;    

    public ChatBot(IKernel kernel, IOptions<ChatBotOptions> chatOptions)
    {        
        Kernel = kernel ?? throw new ArgumentNullException(nameof(kernel));
        Kernel.ImportSkill(new TextMemorySkill());
        
        Context = Kernel.CreateNewContext();

        ChatFunction = Kernel.RegisterSemanticFunction("ChatBot", "Chat", 
            ConfigureChatFunction(chatOptions.Value));        
    }

    SemanticFunctionConfig ConfigureChatFunction(ChatBotOptions botOptions)
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
                MaxTokens = botOptions.MaxTokens,
                Temperature = botOptions.Temperature,
                TopP = botOptions.TopP,
            }
        };
        var promptTemplate = new PromptTemplate(skPrompt, promptTmplConfiguration, Kernel);
        var functionConfig = new SemanticFunctionConfig(promptTmplConfiguration, promptTemplate);
        return functionConfig;
    }

    public async Task<string> Chat(string input) 
    {
        // Save new message in the context variables
        Context.Set("human_input", input);

        // Process the user message and get an answer
        var answer = await Kernel.RunAsync(Context, ChatFunction);

        // Append the new interaction to the chat history
        History += $"\nHuman: {input}\nChatBot: {answer}\n";
        Context.Set("history", History);

        // Show the response
        return Context.ToString();
    }
}