using OpenAI;
using OpenAI.Images;

namespace AdventurePdfForge.Pipeline.ImageProviders;

/// <summary>
/// Generates images using OpenAI DALL-E 3 API.
/// Requires the OPENAI_API_KEY environment variable.
/// </summary>
public class OpenAiImageProvider : IImageProvider
{
    public string Name => "OpenAI DALL-E 3";

    public async Task<bool> GenerateImageAsync(string prompt, string outputPath, int width, int height)
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Console.WriteLine("      ❌ OPENAI_API_KEY environment variable not set.");
            return false;
        }

        var client = new OpenAIClient(apiKey);
        var imageClient = client.GetImageClient("dall-e-3");

        var size = (width, height) switch
        {
            (1024, 1792) => GeneratedImageSize.W1024xH1792,
            (1792, 1024) => GeneratedImageSize.W1792xH1024,
            _ => GeneratedImageSize.W1024xH1024
        };

        var options = new ImageGenerationOptions
        {
            Quality = GeneratedImageQuality.High,
            Size = size,
            Style = GeneratedImageStyle.Vivid,
            ResponseFormat = GeneratedImageFormat.Bytes
        };

        var result = await imageClient.GenerateImageAsync(prompt, options);
        await File.WriteAllBytesAsync(outputPath, result.Value.ImageBytes.ToArray());
        return true;
    }
}
