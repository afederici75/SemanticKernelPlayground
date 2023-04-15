using AleF.SemanticBots.Bots;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddSemanticBots(this IServiceCollection services, IConfiguration configuration)
    {
        // IOptions
        services.Configure<NLPServiceOptions>(configuration.GetSection(NLPServiceOptions.SettingName));

        // Services        
        services.AddScoped<ISemanticBot, SemanticBot>();
        
        return services;
    }
}