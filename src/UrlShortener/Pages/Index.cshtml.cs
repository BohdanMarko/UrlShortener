using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using UrlShortener.Entities;
using UrlShortener.Services;

namespace UrlShortener.Pages;

// TODO: Maybe add some cache
// TODO: Test asynchronous code!!!!
// TODO: Somehow deploy this shit

public sealed class IndexModel : PageModel
{
    private readonly UrlShorteningService _service;
    private readonly ApplicationDbContext _dbContext;

    public IndexModel(UrlShorteningService service, ApplicationDbContext dbContext)
    {
        _service = service;
        _dbContext = dbContext;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty, Url]
    public string InputUrl { get; set; } = string.Empty;

    public string ShortUrl { get; set; } = string.Empty;

    public async Task<IActionResult> OnPostCreateShortenedUrlAsync()
    {
        await Task.Delay(0);
        if (ModelState.IsValid is false) return Page();

        var existingRecord = await _dbContext.ShortenedUrls.FirstOrDefaultAsync(x => x.LongUrl.Equals(InputUrl));
        if (existingRecord is not null)
        {
            ShortUrl = existingRecord.ShortUrl;
            existingRecord.LastUpdatedAtUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return Page();
        }

        var code = await _service.GenerateUniqueCode();

        ShortenedUrl shortenedUrl = new()
        {
            ID = Guid.NewGuid(),
            LongUrl = InputUrl,
            ShortUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/{code}",
            Code = code,
            CreatedAtUtc = DateTime.UtcNow,
            LastUpdatedAtUtc = DateTime.UtcNow
        };

        ShortUrl = shortenedUrl.ShortUrl;

        await _dbContext.ShortenedUrls.AddAsync(shortenedUrl);
        await _dbContext.SaveChangesAsync();

        return Page();
    }

    public string UniqueCode { get; set; } = string.Empty;

    public async Task<IActionResult> OnPostGenerateUniqueCodeAsync()
    {
        UniqueCode = $"Random number: {await _service.GenerateUniqueCode()}";
        ModelState.Clear();
        return Page();
    }
}
