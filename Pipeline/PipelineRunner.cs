namespace AdventurePdfForge.Pipeline;

/// <summary>
/// Esegue una sequenza ordinata di step, gestendo step opzionali e logging.
/// </summary>
public class PipelineRunner
{
    private readonly List<IPipelineStep> _steps = [];

    public PipelineRunner AddStep(IPipelineStep step)
    {
        _steps.Add(step);
        return this;
    }

    public async Task<PipelineContext> RunAsync(PipelineContext context)
    {
        Console.WriteLine($"▶ Pipeline avviata ({_steps.Count} step)");
        Console.WriteLine(new string('─', 50));

        foreach (var step in _steps)
        {
            var tag = step.IsOptional ? " [opzionale]" : "";
            Console.WriteLine($"  ⏳ {step.Name}{tag}...");

            try
            {
                var success = await step.ExecuteAsync(context);

                if (!success && !step.IsOptional)
                {
                    Console.WriteLine($"  ❌ {step.Name} fallito — pipeline interrotta.");
                    throw new InvalidOperationException(
                        $"Lo step obbligatorio '{step.Name}' è fallito.");
                }

                var icon = success ? "✅" : "⚠️ saltato";
                Console.WriteLine($"  {icon} {step.Name}");
            }
            catch (Exception ex) when (step.IsOptional)
            {
                Console.WriteLine($"  ⚠️ {step.Name} — errore ignorato: {ex.Message}");
            }
        }

        Console.WriteLine(new string('─', 50));
        Console.WriteLine("▶ Pipeline completata.");
        return context;
    }
}
