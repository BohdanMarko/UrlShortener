using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using UrlShortener.Entities;
using UrlShortener.Services;

namespace UrlShortener.Pages;

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

    public string? ShortUrl { get; set; }

    public async Task OnPostAsync()
    {
        if (ModelState.IsValid is false) return;

        var code = await _service.GenerateUniqueCode();

        ShortenedUrl shortenedUrl = new()
        {
            ID = Guid.NewGuid(),
            LongUrl = InputUrl,
            ShortUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/{code}",
            Code = code,
            CreatedAtUtc = DateTime.UtcNow
        };

        ShortUrl = shortenedUrl.ShortUrl;

        await _dbContext.ShortenedUrls.AddAsync(shortenedUrl);
        await _dbContext.SaveChangesAsync();
    }
}
