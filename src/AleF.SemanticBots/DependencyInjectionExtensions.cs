using AleF.SemanticBots.Bots;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddSemanticBots(this IServiceCollection services, IConfiguration configuration)
    {
        // IOptions
        services.Configure<NLPServiceOptions>(configuration.GetSection(NLPServiceOptions.SettingName));

        // Services        
        services.AddScoped<IKernel>(sp =>
        {
            var logFact = sp.GetRequiredService<ILoggerFactory>();

            var kernel = Kernel.Builder
                .WithLogger(logFact.CreateLogger<IKernel>())
                .Configure(cfg =>
                {                    
                    var options = sp.GetRequiredService<IOptions<NLPServiceOptions>>().Value;

                    // IMPORTANT: if the model is not "gpt-3.5-turbo" we get errors in the HTTP calls later.
                    cfg.AddOpenAIChatCompletionService("chat", options.ChatModel, options.ApiKey);
                })
                // TODO: several other WithXXX to look into...
                //.WithMemoryStorage(new VolatileMemoryStore())
                .Build();

            return kernel;
        });
        services.AddScoped<ISemanticBot, SemanticBot>();
        
        return services;
    }
}