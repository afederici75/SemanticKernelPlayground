using AleF.SemanticBots.Bots.BasicBot;
using AleF.SemanticKernel.ChatBot.Model;
using AleF.SemanticKernel.ChatBot.Abstractions;
using AleF.SemanticKernel.Options;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit.Abstractions;

namespace AleF.SemanticBots.Tests;

public class UnitTest1
{
    private readonly ITestOutputHelper Output;

    public UnitTest1(ITestOutputHelper output)
    {
        Output = output;
    }

    [Fact]
    public async Task TestBasicBot()
    {
        var cfg = TestsConfiguration.GetConfiguration();

        ISemanticBot bot = new BasicBot(
            NullLogger.Instance,
            Options.Create(new NLPServiceOptions
            {
                ApiKey = cfg.GetAPIKey()
            }),
            Options.Create(new SemanticBotOptions())); ;
        
        var answer = await bot.Send("What time is it?");
        Assert.NotEqual("Hi there! What would you like to talk about?", answer, StringComparer.OrdinalIgnoreCase);
        Assert.NotEqual("{$BotResponse}", answer, StringComparer.OrdinalIgnoreCase);        
        Assert.NotEqual("{$bot_response}", answer, StringComparer.OrdinalIgnoreCase);
        Output.WriteLine(answer);
    }        
}