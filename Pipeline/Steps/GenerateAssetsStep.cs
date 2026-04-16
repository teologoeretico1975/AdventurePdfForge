namespace AdventurePdfForge.Pipeline.Steps;

/// <summary>
/// Generates images for each asset using the configured <see cref="PipelineContext.ImageProvider"/>.
/// </summary>
public class GenerateAssetsStep : IPipelineStep
{
    public string Name => "Asset generation";
    public bool IsOptional => false;

    /// <summary>
    /// Base asset sizes at 300 DPI (reference). Scaled proportionally for other DPI values.
    /// </summary>
    private static readonly Dictionary<string, (int Width, int Height)> BaseAssetSizes = new()
    {
        ["scene.png"] = (1024, 1792),
        ["parchment.png"] = (1024, 1792),
        ["introimage.png"] = (1792, 1024)
    };

    private const int ReferenceDpi = 300;

    public async Task<bool> ExecuteAsync(PipelineContext context)
    {
        if (context.AssetPrompts.Count == 0)
        {
            Console.WriteLine("    ⚠️ No asset prompts found — skipping generation.");
            return false;
        }

        if (context.ImageProvider is null)
        {
            Console.WriteLine("    ❌ No image provider configured.");
            return false;
        }

        Console.WriteLine($"    🔌 Using provider: {context.ImageProvider.Name}");
        Console.WriteLine($"    📐 DPI target: {context.ImageDpi}");

        Directory.CreateDirectory(context.AssetsDir);

        var assetsToGenerate = context.AssetFilter is not null
            ? context.AssetPrompts
                .Where(kv => kv.Key.Equals(context.AssetFilter, StringComparison.OrdinalIgnoreCase))
                .ToDictionary(kv => kv.Key, kv => kv.Value)
            : context.AssetPrompts;

        if (assetsToGenerate.Count == 0)
        {
            Console.WriteLine($"    ⚠️ Asset '{context.AssetFilter}' not found. Available: {string.Join(", ", context.AssetPrompts.Keys)}");
            return false;
        }

        if (context.AssetFilter is not null)
            Console.WriteLine($"    🎯 Generating only: {context.AssetFilter}");

        foreach (var (assetName, prompt) in assetsToGenerate)
        {
            Console.WriteLine($"    🎨 Generating {assetName}...");
            Console.WriteLine($"       📝 Prompt: {prompt}");

            var (baseW, baseH) = BaseAssetSizes.GetValueOrDefault(assetName, (1024, 1792));
            var scale = context.ImageDpi / (double)ReferenceDpi;
            var width = (int)(baseW * scale);
            var height = (int)(baseH * scale);

            // SDXL richiede dimensioni multiple di 8
            width = width / 8 * 8;
            height = height / 8 * 8;

            var outputPath = Path.Combine(context.AssetsDir, assetName);

            var success = await context.ImageProvider.GenerateImageAsync(prompt, outputPath, width, height);

            if (!success)
            {
                Console.WriteLine($"    ❌ Failed to generate {assetName}.");
                return false;
            }

            context.AssetPaths[assetName] = outputPath;
            Console.WriteLine($"    ✅ {assetName} saved to {outputPath}");
        }

        return true;
    }
}
