namespace ChatApp.ChatBox;

public class ChatBotOptions
{
    public const string SettingName = "ChatBot";

    public int MaxTokens { get; set; } = 2000;
    public double Temperature { get; set; } = 0.7;  
    public double TopP { get; set; } = 0.5; 
    public string[]? History { get; set; }
    public string? NameOfUser { get; set; }
}