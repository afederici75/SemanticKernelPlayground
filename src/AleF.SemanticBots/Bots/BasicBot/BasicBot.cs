using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Configuration;

namespace AleF.SemanticBots.Bots.BasicBot;

public class BasicBot : SemanticBot
{
    public BasicBot(ILogger logger, IOptions<NLPServiceOptions> nlpOptions, IOptions<SemanticBotOptions> botOptions) 
        : base(logger, nlpOptions, botOptions)
    {
    }

    //protected override string GetPrompt() => string.Empty;
    //
    //protected override void ConfigureKernel(KernelConfig config)
    //{
    //    base.ConfigureKernel(config);
    //
    //    // Refer to https://github.com/microsoft/semantic-kernel/discussions/136#discussioncomment-5400791
    //    
    //    // TODO: make more generic so it can handle Azure, too
    //    config.AddOpenAITextCompletion(GetType().Name, NLPOptions.Model, NLPOptions.ApiKey, NLPOptions.Organization);
    //}
}
