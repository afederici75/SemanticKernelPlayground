using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AleF.SemanticBots.Tests;

// See https://github.com/pengweiqhca/Xunit.DependencyInjection

public class Startup
{
    public void ConfigureHost(IHostBuilder hostBuilder) =>
        hostBuilder
            .ConfigureHostConfiguration(builder => builder
                .AddJsonFile("testSettings.json")
                .AddUserSecrets<Startup>()
            );

    public void ConfigureServices(IServiceCollection services, HostBuilderContext context)
    {        
        services.AddSemanticBots(context.Configuration);        
    }
}
