namespace ChatApp.ChatBox;

public delegate Task AnswerReceived(string userInput, string answer);

public interface IChatBot
{
    public Task Send(string input, AnswerReceived? callback);

    public string GetHistory();
}
