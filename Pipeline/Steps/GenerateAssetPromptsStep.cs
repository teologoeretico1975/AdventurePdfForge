using System.Text.Json;

namespace AdventurePdfForge.Pipeline.Steps;

/// <summary>
/// Legge il JSON dell'avventura e genera prompt descrittive
/// per la creazione degli asset grafici tramite AI.
/// </summary>
public class GenerateAssetPromptsStep : IPipelineStep
{
    public string Name => "Generazione prompt asset";
    public bool IsOptional => false;

    public async Task<bool> ExecuteAsync(PipelineContext context)
    {
        if (context.Adventure is null)
        {
            var json = await File.ReadAllTextAsync(context.JsonPath);
            context.Adventure = JsonSerializer.Deserialize<Adventure>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        var adv = context.Adventure;
        var theme = BuildThemeSummary(adv);

        context.AssetPrompts["scene.png"] = BuildScenePrompt(adv, theme);
        context.AssetPrompts["parchment.png"] = BuildBackgroundPrompt(theme);
        context.AssetPrompts["frame.png"] = BuildFramePrompt(theme);

        foreach (var (asset, prompt) in context.AssetPrompts)
        {
            Console.WriteLine($"    📝 {asset}:");
            Console.WriteLine($"       {prompt[..Math.Min(prompt.Length, 120)]}...");
        }

        return true;
    }

    private static string BuildThemeSummary(Adventure adv)
    {
        var introSnippet = string.Join(" ", adv.IntroText.Take(2));
        var clues = string.Join(", ", adv.ClueBullets.Take(3));

        return $"Title: \"{adv.Title}\". Subtitle: \"{adv.Subtitle}\". " +
               $"Setting: {introSnippet} " +
               $"Key elements: {clues}.";
    }

    private static string BuildScenePrompt(Adventure adv, string theme)
    {
        return $"Digital painting, dark fantasy illustration for a tabletop RPG adventure. " +
               $"Scene inspired by: {theme} " +
               $"The image should depict: {string.Join("; ", adv.IntroText)}. " +
               $"Cinematic lighting, dramatic atmosphere, highly detailed, " +
               $"no text, no watermarks, 2:3 portrait aspect ratio.";
    }

    private static string BuildBackgroundPrompt(string theme)
    {
        return $"Seamless aged parchment paper texture for a dark fantasy RPG document. " +
               $"Theme context: {theme} " +
               $"Warm sepia and ochre tones, subtle stains, worn edges feel, " +
               $"tileable, no text, no symbols, soft vintage look.";
    }

    private static string BuildFramePrompt(string theme)
    {
        return $"Thin ornamental border frame for a dark fantasy RPG page. " +
               $"Theme context: {theme} " +
               $"Gothic style, dark red (#7a1f1f) and antique gold accents, " +
               $"very thin elegant lines, transparent center (PNG with alpha), " +
               $"A4 portrait proportion, no text.";
    }
}
