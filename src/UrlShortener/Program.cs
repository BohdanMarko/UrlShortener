using Microsoft.EntityFrameworkCore;
using UrlShortener;
using UrlShortener.Extensions;
using UrlShortener.Services;
using UrlShortener.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
{
    var settings = sp.GetRequiredService<IStorageSettings>();
    options.UseSqlite(settings.GetConnectionString());
});

builder.Services.AddRazorPages();
builder.Services.AddScoped<UrlShorteningService>();
builder.Services.AddScoped<IStorageSettings, SqliteSettings>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseHttpsRedirection();

app.ApplyPendingMigrations();

app.MapGet("{code}", async (string code, ApplicationDbContext dbContext) =>
{
    var shortenedUrl = await dbContext.ShortenedUrls.FirstOrDefaultAsync(x => x.Code == code);
    if (shortenedUrl is null) return Results.NotFound();
    return Results.Redirect(shortenedUrl.LongUrl, permanent: true);
});

app.MapPost("api/refresh", async (ApplicationDbContext dbContext) =>
{
    dbContext.ShortenedUrls.RemoveRange(await dbContext.ShortenedUrls.ToListAsync());
    await dbContext.SaveChangesAsync();
    return Results.Ok();
});

app.MapGet("api/ping", () => "PONG");

app.MapRazorPages();

app.Run();