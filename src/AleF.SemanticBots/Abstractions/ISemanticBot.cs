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

    /// <summary>
    /// Returns all chat exchanges that have been performed with the bot.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<ChatExchange> GetHistory();

    // TODO: add 'facts' which would be statements the bot knows about before the chat starts.
    // For instance a fact could be my name is Alessandro and I am 47 years old.

    // TODO: add ability to load native and semantics functions and configure the bot.
    // A Configure() method or some fluent builder extensions could help.
}