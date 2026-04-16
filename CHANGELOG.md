# 📝 Changelog

Tutte le modifiche rilevanti al progetto sono documentate in questo file.

Il formato è basato su [Keep a Changelog](https://keepachangelog.com/it-IT/1.1.0/).

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