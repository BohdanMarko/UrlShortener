using Microsoft.EntityFrameworkCore;
using UrlShortener;
using UrlShortener.Extensions;
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

builder.Services.AddRazorPages();
builder.Services.AddScoped<UrlShorteningService>();
builder.Services.AddScoped<IStorageSettings, SqliteSettings>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.ApplyPendingMigrations();

app.MapGet("{code}", async (string code, ApplicationDbContext dbContext) =>
{
    var shortenedUrl = await dbContext.ShortenedUrls.FirstOrDefaultAsync(x => x.Code == code);
    if (shortenedUrl is null) return Results.NotFound();
    return Results.Redirect(shortenedUrl.LongUrl, permanent: true);
});

app.MapGet("api/ping", () => "PONG");

app.MapRazorPages();

app.Run();