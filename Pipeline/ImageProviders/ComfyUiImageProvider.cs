using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace AdventurePdfForge.Pipeline.ImageProviders;

/// <summary>
/// Generates images using a local ComfyUI server.
/// Expects ComfyUI running at http://127.0.0.1:8188.
/// </summary>
public class ComfyUiImageProvider : IImageProvider
{
    public string Name => "ComfyUI (local)";

    private readonly string _baseUrl;
    private readonly string _checkpoint;
    private readonly HttpClient _http = new() { Timeout = TimeSpan.FromMinutes(5) };

    public ComfyUiImageProvider(
        string baseUrl = "http://127.0.0.1:8188",
        string checkpoint = "Juggernaut-XL_v9.safetensors")
    {
        _baseUrl = baseUrl.TrimEnd('/');
        _checkpoint = checkpoint;
    }

    public async Task<bool> GenerateImageAsync(string prompt, string outputPath, int width, int height)
    {
        // Verify ComfyUI is reachable
        try
        {
            await _http.GetAsync($"{_baseUrl}/system_stats");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"      ❌ ComfyUI not reachable at {_baseUrl}: {ex.Message}");
            Console.WriteLine("      💡 Start ComfyUI with: python C:\\Users\\teolo\\ComfyUI\\main.py");
            return false;
        }

        // Build the ComfyUI workflow as a JSON API prompt
        var workflow = BuildWorkflow(prompt, width, height);

        // Queue the prompt — send raw JSON to preserve array references
        var payload = new JsonObject { ["prompt"] = workflow };
        var queueResponse = await _http.PostAsync(
            $"{_baseUrl}/prompt",
            new StringContent(payload.ToJsonString(), System.Text.Encoding.UTF8, "application/json"));

        if (!queueResponse.IsSuccessStatusCode)
        {
            var error = await queueResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"      ❌ ComfyUI queue failed: {error}");
            return false;
        }

        var queueResult = await queueResponse.Content.ReadFromJsonAsync<JsonNode>();
        var promptId = queueResult?["prompt_id"]?.GetValue<string>();

        if (string.IsNullOrEmpty(promptId))
        {
            Console.WriteLine("      ❌ ComfyUI returned no prompt_id.");
            return false;
        }

        // Poll for completion
        Console.WriteLine($"      ⏳ Waiting for ComfyUI (prompt {promptId[..8]})...");
        string? filename = null;
        string? subfolder = null;

        for (var i = 0; i < 120; i++) // max 10 minutes
        {
            await Task.Delay(5000);

            var historyResponse = await _http.GetFromJsonAsync<JsonNode>($"{_baseUrl}/history/{promptId}");
            var outputs = historyResponse?[promptId]?["outputs"];

            if (outputs is null) continue;

            // Find the SaveImage node output (node "9" in our workflow)
            var images = outputs["9"]?["images"];
            if (images is not JsonArray { Count: > 0 } arr) continue;

            filename = arr[0]?["filename"]?.GetValue<string>();
            subfolder = arr[0]?["subfolder"]?.GetValue<string>() ?? "";
            break;
        }

        if (filename is null)
        {
            Console.WriteLine("      ❌ ComfyUI generation timed out.");
            return false;
        }

        // Download the generated image
        var imageUrl = $"{_baseUrl}/view?filename={Uri.EscapeDataString(filename)}&subfolder={Uri.EscapeDataString(subfolder)}&type=output";
        var imageBytes = await _http.GetByteArrayAsync(imageUrl);
        await File.WriteAllBytesAsync(outputPath, imageBytes);

        return true;
    }

    /// <summary>
    /// Builds a minimal SDXL txt2img workflow for the ComfyUI API.
    /// Node IDs: 1=Checkpoint, 3=KSampler, 4=EmptyLatent, 5=CLIPPositive, 6=CLIPNegative, 7=VAEDecode, 9=SaveImage
    /// </summary>
    private JsonNode BuildWorkflow(string prompt, int width, int height)
    {
        var negativePrompt = "text, watermark, signature, logo, caption, title, font, letters, words, " +
            "blurry, low quality, worst quality, jpeg artifacts, pixelated, " +
            "deformed, ugly, disfigured, mutated, extra limbs, bad anatomy, " +
            "oversaturated, underexposed, cropped, out of frame, nsfw";

        var workflow = JsonNode.Parse($$"""
        {
            "1": {
                "class_type": "CheckpointLoaderSimple",
                "inputs": {
                    "ckpt_name": "{{_checkpoint}}"
                }
            },
            "4": {
                "class_type": "EmptyLatentImage",
                "inputs": {
                    "width": {{width}},
                    "height": {{height}},
                    "batch_size": 1
                }
            },
            "5": {
                "class_type": "CLIPTextEncode",
                "inputs": {
                    "text": {{JsonSerializer.Serialize(prompt)}},
                    "clip": ["1", 1]
                }
            },
            "6": {
                "class_type": "CLIPTextEncode",
                "inputs": {
                    "text": {{JsonSerializer.Serialize(negativePrompt)}},
                    "clip": ["1", 1]
                }
            },
            "3": {
                "class_type": "KSampler",
                "inputs": {
                    "model": ["1", 0],
                    "positive": ["5", 0],
                    "negative": ["6", 0],
                    "latent_image": ["4", 0],
                    "seed": {{Random.Shared.Next()}},
                    "steps": 25,
                    "cfg": 7.0,
                    "sampler_name": "euler_ancestral",
                    "scheduler": "normal",
                    "denoise": 1.0
                }
            },
            "7": {
                "class_type": "VAEDecode",
                "inputs": {
                    "samples": ["3", 0],
                    "vae": ["1", 2]
                }
            },
            "9": {
                "class_type": "SaveImage",
                "inputs": {
                    "images": ["7", 0],
                    "filename_prefix": "pdfforge"
                }
            }
        }
        """)!;

        return workflow;
    }
}
