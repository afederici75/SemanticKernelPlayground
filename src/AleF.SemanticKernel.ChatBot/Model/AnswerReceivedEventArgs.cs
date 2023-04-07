namespace AleF.SemanticKernel.ChatBot.Model;

public delegate Task AnswerReceivedEventHandler(object sender, ChatInteraction interaction);

public class AnswerReceivedEventArgs : EventArgs
{
    public AnswerReceivedEventArgs(ChatInteraction interaction)
    {        
        Interaction = interaction;        
    }

    public ChatInteraction Interaction { get; }    
}
