namespace AleF.SemanticKernel.ChatBot.Model;

public class ChatInteraction
{
    public ChatInteraction(string prompt, string reply, TimeSpan duration)
    {
        Prompt = prompt;
        Reply = reply;
        Duration = duration;
        OccurredOnUTC = DateTime.UtcNow;
    }
    
    public string Prompt { get; }
    public string Reply { get; }
    public TimeSpan Duration { get; }
    public DateTime OccurredOnUTC { get; }

    public override string ToString() => $"[{Prompt}] -> [{Reply}][{Duration.TotalMilliseconds:0}ms]";  
}