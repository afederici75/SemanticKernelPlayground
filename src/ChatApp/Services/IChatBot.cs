namespace ChatApp.Services;

public interface IChatBot
{
    public Task Send(string userInput);

    public string GetHistory();

    public event AnswerReceivedEventHandler AnswerReceived;
}