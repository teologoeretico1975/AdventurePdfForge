namespace AdventurePdfForge.Pipeline.ImageProviders;

/// <summary>
/// Abstraction for image generation backends (OpenAI, ComfyUI, etc.).
/// </summary>
public interface IImageProvider
{
    string Name { get; }

    /// <summary>
    /// Generates an image from a text prompt and saves it to the specified output path.
    /// </summary>
    Task<bool> GenerateImageAsync(string prompt, string outputPath, int width, int height);
}
