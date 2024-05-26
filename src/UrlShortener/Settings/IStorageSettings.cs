namespace UrlShortener.Settings;

public interface IStorageSettings
{
    string Server { get; }
    string Port { get; }
    string User { get; }
    string Password { get; }
    string DatabaseName { get; }

    string GetConnectionString();
}
