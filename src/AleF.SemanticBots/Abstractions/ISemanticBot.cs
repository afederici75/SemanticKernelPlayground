using AleF.SemanticKernel.ChatBot.Model;

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
    public Task<string> Send(string prompt, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the context variables of the bot. See <see cref="ContextVariables"/>.
    /// </summary>
    public ContextVariables Context { get; }

    public IEnumerable<ChatExchange> GetHistory();
}