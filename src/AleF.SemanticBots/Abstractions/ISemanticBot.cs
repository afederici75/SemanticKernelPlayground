namespace AleF.SemanticKernel.ChatBot.Abstractions;

/// <summary>
/// This interface defines the contract for an interactive bot 
/// that understands the semantics of natural language and can 
/// return a response to a user's prompt.
/// </summary>
public interface ISemanticBot
{
    /// <summary>
    /// Sends a prompt to the chat bot.    
    /// </summary>
    /// <param name="prompt">The user's prompt.</param>
    /// <returns>A task that completes when the answer is received. See <see cref="PromptAnswered">.</returns>
    public Task<string> Send(string prompt, CancellationToken cancellationToken = default);
}