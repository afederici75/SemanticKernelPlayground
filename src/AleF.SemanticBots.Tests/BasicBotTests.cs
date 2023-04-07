using AleF.SemanticKernel.ChatBot.Model;
using AleF.SemanticKernel.ChatBot.Abstractions;
using AleF.SemanticKernel.Options;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit.Abstractions;
using Microsoft.Extensions.Configuration;
using AleF.SemanticBots.Bots;

namespace AleF.SemanticBots.Tests;

public class BasicBotTests
{
    private readonly IOptions<NLPServiceOptions> NlpOptions;
    private readonly ITestOutputHelper Output;

    public BasicBotTests(IOptions<NLPServiceOptions> nlpOptions, ITestOutputHelper output)
    {
        this.NlpOptions = nlpOptions;
        this.Output = output;
    }

    ISemanticBot BuildBasicBot()
    {
        ISemanticBot bot = new BasicBot(
            NullLogger.Instance,
            NlpOptions,
            Options.Create(new SemanticBotOptions()));

        return bot;
    }

    [Fact]
    public async Task BasicBotKnowsTheTime()
    {
        var bot = BuildBasicBot();

        var answer = await bot.Send("What time is it?");

        Assert.NotEqual("Hi there! What would you like to talk about?", answer, StringComparer.OrdinalIgnoreCase);
        Assert.NotEqual("{$BotResponse}", answer, StringComparer.OrdinalIgnoreCase);        
        Assert.NotEqual("{$bot_response}", answer, StringComparer.OrdinalIgnoreCase);
        Assert.NotEqual("{$chatbot_response}", answer, StringComparer.OrdinalIgnoreCase);
        
        Assert.StartsWith("It is currently", answer, StringComparison.OrdinalIgnoreCase);
        Output.WriteLine(answer);
    }

    [Fact]
    public async Task BasicBotRemembersNameAndAge()
    {
        var bot = BuildBasicBot();

        var answer = await bot.Send("My name is Alessandro and I am 47");
        
        var nameAnswer = await bot.Send("What is my name?");
        Assert.Equal("Your name is Alessandro.", nameAnswer);

        var ageAnswer = await bot.Send("What is my age?");
        Assert.Equal("You are 47 years old.", ageAnswer);
    }
}