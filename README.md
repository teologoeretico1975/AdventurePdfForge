# 🛠️ PDFForge

**PDFForge** è un tool .NET 10 per la generazione semi-automatica di PDF vendibili — micro-avventure D&D dark fantasy.

## 🎯 Obiettivo

Creare una pipeline ripetibile per produrre contenuti:

- coerenti e giocabili
- di qualità minima vendibile
- rapidamente iterabili

> L'obiettivo NON è generare PDF perfetti, ma costruire una **macchina industriale** per micro-avventure.

---

## ⚙️ Stack Tecnologico

| Tecnologia | Ruolo |
|---|---|
| .NET 10 / C# | Core applicativo |
| Playwright 1.59 | Rendering HTML → PDF |
| HTML / CSS | Template di layout |
| ChatGPT | Generazione contenuti narrativi |

---

## 📁 Struttura del Progetto

```text
PdfForge/
├── Assets/                             # Immagini (scene, parchment, frame)
├── Data/
│   └── ombre-nella-cripta.json         # Input JSON (metadati + contenuto)
├── Templates/
│   ├── document-template.html          # Template HTML con placeholder
│   └── styles.css                      # Stili del documento
├── Output/                             # PDF e HTML generati
├── Documentation/
│   └── Constitution.md                 # Documentazione di progetto
├── Program.cs                          # Entry point
├── AdventurePdfForge.csproj            # Progetto .NET 10
├── README.md
└── CHANGELOG.md
```

---

## 🚀 Quick Start

### Prerequisiti

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Playwright (installato automaticamente al primo avvio)

### Esecuzione

```bash
dotnet run
```

Il PDF verrà generato nella cartella `Output/`.

### Workflow tipico

1. Genera il JSON del contenuto (via ChatGPT o manualmente) e salvalo in `Data/`
2. Genera o inserisci le immagini in `Assets/`
3. Esegui `dotnet run`
4. Ritira il PDF da `Output/`

⏱️ Tempo medio per avventura: **~5–10 minuti**

---

## 🔄 Come Funziona

1. **Input JSON** — Metadati e contenuto narrativo strutturato
2. **Template Engine** — HTML + CSS con placeholder sostituiti a runtime
3. **Renderer** — Playwright converte l'HTML in PDF A4
4. **Output** — PDF pronto per la vendita

---

## 📊 Stato del Progetto

**MVP Tecnico completato**

| Area | Stato |
|---|---|
| Pipeline PDF | ✅ Funzionante |
| Contenuto | 🟡 Da migliorare |
| Scalabilità | 🔴 Da implementare |

---

## 🗺️ Roadmap

- [ ] Template definitivo multi-page
- [ ] Interfaccia CLI
- [ ] Asset library centralizzata
- [ ] Supporto multi-avventura in batch

---

## 🧩 Visione

Macchina per generare micro-avventure vendibili in modo industriale.

---

## 📄 Licenza

Tutti i diritti riservati.
