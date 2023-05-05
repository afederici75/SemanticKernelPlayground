using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AleF.SemanticBots.Bots;


public class EchoBot : ISemanticBot
{    
    List<ChatInteraction> _fakeInteractions = new List<ChatInteraction>();
    public EchoBot()
    {
        for (int i = 0; i < 20; i++)
            Send($"Question #{i} is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s.");
    }

    public IEnumerable<ChatInteraction> GetHistory() => _fakeInteractions;

    public Task<string> Send(string prompt, ChatRequestSettings? settings = null, CancellationToken cancellationToken = default)
    {
        var response = $"The question was '{prompt}'. Did I hear that right?";
        var fakeInteraction = new ChatInteraction(prompt, response, TimeSpan.FromMilliseconds(123));

        _fakeInteractions.Add(fakeInteraction);

        return Task.FromResult(response);
    }


    
}
