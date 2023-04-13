namespace AleF.SemanticBots.Tests;

public class BasicSemanticBotTests
{    
    [Theory]
    [InlineData(null, null)]
    public async Task BasicBotKnowsTheTime(
        [FromServices] ISemanticBot bot,
        [FromServices] ITestOutputHelper output)
    {
        var answer = await bot.Send("What time is it?");

        answer.Should().NotBe("Hi there! What would you like to talk about?");
        answer.Should().NotBe("{$BotResponse}");        
        answer.Should().NotBe("{$bot_response}");
        answer.Should().NotBe("{$chatbot_response}");

        answer.ToLower().Should().ContainAny("am", "pm"); // It is now 12:31pm
        
        output.WriteLine(answer);
    }

    [Theory]
    [InlineData(null)]
    public async Task BasicBotRemembersNameAndAge(
        [FromServices] ISemanticBot bot)
    {
        var answer = await bot.Send("I am Alessandro and I am 47 years old");
        
        var nameAnswer = await bot.Send("What is my name? Answer with the name only");
        nameAnswer.Should().Contain("Alessandro");

        var ageAnswer = await bot.Send("What is my age? Answer with the number only");
        ageAnswer.Should().Contain("47");
    }
}