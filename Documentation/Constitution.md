# 📘 PDFForge — Checkpoint Progetto

## 🧭 Descrizione del Progetto

PDFForge è un tool .NET per la generazione semi-automatica di PDF vendibili (micro-avventure D&D dark fantasy), progettato per bilanciare:

- automazione (pipeline)
- qualità minima vendibile
- velocità di produzione

L'obiettivo NON è generare PDF perfetti, ma creare una macchina ripetibile che produca contenuti:

- coerenti
- giocabili
- rapidamente iterabili

---

## 🏗️ Architettura

### 🔹 Struttura generale

PdfForge/
├── Assets/
├── Templates/
├── Data/
├── Output/
└── Program.cs

---

### 🔹 Componenti principali

#### 1. Input (JSON)
Contiene:
- metadati
- contenuto narrativo
- elementi strutturali

#### 2. Template Engine
HTML + CSS con placeholder

#### 3. Renderer
.NET + Playwright → HTML → PDF

#### 4. Asset System
Scene, parchment, frame

---

## ⚙️ Stack Tecnologico

- .NET 10
- C#
- Playwright
- HTML/CSS
- ChatGPT (contenuti)

---

## 🔄 Workflow Operativo

1. Generazione JSON
2. Generazione immagine
3. Inserimento asset
4. dotnet run
5. Output PDF

Tempo medio: ~5–10 minuti

---

## 🎯 Obiettivi

- Pipeline veloce
- PDF vendibili
- Scalabilità

---

## ✅ Obiettivi Raggiunti

- Pipeline PDF funzionante
- Playwright integrato
- Template A4
- Automazione base

---

## ⚠️ Open Point

- Contenuto troppo generico
- Asset pipeline manuale
- Template fragile
- Mancanza CLI
- No multi-page

---

## 📊 Stato

MVP Tecnico completato

- Pipeline: ✅
- Contenuto: 🟡
- Scalabilità: 🔴

---

## 🧠 Linee Guida

- contenuto > estetica
- velocità > perfezione
- sistema > singolo output

---

## 🚀 Prossimi Step

1. Template definitivo
2. CLI
3. Multi-page
4. Asset library

---

## 🧩 Visione

Macchina per generare micro-avventure vendibili in modo industriale
