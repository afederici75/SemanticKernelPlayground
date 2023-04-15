using Microsoft.SemanticKernel.AI.ChatCompletion;
using static Microsoft.SemanticKernel.AI.ChatCompletion.ChatHistory;

namespace AleF.SemanticKernel.ChatBot.Abstractions;

/// <summary>
/// This interface defines the contract for an interactive bot 
/// that understands the semantics of natural language and can 
/// return a response to a user's prompt.
/// </summary>
public interface ISemanticBot
{
    /// <summary>
    /// Sends a prompt to the bot and returns the answer.
    /// </summary>
    /// <param name="prompt">The prompt.</param>
    /// <returns>The bot's reply.</returns>
    public Task<string> Send(string prompt, ChatRequestSettings? settings = default, CancellationToken cancellationToken = default);

    public IEnumerable<Message> GetHistory();
}