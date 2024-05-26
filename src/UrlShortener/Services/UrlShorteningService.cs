using Microsoft.EntityFrameworkCore;
using System.Text;
using UrlShortener.Entities;

namespace UrlShortener.Services;

public sealed class UrlShorteningService
{
    public const int ShortLinkCharactersCount = 7;
    private const string ShortLinkCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UrlShorteningService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> GenerateUniqueCode()
    {
        var builder = new StringBuilder();

        for (var i = 0; i < ShortLinkCharactersCount; i++)
        {
            var index = Random.Shared.Next(0, ShortLinkCharacters.Length);
            builder.Append(ShortLinkCharacters[index]);
        }
        
        var code = builder.ToString();

        if (await _dbContext.ShortenedUrls.AnyAsync(x => x.Code == code))
            return await GenerateUniqueCode();

        return code;
    }

    public async Task<string> CreateShortenedUrl(string inputUrl)
    {
        var existingRecord = _dbContext.ShortenedUrls.FirstOrDefault(x => x.LongUrl.Equals(inputUrl));
        if (existingRecord is not null)
        {
            existingRecord.LastUpdatedAtUtc = DateTime.UtcNow;
            _dbContext.SaveChanges();
            return existingRecord.ShortUrl;
        }

        var code = await GenerateUniqueCode();

        ShortenedUrl shortenedUrl = new()
        {
            ID = Guid.NewGuid(),
            LongUrl = inputUrl,
            ShortUrl = $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext!.Request.Host}/{code}",
            Code = code,
            CreatedAtUtc = DateTime.UtcNow,
            LastUpdatedAtUtc = DateTime.UtcNow
        };

        _dbContext.ShortenedUrls.Add(shortenedUrl);
        _dbContext.SaveChanges();

        return shortenedUrl.ShortUrl;
    }
}
