using AdventurePdfForge.Pipeline.ImageProviders;

namespace AdventurePdfForge.Pipeline;

/// <summary>
/// Contesto condiviso tra tutti gli step della pipeline.
/// </summary>
public class PipelineContext
{
    public required string JsonPath { get; init; }
    public required string OutputDir { get; init; }
    public required string TemplatesDir { get; init; }
    public required string AssetsDir { get; init; }

    /// <summary>Image generation backend to use.</summary>
    public IImageProvider? ImageProvider { get; set; }

    /// <summary>DPI target per le immagini generate (default 300). Valori più bassi producono file più leggeri.</summary>
    public int ImageDpi { get; init; } = 300;

    /// <summary>Se valorizzato, limita la generazione al solo asset specificato (es. "frame.png").</summary>
    public string? AssetFilter { get; init; }

    /// <summary>Dati dell'avventura deserializzati dal JSON.</summary>
    public Adventure? Adventure { get; set; }

    /// <summary>Prompt generate per ogni asset (chiave = nome file asset).</summary>
    public Dictionary<string, string> AssetPrompts { get; } = new();

    /// <summary>Percorsi dei file asset generati/disponibili.</summary>
    public Dictionary<string, string> AssetPaths { get; } = new();

    /// <summary>HTML finale pronto per la conversione in PDF.</summary>
    public string? FinalHtml { get; set; }

    /// <summary>Percorso del PDF generato.</summary>
    public string? PdfPath { get; set; }
}
