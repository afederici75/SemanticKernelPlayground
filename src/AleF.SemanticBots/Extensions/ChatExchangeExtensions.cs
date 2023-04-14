namespace AleF.SemanticBots.Extensions;

/// <summary>
/// Extensions for <see cref="ChatExchange"/>.
/// </summary>
public static class ChatExchangeExtensions
{
    /// <summary>
    /// This method converts a <see cref="ChatExchange"/> to a string.   
    /// </summary>
    public static string ToChatHistoryString(this ChatExchange item, string humanTag = Tags.Human, string botTag = Tags.ChatBot)
    {
        return $"\n{humanTag}: {item.Ask}\n{botTag}: {item.Answer}\n"; ;
    }
}