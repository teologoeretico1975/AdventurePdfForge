using AdventurePdfForge;
using AdventurePdfForge.Pipeline;
using AdventurePdfForge.Pipeline.Steps;
using Microsoft.Playwright;

// ── Parsing opzioni da riga di comando ──────────────────
var buildAssetPrompt = args.Contains("-buildassetprompt", StringComparer.OrdinalIgnoreCase);
var buildAsset = args.Contains("-buildasset", StringComparer.OrdinalIgnoreCase);

// ── Pipeline setup ──────────────────────────────────────
var context = new PipelineContext
{
    JsonPath = "Data/ombre-nella-cripta.json",
    OutputDir = "Output",
    TemplatesDir = "Templates",
    AssetsDir = "Assets"
};

var pipeline = new PipelineRunner();

if (buildAssetPrompt || buildAsset)
    pipeline.AddStep(new GenerateAssetPromptsStep());

if (buildAsset)
    pipeline.AddStep(new GenerateAssetsStep());

await pipeline.RunAsync(context);

// ── Generazione PDF (logica esistente, sarà migrata in step dedicati) ──

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

// Se la pipeline non ha deserializzato l'avventura, lo facciamo qui
if (context.Adventure is null)
{
    var json = await File.ReadAllTextAsync(context.JsonPath);
    context.Adventure = System.Text.Json.JsonSerializer.Deserialize<Adventure>(
        json,
        new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
}

var data = context.Adventure!;

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
    .Replace("{{CSS_PATH}}", cssPath)
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