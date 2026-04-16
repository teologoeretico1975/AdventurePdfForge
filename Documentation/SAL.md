# 📋 AdventurePdfForge — Stato Avanzamento Lavori

> Ultimo aggiornamento: Luglio 2025
> Branch: `main`

---

## 🧭 Descrizione

Tool .NET 10 per la generazione semi-automatica di PDF (micro-avventure D&D dark fantasy).
Dato un JSON strutturato, produce un PDF A4 con copertina e pagina interna tramite Playwright.

---

## 🏗️ Struttura Progetto

```
PdfForge/
├── Models/
│   └── Adventure.cs              # Modello dati avventura
├── Pipeline/
│   ├── IPipelineStep.cs          # Interfaccia step
│   ├── PipelineContext.cs        # Contesto condiviso tra step
│   ├── PipelineRunner.cs         # Runner con logging e gestione step opzionali
│   └── Steps/
│       ├── GenerateAssetPromptsStep.cs   # Genera prompt AI per ogni asset
│       └── GenerateAssetsStep.cs         # Chiama DALL-E 3 per generare immagini
├── Templates/
│   ├── document-template.html    # Template HTML con placeholder
│   └── styles.css                # CSS per copertina + pagina interna
├── Data/
│   └── ombre-nella-cripta.json   # JSON avventura di esempio
├── Assets/                       # Immagini (scene.png, parchment.png, frame.png)
├── Output/                       # HTML e PDF generati
├── Documentation/
│   ├── Constitution.md           # Principi e architettura del progetto
│   └── SAL.md                    # Questo file (stato avanzamento)
├── Program.cs                    # Entry point con pipeline + generazione PDF
└── AdventurePdfForge.csproj
```

---

## ⚙️ Stack Tecnologico

| Componente | Tecnologia |
|---|---|
| Runtime | .NET 10 / C# 14 |
| PDF rendering | Playwright (Chromium headless) |
| Image generation | OpenAI DALL-E 3 (pacchetto `OpenAI` 2.10.0) |
| Template | HTML + CSS con placeholder `{{...}}` |

---

## ✅ Funzionalità Implementate

### Pipeline modulare
- `IPipelineStep` — interfaccia con `Name`, `IsOptional`, `ExecuteAsync`
- `PipelineContext` — contesto condiviso (Adventure, AssetPrompts, AssetPaths, FinalHtml, PdfPath)
- `PipelineRunner` — esegue step in sequenza, gestisce step opzionali con logging emoji

### Step implementati
1. **`GenerateAssetPromptsStep`** — Legge il JSON, estrae tema/mood, genera prompt in inglese per 3 asset:
   - `scene.png` (copertina, digital painting dark fantasy)
   - `parchment.png` (texture pergamena)
   - `frame.png` (cornice gotica sottile con centro trasparente)
2. **`GenerateAssetsStep`** — Chiama DALL-E 3 via API OpenAI, salva le immagini in `Assets/`. Richiede `OPENAI_API_KEY` come variabile d'ambiente.

### CLI arguments
| Flag | Effetto |
|---|---|
| *(nessuno)* | Genera solo il PDF usando gli asset esistenti |
| `-buildassetprompt` | Esegue solo la generazione delle prompt (senza chiamare DALL-E) |
| `-buildasset` | Genera prompt + chiama DALL-E + salva immagini in Assets/ |

### Generazione PDF (logica inline in Program.cs)
- Deserializza JSON → compone HTML da template → converte immagini in data URI base64
- CSS linkato tramite placeholder `{{CSS_PATH}}` (risolto come `file:///` URL assoluto)
- Genera PDF A4 senza margini con Playwright

---

## 🔲 Step Futuri (non ancora implementati)

| Step | Descrizione |
|---|---|
| `BuildHtmlStep` | Migra la composizione HTML da Program.cs in uno step dedicato |
| `RenderPdfStep` | Migra la generazione PDF (Playwright) in uno step dedicato |
| Gestione multi-pagina | Supporto per avventure con più sezioni/pagine |
| Post-processing asset | Resize/crop immagini con ImageSharp, rimozione sfondo per frame.png |
| Agent orchestratore | Orchestrazione completa JSON → prompt → asset → PDF via Semantic Kernel o simile |

---

## 🐛 Problemi Noti / Risolti

| Problema | Stato | Note |
|---|---|---|
| `DirectoryNotFoundException` su Templates/ | ✅ Risolto | Aggiunto `CopyToOutputDirectory` nel .csproj |
| CSS non applicato nel PDF | ✅ Risolto | Il template usava `href="styles.css"` relativo; aggiunto placeholder `{{CSS_PATH}}` con URL assoluto |
| Cornice troppo interna e trasparente | ✅ Risolto | Cambiato `inset: 10mm` → `inset: 0`, `opacity: 0.22` → `0.45`, `object-fit: fill` |
| frame.png non ideale come asset | ⚠️ Aperto | Serve immagine con centro trasparente (alpha); DALL-E non genera bene la trasparenza |

---

## 📝 Convenzioni

- Le prompt per la generazione asset sono **sempre in inglese**
- I commenti nel codice seguono lo stile esistente (italiano per UI/log, inglese per XML doc)
- La pipeline è progettata per essere estensibile: ogni nuovo step implementa `IPipelineStep`
