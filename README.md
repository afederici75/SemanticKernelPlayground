# Semantic Bots Library
This library is a collection of bots that can be used to build semantic applications.
It is based on the [Microsoft Semantic Kernel](https://github.com/microsoft/semantic-kernel).

## Installation and Usage
To use the library, simply reference the assembly AleF.SemanticBots in your project and call the
extension method AddSemanticBots() during Dependency Injection configuration:

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets(typeof(App).Assembly);

// Add services to the container.
builder.Services.AddSemanticBots(builder.Configuration);
```

The extesion method AddSemanticBots() will take care of registering all the required services 
(e.g. ISemanticBot) and their configuration (e.g. NLPServiceOptions and SemanticBotOptions).

## ISemanticBot Service
Once SemanticBots is configured, you can inject the ISemanticBot service in your code and start 
using it.

The service interface is defined as:
```csharp
public interface ISemanticBot
{
    public Task<string> Send(string prompt, CancellationToken cancellationToken = default);
    public ContextVariables Context { get; }
    public IEnumerable<ChatExchange> GetHistory();
}
```

## Using ISemanticBot to Create a Chat Application
The sample BlazorChat application shows how to use the ISemanticBot service to create a chat application.
The main Chat.razor page is shown below:
```razor
@page "/chat"
@inject ISemanticBot bot

<PageTitle>Chat</PageTitle>

<div class="chat-container">
    <div class="chat-history">
        @foreach (var item in bot.GetHistory())
        {
            <ChatInteractionCard Item="item" />
        }
    </div>

    <div class="user-input">
        <input type="text" @bind-value="@Ask" />
        <button @onclick="SendPrompt">Add</button>
        <button @onclick="RecallAllMemories">Recall Memory</button>
    </div>
</div>

@code {
    public string Ask { get; set; } = "I am Alessandro and I am 47 years old";

    async Task SendPrompt()
    {
        var answer = await bot.Send(Ask);
    }

    Task RecallAllMemories()
    {
        string[] BasicQuestions =
        {
            "Say all that you remember from our chat so far.",
        };

        Ask = BasicQuestions[new Random().Next(0, BasicQuestions.Length - 1)];
        return SendPrompt();
    }
}

```

## Notes and Limitations
The SemanticBots library is still in its early stages and is not yet ready for production use.
Most of what I am doing at this point is figuring out the best way to implement the library and
become familiar with Semantic Kernel.

## Important files
1. https://github.com/afederici75/SemanticKernelPlayground/blob/main/src/AleF.SemanticBots/Abstractions/ISemanticBot.cs
   The ISemanticBot interface. This is the main interface that you will use to interact with the library.

2. https://github.com/afederici75/SemanticKernelPlayground/blob/main/src/AleF.SemanticBots/Model/ChatExchange.cs
   The ChatExchange class is used to record the chat history. It contains the prompt and the answer.

3. https://github.com/afederici75/SemanticKernelPlayground/blob/main/src/AleF.SemanticBots/Bots/SemanticBot.cs
   The Bot implementation itself. It shows a rudimentary use of memory and it's likely in need of refactoring.
   See questions below about memory.

## Big Questions and Next Steps
1. ChatGPT's client app shows that responses are received in a stream. How do I implement this in
   a Blazor app? Specifically, how do I change the ISemanticBot interface to use AsyncStream and/or 
   raise events when a new response token is received? I currently wait for the full answer to be
   generated before returning it to the calling app.

2. The approach I used for chat memory is probably flawed. Kernel has a Memory property which
   is probably what I should be using to record the chat history.

3. I think that what I need now is to get familiar with the memory aspect and figure out how to use
   what comes with SK out of the box. It's hard to follow the examples because they are everywhere and
   there's always Python code mixed in.

4. After I figure that stuff out I can then look into the power of semantic functions (native or otherwise).
   Until the memory thing is resolved, semantic functions are mostly useless I think.

5. The Biggest question of them all: how do I feed my bot a big chunk of 'facts' (history).
   For instance, how do I feed it 45 PDFs (or whatever) and get the bot to speak about them?