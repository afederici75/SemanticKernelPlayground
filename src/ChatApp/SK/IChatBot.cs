namespace ChatApp.ChatBox;

public interface IChatBot
{
    public Task<string> Chat(string input);

    public string GetHistory();
}
