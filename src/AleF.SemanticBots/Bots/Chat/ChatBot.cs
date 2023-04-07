using Microsoft.Extensions.Logging;
using System.Text;

namespace AleF.SemanticBots.Bots.Chat;

public class ChatBot : SemanticBot
{
    public ChatBot(
        ILogger logger, 
        IOptions<NLPServiceOptions> nlpOptions, 
        IOptions<SemanticBotOptions> botOptions) 
        : base(logger, nlpOptions, botOptions)
    {
    }

    public static class Params
    {
        public const string FactsParam = "facts";
        public const string HistoryParam = "history";
        public const string UserInputParam = "userInput";
    }

    //protected override async Task<SKContext> CreateContextAsync()
    //{
    //    var context = Kernel.CreateNewContext();
    //    context[Params.HistoryParam] = string.Empty;
    //
    //    //if (Options.Facts is not null)
    //    //{
    //    //    const string MemoryCollectionName = "FactsCollection";
    //    //
    //    //    var idx = 0;
    //    //    var sb = new StringBuilder();
    //    //    foreach (var fact in chatOptions.Facts)
    //    //    {
    //    //        // Adds to the memory but it does not seem to affect the response
    //    //        // Does this work only with Search? What is the purpose of this?
    //    //        await kernel.Memory.SaveInformationAsync(
    //    //                collection: MemoryCollectionName,
    //    //                id: $"info{idx++}",
    //    //                text: fact.Question);
    //    //
    //    //        // Adds a fact to the facts
    //    //        sb.AppendLine(fact.Question + ' ' + fact.Answer);
    //    //    }
    //    //    context[Params.FactsParam] = sb.ToString();
    //    //}
    //
    //    return context;
    //}

    protected override string GetPrompt()
    {
        const string Prompt = @"";
        return Prompt;
    }
}