using OpenAI;
using OpenAI.Images;

namespace AdventurePdfForge.Pipeline.Steps;

/// <summary>
/// Calls OpenAI DALL-E to generate images from the prompts
/// stored in <see cref="PipelineContext.AssetPrompts"/>.
/// Requires the OPENAI_API_KEY environment variable.
/// </summary>
public class GenerateAssetsStep : IPipelineStep
{
    public string Name => "Asset generation (DALL-E)";
    public bool IsOptional => false;

    private static readonly Dictionary<string, GeneratedImageSize> AssetSizes = new()
    {
        ["scene.png"] = GeneratedImageSize.W1024xH1792,      // 2:3 portrait cover
        ["parchment.png"] = GeneratedImageSize.W1024xH1792,   // A4-like background
        ["frame.png"] = GeneratedImageSize.W1024xH1792         // A4-like frame
    };

    public async Task<bool> ExecuteAsync(PipelineContext context)
    {
        if (context.AssetPrompts.Count == 0)
        {
            Console.WriteLine("    ⚠️ No asset prompts found — skipping generation.");
            return false;
        }

        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Console.WriteLine("    ❌ OPENAI_API_KEY environment variable not set.");
            return false;
        }

        var client = new OpenAIClient(apiKey);
        var imageClient = client.GetImageClient("dall-e-3");

        Directory.CreateDirectory(context.AssetsDir);

        foreach (var (assetName, prompt) in context.AssetPrompts)
        {
            Console.WriteLine($"    🎨 Generating {assetName}...");

            var size = AssetSizes.GetValueOrDefault(assetName, GeneratedImageSize.W1024xH1792);

            var options = new ImageGenerationOptions
            {
                Quality = GeneratedImageQuality.High,
                Size = size,
                Style = GeneratedImageStyle.Vivid,
                ResponseFormat = GeneratedImageFormat.Bytes
            };

            var result = await imageClient.GenerateImageAsync(prompt, options);
            var imageBytes = result.Value.ImageBytes;

            var outputPath = Path.Combine(context.AssetsDir, assetName);
            await File.WriteAllBytesAsync(outputPath, imageBytes.ToArray());

            context.AssetPaths[assetName] = outputPath;
            Console.WriteLine($"    ✅ {assetName} saved to {outputPath}");
        }

        return true;
    }
}
