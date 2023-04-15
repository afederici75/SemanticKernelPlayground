using static Microsoft.SemanticKernel.AI.ChatCompletion.ChatHistory;

namespace AleF.SemanticBots.Extensions;

public static class SemanticBotChatMessageExtensions
{
    // TODO: unfortunately SK hardcoded the magic strings. I have to repeat them.
    public static bool IsFromUser(this Message message) => message.AuthorRole.Equals("user", StringComparison.OrdinalIgnoreCase);
    public static bool IsFromAssistant(this Message message) => message.AuthorRole.Equals("assistant", StringComparison.OrdinalIgnoreCase);
    public static bool IsFromSystem(this Message message) => message.AuthorRole.Equals("system", StringComparison.OrdinalIgnoreCase);

}