using Microsoft.Extensions.Configuration;

public static class Config
{
    private static IConfigurationRoot _config;

    static Config()
    {
        _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
    }

    public static string ConnectionString =>
        _config.GetValue<string>("ConnectionString");
}
