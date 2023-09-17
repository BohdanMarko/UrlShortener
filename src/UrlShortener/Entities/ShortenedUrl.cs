namespace UrlShortener.Entities;

public sealed class ShortenedUrl
{
    public Guid ID { get; set; }

    public string LongUrl { get; set; } = string.Empty;

    public string ShortUrl { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }
}
