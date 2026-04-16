namespace AdventurePdfForge.Pipeline;

/// <summary>
/// Singolo step della pipeline di generazione PDF.
/// </summary>
public interface IPipelineStep
{
    /// <summary>Nome descrittivo dello step (per logging).</summary>
    string Name { get; }

    /// <summary>Indica se lo step è opzionale e può essere saltato.</summary>
    bool IsOptional { get; }

    /// <summary>Esegue lo step. Restituisce true se completato con successo.</summary>
    Task<bool> ExecuteAsync(PipelineContext context);
}
