using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortener;
using UrlShortener.Entities;
using UrlShortener.Extensions;
using UrlShortener.Models;
using UrlShortener.Services;
using UrlShortener.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
{
    var settings = sp.GetRequiredService<IStorageSettings>();
    options.UseSqlite(settings.GetConnectionString());
});

builder.Services.AddScoped<UrlShorteningService>();
builder.Services.AddScoped<IStorageSettings, SqliteSettings>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.ApplyMigration();

app.MapPost("api/shorten", async (
    [FromBody] ShortenUrlRequest request,
    UrlShorteningService urlShorteningService,
    ApplicationDbContext dbContext,
    HttpContext httpContext) =>
{
    if (Uri.TryCreate(request.Url, UriKind.Absolute, out _) is false)
        return Results.BadRequest("Invalid URL");

    var code = await urlShorteningService.GenerateUniqueCode();

    ShortenedUrl shortenedUrl = new()
    {
        ID = Guid.NewGuid(),
        LongUrl = request.Url,
        ShortUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/api/{code}",
        Code = code,
        CreatedAtUtc = DateTime.UtcNow
    };

    await dbContext.ShortenedUrls.AddAsync(shortenedUrl);
    await dbContext.SaveChangesAsync();

    return Results.Ok(new ShortenUrlResponse(shortenedUrl.ShortUrl));
});

app.MapGet("api/{code}", async (string code, ApplicationDbContext dbContext) =>
{
    var shortenedUrl = await dbContext.ShortenedUrls.FirstOrDefaultAsync(x => x.Code == code);

    if (shortenedUrl is null)
        return Results.NotFound();

    return Results.Redirect(shortenedUrl.LongUrl, permanent: true);
});

app.MapGet("api/ping", () => "PONG");

app.Run();