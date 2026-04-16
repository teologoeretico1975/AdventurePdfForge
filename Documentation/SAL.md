# 📋 AdventurePdfForge — Stato Avanzamento Lavori

> Ultimo aggiornamento: Aprile 2026
> Branch: `main`

---

## 🧭 Descrizione

Tool .NET 10 per la generazione semi-automatica di PDF (micro-avventure D&D dark fantasy).
Dato un JSON strutturato, produce un PDF A4 con copertina e pagina interna tramite Playwright.
Le immagini vengono generate localmente tramite ComfyUI (SDXL) o cloud via OpenAI DALL-E 3.

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
│   ├── ImageProviders/
│   │   ├── IImageProvider.cs     # Interfaccia provider immagini
│   │   ├── ComfyUiImageProvider.cs  # Backend locale (ComfyUI + SDXL)
│   │   └── OpenAiImageProvider.cs   # Backend cloud (DALL-E 3)
│   └── Steps/
│       ├── GenerateAssetPromptsStep.cs   # Genera prompt AI per ogni asset
│       └── GenerateAssetsStep.cs         # Genera immagini via provider configurato
├── Templates/
│   ├── document-template.html    # Template HTML con placeholder
│   └── styles.css                # CSS per copertina + pagina interna + cornice CSS
├── Data/
│   └── ombre-nella-cripta.json   # JSON avventura di esempio
├── Assets/                       # Immagini (scene.png, parchment.png, introimage.png)
├── Output/                       # HTML e PDF generati
├── Documentation/
│   ├── Constitution.md           # Principi e architettura del progetto
│   ├── ComfyUI.md                # Guida installazione e configurazione ComfyUI
│   └── SAL.md                    # Questo file (stato avanzamento)
├── Program.cs                    # Entry point con flussi mutuamente esclusivi
└── AdventurePdfForge.csproj
```

---

## ⚙️ Stack Tecnologico

| Componente | Tecnologia |
|---|---|
| Runtime | .NET 10 / C# 14 |
| PDF rendering | Playwright (Chromium headless) |
| Image generation (locale) | ComfyUI 0.19.1 + Juggernaut XL v9 (SDXL) |
| Image generation (cloud) | OpenAI DALL-E 3 (pacchetto `OpenAI` 2.10.0) |
| GPU | NVIDIA GTX 1070 8 GB VRAM, PyTorch 2.11.0+cu126 |
| Template | HTML + CSS con placeholder `{{...}}` |

---

## ✅ Funzionalità Implementate

### Pipeline modulare
- `IPipelineStep` — interfaccia con `Name`, `IsOptional`, `ExecuteAsync`
- `PipelineContext` — contesto condiviso (Adventure, AssetPrompts, AssetPaths, ImageDpi, AssetFilter)
- `PipelineRunner` — esegue step in sequenza, gestisce step opzionali con logging emoji

### Provider immagini (IImageProvider)
- **`ComfyUiImageProvider`** (default) — generazione locale gratuita via ComfyUI API, checkpoint Juggernaut XL v9
- **`OpenAiImageProvider`** — generazione cloud via DALL-E 3 (richiede `OPENAI_API_KEY`)

### Step implementati
1. **`GenerateAssetPromptsStep`** — Genera prompt SDXL-optimized per 3 asset:
   - `scene.png` (copertina, digital painting dark fantasy)
   - `parchment.png` (texture pergamena)
   - `introimage.png` (immagine hero per pagina intro)
2. **`GenerateAssetsStep`** — Genera immagini via provider configurato, con scaling DPI e filtro per singolo asset

### Flussi mutuamente esclusivi
| Flag | Effetto |
|---|---|
| *(nessuno)* | Genera solo il PDF usando gli asset esistenti |
| `-buildassetprompt` | Esegue solo la generazione delle prompt e termina |
| `-buildasset` | Genera prompt + immagini e termina (no PDF) |
| `-buildasset -asset scene.png` | Rigenera solo l'asset specificato |
| `-dpi 150` | Scala le dimensioni per immagini più leggere |
| `-provider openai` | Usa DALL-E 3 invece di ComfyUI |

### Cornice pagina (CSS puro)
- Bordo gotico sottile realizzato interamente in CSS (doppia linea semi-trasparente)
- Eliminata la generazione AI della cornice (SDXL non riusciva a produrre bordi sottili)

### Generazione PDF (logica inline in Program.cs)
- Deserializza JSON → compone HTML da template → converte immagini in data URI base64
- CSS linkato tramite placeholder `{{CSS_PATH}}`
- Genera PDF A4 senza margini con Playwright
- **6 pagine**: copertina, intro, hook+twist, indizi dettagliati, struttura scena, fail forward

### Modello dati (`Adventure`)
- Proprietà base: `Title`, `Subtitle`, `IntroTitle`, `IntroText`, `SummaryBullets`, `ClueBullets`
- Hook/Twist: `Hook`, `Twist`
- Indizi dettagliati: `CluesDetailed` → `ClueDetail` (Title, Description, Interpretation, Risk)
- Struttura scena: `SceneStructure` → `SceneStructureData` (Entry, Distortion, Revelation, Climax)
- Fail forward: `FailForward` → `FailForwardData` (Failure, Consequence, Escalation)
- `FooterNote`

---

## 🔲 Step Futuri (non ancora implementati)

| Step | Descrizione |
|---|---|
| `BuildHtmlStep` | Migra la composizione HTML da Program.cs in uno step dedicato |
| `RenderPdfStep` | Migra la generazione PDF (Playwright) in uno step dedicato |
| Post-processing asset | Resize/crop immagini con ImageSharp |
| Agent orchestratore | Orchestrazione completa JSON → prompt → asset → PDF via Semantic Kernel o simile |

---

## 🐛 Problemi Noti / Risolti

| Problema | Stato | Note |
|---|---|---|
| `DirectoryNotFoundException` su Templates/ | ✅ Risolto | Aggiunto `CopyToOutputDirectory` nel .csproj |
| CSS non applicato nel PDF | ✅ Risolto | Aggiunto placeholder `{{CSS_PATH}}` con URL assoluto |
| ComfyUI workflow JSON serialization | ✅ Risolto | `PostAsJsonAsync` ri-serializzava gli array; usato `JsonObject` + `PostAsync` raw |
| PyTorch no CUDA su GTX 1070 | ✅ Risolto | Reinstallato torch con cu126 |
| Cornice AI troppo pesante | ✅ Risolto | Sostituita con bordo CSS puro |

---

## 📝 Convenzioni

- Le prompt per la generazione asset sono **sempre in inglese**
- I commenti nel codice seguono lo stile esistente (italiano per UI/log, inglese per XML doc)
- La pipeline è progettata per essere estensibile: ogni nuovo step implementa `IPipelineStep`
