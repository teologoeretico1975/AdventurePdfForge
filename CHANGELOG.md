# 📝 Changelog

Tutte le modifiche rilevanti al progetto sono documentate in questo file.

Il formato è basato su [Keep a Changelog](https://keepachangelog.com/it-IT/1.1.0/).

---

## [0.4.0] - 2026-04-16

### Aggiunto

- **Supporto multi-pagina** — il PDF ora genera 6 pagine complete:
  - Pagina 1: Copertina
  - Pagina 2: Intro + riassunto + indizi
  - Pagina 3: Hook + Twist + nota d'uso
  - Pagina 4: Indizi dettagliati (con descrizione, interpretazione, rischio)
  - Pagina 5: Struttura della scena (Ingresso → Distorsione → Rivelazione → Climax)
  - Pagina 6: Fail forward (fallimento, conseguenza, escalation) + footer
- **Modello `Adventure` esteso** con `Hook`, `Twist`, `CluesDetailed`, `SceneStructure`, `FailForward`, `FooterNote`
- **Classi dati** `ClueDetail`, `SceneStructureData`, `FailForwardData`
- **Nuovi stili CSS**: `.full-box`, `.large-box`, `.note-box`, `.timeline-box`, `.timeline-step`, `.footer-note`
- Tutti i nuovi placeholder nel template HTML e in Program.cs

### Corretto

- **Template HTML pagine 3-6** — uniformate a pagina 2: `{{BACKGROUND}}` invece di `{{BACKGROUND_IMAGE}}`, cornice CSS `<div>` invece di `<img>`

### Rimosso

- Proprietà legacy `IntroImage`, `BackgroundImage`, `FrameImage` dal modello `Adventure`

---

## [0.3.0] - 2026-04-16

### Aggiunto

- **ComfyUI image provider** — generazione immagini locale gratuita via ComfyUI API + SDXL
- **Juggernaut XL v9** come checkpoint di default (sostituisce SDXL 1.0 base)
- **Flussi mutuamente esclusivi** — `-buildasset`/`-buildassetprompt` genera solo asset e termina; senza flag genera solo PDF
- **Parametro `-dpi`** — scala le dimensioni delle immagini (default 300, 150 per file leggeri)
- **Parametro `-asset`** — rigenera un singolo asset specifico (es. `-asset scene.png`)
- **Parametro `-provider`** — seleziona backend (`comfyui` o `openai`)
- **Asset `introimage.png`** — quarto asset per l'immagine hero della pagina intro
- Placeholder separati `{{SCENE_IMAGE}}` (copertina) e `{{INTRO_IMAGE}}` (pagina intro)
- Documentazione **ComfyUI.md** con guida installazione, comandi e troubleshooting
- Prompt SDXL-optimized (keyword-based, quality boosters, negative prompt esteso)
- Log del prompt generato per ogni asset durante la generazione

### Corretto

- **Cornice sostituita con CSS puro** — SDXL non produceva bordi sottili; ora doppia linea CSS semi-trasparente
- **Serializzazione workflow ComfyUI** — `PostAsJsonAsync` ri-serializzava array; usato `JsonObject` + `PostAsync`

### Rimosso

- Asset `frame.png` — la cornice decorativa è ora interamente CSS

### Modificato

- `Program.cs` refactored con flussi mutuamente esclusivi (asset vs PDF)
- `PipelineContext` esteso con `ImageDpi`, `AssetFilter`, `ImageProvider`
- `GenerateAssetsStep` supporta scaling DPI e filtro per singolo asset
- Prompt migliorati per qualità SDXL (scene, parchment, intro)

---

## [0.2.0] - 2025-07-15

### Aggiunto

- **Pipeline modulare** con `IPipelineStep`, `PipelineContext`, `PipelineRunner`
- **`GenerateAssetPromptsStep`** — genera prompt AI in inglese per ogni asset (scene, parchment, frame)
- **`GenerateAssetsStep`** — chiama DALL-E 3 via API OpenAI per generare immagini
- **CLI arguments**: `-buildassetprompt` e `-buildasset` per controllare l'esecuzione degli step
- Modello `Adventure` estratto in `Models/Adventure.cs`
- Documento **SAL.md** (Stato Avanzamento Lavori) in `Documentation/`
- Pacchetto NuGet `OpenAI` 2.10.0

### Corretto

- **CSS non applicato nel PDF** — aggiunto placeholder `{{CSS_PATH}}` nel template, sostituito con URL assoluto `file:///`
- **`DirectoryNotFoundException`** su Templates/ — aggiunto `CopyToOutputDirectory` nel .csproj
- **Cornice pagina 2** — `inset: 0`, `opacity: 0.45`, `object-fit: fill` per coprire i bordi

### Modificato

- `Program.cs` refactored per usare la pipeline con step condizionali
- `document-template.html` usa `{{CSS_PATH}}` invece di `href="styles.css"` hardcoded

---

## [0.1.0] - 2026-04-16

### Aggiunto

- Pipeline PDF funzionante con Playwright (.NET 10)
- Template HTML/CSS A4 con placeholder
- Sistema di input basato su JSON (metadati + contenuto narrativo)
- Asset system per scene, parchment e frame
- Generazione automatica HTML → PDF
- Documentazione di progetto (`Constitution.md`)
- `README.md` e `CHANGELOG.md`

### Limitazioni note

- Contenuto narrativo ancora troppo generico
- Asset pipeline manuale
- Template fragile su variazioni di contenuto
- Nessuna interfaccia CLI
- Nessun supporto multi-page