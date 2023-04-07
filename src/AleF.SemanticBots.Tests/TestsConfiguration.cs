using Microsoft.Extensions.Configuration;

namespace AleF.SemanticBots.Tests;

public static class TestsConfiguration
{
    public static IConfiguration GetConfiguration()
    {
        var cfg = new ConfigurationBuilder()
                        .AddJsonFile("testSettings.json")
                        .AddUserSecrets<UnitTest1>()
                        .Build();    

        return cfg;
    }

    public static string GetAPIKey(this IConfiguration configuration)
    {
        return configuration["NLPService:APIKey"] ?? throw new Exception("Forgot to preparte the secrets?");
    }
}
