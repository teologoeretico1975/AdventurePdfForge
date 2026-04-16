# ComfyUI — Guida di configurazione

## Panoramica

[ComfyUI](https://github.com/comfyanonymous/ComfyUI) è il backend locale gratuito per la generazione delle immagini.  
Il progetto lo utilizza tramite le API REST esposte sulla porta **8188**.

## Requisiti

| Componente | Versione | Note |
|---|---|---|
| Python | 3.14+ | Installato system-wide |
| PyTorch | 2.11.0+cu126 | CUDA 12.6 per GPU NVIDIA |
| GPU | GTX 1070 8 GB VRAM (minimo) | SDXL richiede ~6-7 GB VRAM |
| ComfyUI | 0.19.1+ | Installato in `C:\Users\teolo\ComfyUI` |

## Installazione

```powershell
# Clona ComfyUI
git clone https://github.com/comfyanonymous/ComfyUI.git C:\Users\teolo\ComfyUI
cd C:\Users\teolo\ComfyUI

# Installa dipendenze
pip install -r requirements.txt

# Installa PyTorch con CUDA 12.6 (necessario per GTX 1070 / CC 6.1)
pip install torch torchvision torchaudio --index-url https://download.pytorch.org/whl/cu126
```

## Checkpoint (modello)

Il checkpoint attivo è **Juggernaut XL v9** (~6.6 GB), scaricato da HuggingFace:

```powershell
# Download
Start-BitsTransfer `
  -Source "https://huggingface.co/RunDiffusion/Juggernaut-XL-v9/resolve/main/Juggernaut-XL_v9_RunDiffusionPhoto_v2.safetensors" `
  -Destination "C:\Users\teolo\ComfyUI\models\checkpoints\Juggernaut-XL_v9.safetensors"
```

### Alternative valide

| Checkpoint | Qualità | Stile | Link |
|---|---|---|---|
| **Juggernaut XL v9** (default) | ⭐⭐⭐⭐ | Realistico / Fantasy | [HuggingFace](https://huggingface.co/RunDiffusion/Juggernaut-XL-v9) |
| DreamShaper XL | ⭐⭐⭐⭐ | Illustrativo | [CivitAI](https://civitai.com/models/112902) |
| SDXL 1.0 Base | ⭐⭐⭐ | Generico | [HuggingFace](https://huggingface.co/stabilityai/stable-diffusion-xl-base-1.0) |

I file `.safetensors` vanno posizionati in:

```
C:\Users\teolo\ComfyUI\models\checkpoints\
```

## Avvio e arresto

### Avvio (con UI web abilitata)

```powershell
Start-Process python -ArgumentList "C:\Users\teolo\ComfyUI\main.py --listen 127.0.0.1 --enable-cors-header" -WorkingDirectory "C:\Users\teolo\ComfyUI"
```

### Arresto

```powershell
Get-NetTCPConnection -LocalPort 8188 -ErrorAction SilentlyContinue | ForEach-Object { Stop-Process -Id $_.OwningProcess -Force }
```

### Verifica stato

```powershell
Invoke-RestMethod -Uri "http://127.0.0.1:8188/system_stats" -TimeoutSec 5
```

## Interfaccia web

Dopo l'avvio con `--enable-cors-header`, la UI è disponibile su:

```
http://127.0.0.1:8188
```

> **Nota:** Senza il flag `--enable-cors-header`, la UI restituisce HTTP 403 (accesso negato).  
> Le API REST funzionano comunque — il programma non necessita della UI.

## Integrazione con PdfForge

### Flusso di generazione

```
Program.cs  →  GenerateAssetsStep  →  ComfyUiImageProvider  →  ComfyUI API
                                            │
                                            ├─ POST /prompt     (invia workflow)
                                            ├─ GET  /history/{id} (polling risultato)
                                            └─ GET  /view        (download immagine)
```

### Workflow interno

Il provider costruisce un workflow SDXL con i seguenti nodi:

| Nodo | Tipo | Funzione |
|---|---|---|
| 1 | CheckpointLoaderSimple | Carica il modello |
| 4 | EmptyLatentImage | Dimensioni immagine (scalate per DPI) |
| 5 | CLIPTextEncode | Prompt positivo |
| 6 | CLIPTextEncode | Prompt negativo |
| 3 | KSampler | Generazione (25 step, euler_ancestral, cfg 7.0) |
| 7 | VAEDecode | Decodifica latent → pixel |
| 9 | SaveImage | Salvataggio output |

### Parametri CLI

```powershell
# Genera asset con ComfyUI (default) a 300 DPI
dotnet run -- -buildasset -dpi 300

# Genera asset a DPI ridotti (immagini più leggere, ~4x più piccole)
dotnet run -- -buildasset -dpi 150

# Usa provider specifico
dotnet run -- -buildasset -provider comfyui -dpi 300
```

### Dimensioni generate (per asset)

| DPI | Larghezza | Altezza | Peso stimato |
|---|---|---|---|
| 300 (default) | 1024 | 1792 | ~3 MB |
| 150 | 512 | 896 | ~700 KB |
| 72 | 248 | 432 | ~200 KB |

## Troubleshooting

| Problema | Causa | Soluzione |
|---|---|---|
| `ComfyUI not reachable` | Server non avviato | Avviare con il comando di avvio |
| HTTP 403 nel browser | UI web disabilitata | Riavviare con `--enable-cors-header` |
| `CUDA out of memory` | VRAM insufficiente | Ridurre DPI (`-dpi 150`) o chiudere altre app GPU |
| Generazione lenta (~5 min/immagine) | GTX 1070 limitata | Normale per SDXL su 8 GB VRAM |
| Qualità bassa | Checkpoint base | Usare Juggernaut XL v9 (default attuale) |
| PyTorch no CUDA | Versione torch errata | Reinstallare con `--index-url .../cu126` |
