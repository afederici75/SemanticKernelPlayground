using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Configuration;

namespace AleF.SemanticBots.Bots;

public class BasicBot : SemanticBot
{
    public BasicBot(ILogger logger, IOptions<NLPServiceOptions> nlpOptions, IOptions<SemanticBotOptions> botOptions)
        : base(logger, nlpOptions, botOptions)
    {
    }
}
