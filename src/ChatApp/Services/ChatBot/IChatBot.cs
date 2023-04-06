using ChatApp.Services.ChatBot.Model;

namespace ChatApp.Services.ChatBot;

public interface IChatBot
{
    public Task Send(string userInput);

    public IEnumerable<ChatInteraction> GetHistory(int? max = default);

    public event AnswerReceivedEventHandler AnswerReceived;
}