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
        context.AssetPrompts["introimage.png"] = BuildIntroImagePrompt(adv);

        foreach (var (asset, prompt) in context.AssetPrompts)
        {
            Console.WriteLine($"    📝 {asset}:");
            Console.WriteLine($"       {prompt[..Math.Min(prompt.Length, 120)]}...");
        }

        return true;
    }

    private static string BuildThemeSummary(Adventure adv)
    {
        var clues = string.Join(", ", adv.ClueBullets.Take(3));
        return $"{adv.Title}, {adv.Subtitle}, {clues}";
    }

    private static string BuildScenePrompt(Adventure adv, string theme)
    {
        var subject = string.Join(", ", adv.IntroText.Take(1)
            .Select(t => t.Length > 150 ? t[..150] : t));

        return $"masterpiece, best quality, ultra detailed, " +
               $"dark fantasy digital painting, tabletop RPG illustration, " +
               //$"{subject}, " +
               $"flooded crypt with shallow water reflections, " +
               $"collapsed stone corridors with broken statues " +
               $"ritual circle glowing faintly with red symbols " +
               $"open sarcophagus with eerie light " +
               $"gothic architecture, dramatic cinematic lighting, volumetric fog, " +
               $"dark moody atmosphere, intricate details, concept art, " +
               $"portrait composition 2:3";




    }

    private static string BuildBackgroundPrompt(string theme)
    {
        return $"masterpiece, best quality, " +
               $"aged parchment paper texture, seamless tileable, " +
               $"warm sepia tones, ochre, subtle coffee stains, " +
               $"worn vintage paper, soft lighting, " +
               $"no text, no writing, no symbols, blank surface, " +
               $"top-down flat view, high resolution texture";
    }

    private static string BuildIntroImagePrompt(Adventure adv)
    {
        var subject = string.Join(", ", adv.IntroText.Take(1)
            .Select(t => t.Length > 150 ? t[..150] : t));
        return $"masterpiece, best quality, ultra detailed, " +
               $"dark fantasy digital painting, tabletop RPG illustration, " +
               $"interior of a shifting underground crypt, narrow stone corridors, ritual markings on walls, candles burning unevenly, eerie atmosphere, dark fantasy horror, cinematic shadows, realistic style, high detail, low light, textured stone surfaces, subtle fog, black and sepia tones, no characters, no text, horizontal composition" +
               $"portrait composition 2:3";
    }

    private static string BuildFramePrompt(string theme)
    {
        return $"simple monochrome gothic line border, " +
               $"single thin dark gray line with tiny gothic pointed arch repeating motif, " +
               $"hairline weight, 1 pixel thin strokes, " +
               $"border runs along extreme outer edge only, " +
               $"occupying less than 2 percent of the image width, " +
               $"huge empty transparent center filling 98 percent of the image, " +
               //$"pure black background, ultraminimalist, " +
               $"monochrome dark gray on black only, no color, no gold, no red, " +
               $"A4 portrait proportion, symmetrical, " +
               $"no ornament, no flowers, no vines, no flourishes, " +
               $"no text, no characters, no central artwork, geometric border only";
    }
}
