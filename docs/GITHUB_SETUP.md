# GitHub setup for AccessHub

## 1. Local repository (already done if you ran setup)

```powershell
cd d:\IAM
git init
git add .
git commit -m "Initial commit: AccessHub IAM portfolio (NET 8 + React)"
```

## 2. Create the GitHub repository

### Option A — GitHub website (recommended)

1. Sign in at [https://github.com](https://github.com)
2. Click **+** → **New repository**
3. Settings:
   - **Repository name:** `AccessHub` (or `accesshub-iam`)
   - **Description:** `Multi-tenant RBAC admin API (.NET 8) + React — IAM portfolio project`
   - **Public**
   - Do **not** add README, .gitignore, or license (they already exist locally)
4. Click **Create repository**

### Option B — GitHub CLI (after installing)

```powershell
winget install GitHub.cli
gh auth login
cd d:\IAM
gh repo create AccessHub --public --source=. --remote=origin --push
```

## 3. Connect remote and push

Repository URL for this project:

```powershell
cd d:\IAM
git branch -M main
git remote add origin https://github.com/Harshad1732/AccessHub.git
git push -u origin main
```

If you use SSH:

```powershell
git remote add origin git@github.com:Harshad1732/AccessHub.git
git push -u origin main
```

GitHub may prompt for login. Use a **Personal Access Token** as the password if using HTTPS ([create token](https://github.com/settings/tokens) with `repo` scope).

## 4. Polish the repo page

On GitHub → **Settings** → General:

- Add topics: `dotnet`, `aspnet-core`, `rbac`, `jwt`, `react`, `typescript`, `ef-core`, `portfolio`
- Pin the repository on your profile (optional)

In **About** (right sidebar on repo home):

- Website: leave blank or add demo URL later
- Description: same as above

## 5. README badge (optional)

Add to the top of `README.md` after pushing:

```markdown
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)
![React](https://img.shields.io/badge/React-18-61DAFB)
![License](https://img.shields.io/badge/license-MIT-green)
```

## 6. Put on resume / LinkedIn

```
https://github.com/Harshad1732/AccessHub
```

## Troubleshooting

| Issue | Fix |
|-------|-----|
| `remote origin already exists` | `git remote set-url origin <new-url>` |
| `failed to push` / auth | Use PAT or `gh auth login` |
| Large files rejected | Ensure `web/node_modules` is in `.gitignore` |
| Secrets committed by mistake | Rotate JWT key; use `git rm --cached` for sensitive files |

## Security reminder

- Never commit `web/.env` or production connection strings
- Change `Jwt:Key` in production; use GitHub Secrets for CI/CD later
