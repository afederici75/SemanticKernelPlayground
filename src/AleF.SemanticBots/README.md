# Semantic Bots Library
This library is a collection of bots that can be used to build semantic applications.
It is based on the [Microsoft Semantic Kernel](https://github.com/microsoft/semantic-kernel).

## Installation and Usage
To use the library, simply reference the assembly AleF.SemanticBots in your project and call the
extension method AddSemanticBots() during Depenceny Injection configuration:

```
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
```
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
```csharp
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