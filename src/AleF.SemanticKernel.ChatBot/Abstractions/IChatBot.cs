using AleF.SemanticKernel.ChatBot.Model;

namespace AleF.SemanticKernel.ChatBot.Abstractions;

/// <summary>
/// This interface defines the contract for a chat bot.
/// </summary>
public interface IChatBot
{
    /// <summary>
    /// Sends a prompt to the chat bot.    
    /// </summary>
    /// <param name="prompt">The user's prompt.</param>
    /// <returns>A task that completes when the answer is received.</returns>
    public Task Send(string prompt);

    /// <summary>
    /// TBC
    /// </summary>
    /// <param name="max"></param>
    /// <returns></returns>
    public IEnumerable<ChatInteraction> GetHistory(int? max = default);

    /// <summary>
    /// This event is raised when the chat bot has received an answer.
    /// See <see cref="AnswerReceivedEventHandler"/> and <see cref="AnswerReceivedEventArgs"/>.
    /// </summary>
    public event AnswerReceivedEventHandler AnswerReceived;
}