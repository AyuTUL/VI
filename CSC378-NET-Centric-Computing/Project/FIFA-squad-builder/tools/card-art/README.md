# Card Art Downloader

Downloads complete card images from direct FUTWIZ CDN URLs, saves them into the ASP.NET app, and updates `Players.CardImageUrl`.

This tool does not scrape the FUTWIZ squad builder page. FUTWIZ blocks automated page fetches with Cloudflare, and the squad builder page is an interactive app, not a stable image dataset.

The recommended path is the EA ratings image downloader. It uses the player `SourceId`, downloads complete public player shield PNGs from EA's ratings image CDN, saves them locally, and updates MySQL automatically.

## Setup

Install Node.js LTS, then run:

```powershell
cd "D:\Ayu\CSIT\VI\CSC378-NET-Centric-Computing\Project\tools\card-art"
npm.cmd install
```

PowerShell can block `npm.ps1`. Use `npm.cmd` on Windows.

## Run EA downloader

Download cards for the players currently used in squads:

```powershell
npm.cmd run download:ea:squad
```

Or double-click:

```text
tools\card-art\download-ea-squad.cmd
```

Download the top 250 players by overall:

```powershell
npm.cmd run download:ea
```

Or double-click:

```text
tools\card-art\download-ea-top250.cmd
```

Download specific players:

```powershell
node download-ea-cards.mjs --game FC25 --source-ids 231747,239085,158023
```

Download every imported player. This can take a long time and many EA players may not have a public shield for the selected game:

```powershell
npm.cmd run download:ea:all
```

Try another EA FC version if many cards are skipped:

```powershell
node download-ea-cards.mjs --game FC24 --top 250
node download-ea-cards.mjs --game FC26 --top 250
```

Try multiple versions automatically:

```powershell
node download-ea-cards.mjs --games FC25,FC24,FC26 --top 250
```

Images are saved to:

```text
src/FifaSquadBuilder/wwwroot/player-cards/ea/
```

Database values are updated to paths like:

```text
/player-cards/ea/fc25/231747-k-mbappe.png
```

Then restart the ASP.NET app and hard refresh the squad page.

## FUTWIZ direct URL mode

Create `cards.json` next to this file:

```json
[
  {
    "sourceId": 158023,
    "name": "L. Messi",
    "imageUrl": "https://cdn.futwiz.com/..."
  }
]
```

Use either `sourceId` or `name`. `sourceId` is safer because names can repeat.

## Run

```powershell
npm.cmd run download:futwiz
```

Images are saved to:

```text
src/FifaSquadBuilder/wwwroot/player-cards/
```

Database values are updated to paths like:

```text
/player-cards/158023-l-messi.png
```

Then restart the ASP.NET app and hard refresh the squad page.
