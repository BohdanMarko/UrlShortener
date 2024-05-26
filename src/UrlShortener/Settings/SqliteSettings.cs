namespace UrlShortener.Settings;

public sealed class SqliteSettings : IStorageSettings
{
    private readonly IConfiguration _configuration;

    public SqliteSettings(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Server => _configuration.GetValue("DB_SERVER", string.Empty)!;

    public string Port => _configuration.GetValue("DB_PORT", string.Empty)!;

    public string User => _configuration.GetValue("DB_USER", string.Empty)!;

    public string Password => _configuration.GetValue("DB_PASSWORD", string.Empty)!;

    public string DatabaseName => _configuration.GetValue("DB_DATABASE", "UrlShortenerDB")!;

    public string GetConnectionString() => $"Data Source={DatabaseName}.db";
}
