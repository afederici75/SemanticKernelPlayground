namespace ChatApp.Services.Implementations;

public class ChatBotOptions
{
    public const string SettingName = "ChatBot";

    public int MaxTokens { get; set; } = 2000;
    public double Temperature { get; set; } = 0.7;
    public double Relevance { get; set; } = 0.8;
    public double TopP { get; set; } = 0.5;
    public Fact[]? Facts { get; set; } = Array.Empty<Fact>();
}
