namespace ChatApp.Services.ChatBot.Model;

public record ChatInteraction(string Ask, string Answer, TimeSpan Duration)
{
    public override string ToString()
    {
        return $"[{Duration.TotalMilliseconds}] Q: '{Ask}', A: '{Answer}'";
    }
}