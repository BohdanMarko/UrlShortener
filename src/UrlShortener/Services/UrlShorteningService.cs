using Microsoft.EntityFrameworkCore;
using System.Text;

namespace UrlShortener.Services;

public sealed class UrlShorteningService
{
    public const int ShortLinkCharactersCount = 7;
    private const string ShortLinkCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    private readonly ApplicationDbContext _dbContext;

    public UrlShorteningService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
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
}
