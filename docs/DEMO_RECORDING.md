# Demo GIF recording

## Automated (recommended)

From the repo root:

```powershell
.\scripts\record-demo.ps1
```

This script:

1. Starts the API and React dev server
2. Records a ~30s walkthrough (admin → audit → viewer read-only)
3. Saves **`docs/demo/accesshub-demo.gif`**

Requirements: Node.js, .NET 8, SQL Server LocalDB (same as running the app).

First run installs Playwright Chromium:

```powershell
cd scripts
npx playwright install chromium
```

## Manual (Xbox Game Bar)

1. Run API + Web (see README).
2. Press **Win + G** → Record.
3. Follow this script (~45 seconds):

| Time | Action |
|------|--------|
| 0:00 | Show login page |
| 0:05 | Sign in as `admin@acme.local` / `Admin123!` |
| 0:12 | Open **Roles**, then **Invoices**, then **Audit Log** |
| 0:25 | Logout |
| 0:28 | Sign in as `viewer@acme.local` / `Viewer123!` |
| 0:35 | Open **Invoices** (note: no New Invoice button) |
| 0:42 | Stop recording |

Convert MP4 → GIF with [ezgif.com](https://ezgif.com/video-to-gif) or install ffmpeg:

```powershell
winget install Gyan.FFmpeg
ffmpeg -i demo.mp4 -vf "fps=12,scale=960:-1" -loop 0 docs/demo/accesshub-demo.gif
```

## Embed in README

Already wired when `docs/demo/accesshub-demo.gif` exists:

```markdown
## Demo

![AccessHub demo](docs/demo/accesshub-demo.gif)
```

Commit and push:

```powershell
git add docs/demo/accesshub-demo.gif README.md
git commit -m "Add demo GIF for portfolio README"
git push
```
