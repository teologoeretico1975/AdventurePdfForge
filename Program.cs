using System.Text;
using System.Text.Json;
using Microsoft.Playwright;

static string ToDataUri(string path)
{
    var bytes = File.ReadAllBytes(path);
    var ext = Path.GetExtension(path).ToLowerInvariant();

    var mime = ext switch
    {
        ".png" => "image/png",
        ".jpg" => "image/jpeg",
        ".jpeg" => "image/jpeg",
        ".webp" => "image/webp",
        ".svg" => "image/svg+xml",
        _ => throw new NotSupportedException($"Estensione non supportata: {ext}")
    };

    return $"data:{mime};base64,{Convert.ToBase64String(bytes)}";
}

var json = await File.ReadAllTextAsync("Data/ombre-nella-cripta.json");

var data = JsonSerializer.Deserialize<Adventure>(
    json,
    new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    })!;

var htmlTemplate = await File.ReadAllTextAsync("Templates/document-template.html");

string ToFileUrl(string path)
{
    return new Uri(Path.GetFullPath(path)).AbsoluteUri;
}

var introHtml = string.Join("", data.IntroText.Select(x => $"<p>{x}</p>"));
var summaryHtml = string.Join("", data.SummaryBullets.Select(x => $"<li>{x}</li>"));
var cluesHtml = string.Join("", data.ClueBullets.Select(x => $"<li>{x}</li>"));

var cssPath = ToFileUrl("Templates/styles.css");

var sceneImage = ToDataUri("Assets/scene.png");
var backgroundImage = ToDataUri("Assets/parchment.png");
var frameImage = ToDataUri("Assets/frame.png");

var html = htmlTemplate
    .Replace("{{TITLE}}", data.Title)
    .Replace("{{SUBTITLE}}", data.Subtitle)
    .Replace("{{INTRO_TITLE}}", data.IntroTitle)
    .Replace("{{INTRO_TEXT}}", introHtml)
    .Replace("{{SUMMARY}}", summaryHtml)
    .Replace("{{CLUES}}", cluesHtml)
    .Replace("{{INTRO_IMAGE}}", sceneImage)
    .Replace("{{BACKGROUND}}", backgroundImage)
    .Replace("{{FRAME}}", frameImage);

Directory.CreateDirectory("Output");
await File.WriteAllTextAsync("Output/index.html", html);

using var playwright = await Playwright.CreateAsync();
await using var browser = await playwright.Chromium.LaunchAsync(new()
{
    Headless = true
});

var page = await browser.NewPageAsync();
await page.GotoAsync(new Uri(Path.GetFullPath("Output/index.html")).AbsoluteUri);

await page.PdfAsync(new()
{
    Path = "Output/output.pdf",
    Format = "A4",
    PrintBackground = true,
    Margin = new() { Top = "0", Right = "0", Bottom = "0", Left = "0" }
});

public class Adventure
{
    public string Title { get; set; } = "";
    public string Subtitle { get; set; } = "";
    public string IntroTitle { get; set; } = "";
    public List<string> IntroText { get; set; } = new();
    public List<string> SummaryBullets { get; set; } = new();
    public List<string> ClueBullets { get; set; } = new();
    public string IntroImage { get; set; } = "";
    public string BackgroundImage { get; set; } = "";
    public string FrameImage { get; set; } = "";
}