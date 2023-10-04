namespace UrlShortener.Settings;

public sealed class StorageSettings
{
    private readonly IConfiguration _configuration;

    public StorageSettings(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Server => _configuration.GetValue("DB_SERVER", "localhost")!;

    public string Port => _configuration.GetValue("DB_PORT", "1433")!;

    public string User => _configuration.GetValue("DB_USER", "sa")!;

    public string Password => _configuration.GetValue("DB_PASSWORD", "P@ssword12345")!;

    public string Database => _configuration.GetValue("DB_DATABASE", "UrlShortener_DB")!;

    public string GetConnectionString()
        => $"Server={Server},{Port};Initial Catalog={Database};User ID={User};Password={Password};TrustServerCertificate=True;";
}
