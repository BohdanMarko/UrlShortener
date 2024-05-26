using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using UrlShortener.Services;

namespace UrlShortener.Pages;

// TODO: Maybe add some cache
// TODO: Somehow deploy this shit

public sealed class IndexModel : PageModel
{
    private readonly UrlShorteningService _service;

    public IndexModel(UrlShorteningService service) => _service = service;

    public IActionResult OnGet() => Page();

    [BindProperty, Url(ErrorMessage = "Input Url has invalid format")]
    public string InputUrl { get; set; } = string.Empty;

    public string ShortUrl { get; set; } = string.Empty;

    public async Task<IActionResult> OnPostCreateShortenedUrlAsync()
    {
        if (ModelState.IsValid is false) return Page();
        ShortUrl = await _service.CreateShortenedUrl(InputUrl);
        return Page();
    }
}