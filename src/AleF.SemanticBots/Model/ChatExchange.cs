﻿namespace AleF.SemanticKernel.ChatBot.Model;

public class ChatExchange
{
    public ChatExchange(string ask, SKContext response)
    {
        Ask = ask ?? throw new ArgumentNullException(nameof(ask));
        Response = response ?? throw new ArgumentNullException(nameof(response));
        Answer = response.ToString().Trim();
    }

    protected SKContext Response { get; }

    public string Ask { get; }
    public string Answer { get; }

    public override string ToString() => $"[{Ask}] -> [{Answer}]";  
}
