namespace ChatApp.Services;

public delegate Task AnswerReceivedEventHandler(string userInput, string answer);

public class AnswerReceived : EventArgs
{
    public AnswerReceived(string userInput, string answer)
    {
        UserInput = userInput;
        Answer = answer;
    }

    public string UserInput { get; }
    public string Answer { get; }
}
