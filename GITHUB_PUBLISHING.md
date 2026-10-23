# SBPS — GitHub Publication Guide

This file records the intended public repository metadata and the checks that must be completed before publishing.

## Repository

- Owner: `AhmetYILDIRIMuyum`
- Name: `SBPS`
- Visibility: Public
- Intended URL: <https://github.com/AhmetYILDIRIMuyum/SBPS>
- Description: **Smart Batch Printing Studio — A Windows desktop application for organizing and batch-printing supported files.**
- Homepage: Leave empty unless a real product website is available.

## Suggested Topics

`csharp`, `dotnet`, `winforms`, `windows-desktop`, `printing`, `batch-printing`, `print-management`, `pdf-printing`, `webview2`, `desktop-application`

## Required Pre-publication Checks

1. Confirm GitHub authentication belongs to `AhmetYILDIRIMuyum`.
2. Confirm the `SBPS` repository does not already contain work that could be overwritten.
3. Review every tracked file and Git history for credentials, private infrastructure, customer data, company-owned content, and license keys.
4. Include only source and documentation; exclude `.vs/`, `bin/`, `obj/`, `publish/`, logs, user settings, and local development prompts.
5. Complete Debug and Release builds.
6. Perform a non-printing startup check. Never send a physical print job without explicit operator approval.
7. Verify README rendering, visibility, default branch, description, and topics after push.

## Licensing Notice

Public visibility is not an open-source license. Preserve the repository's proprietary `LICENSE` file and direct licensing or commercial-use requests to:

**Ahmet YILDIRIM [dede]**  
[ahmetyildirimuyum@gmail.com](mailto:ahmetyildirimuyum@gmail.com)

Do not create a release, version tag, or passing-build badge unless the corresponding artifact or workflow has actually been created and verified.

## Observed Remote State — 2026-10-23

The public repository already exists and must not be overwritten:

<https://github.com/AhmetYILDIRIMuyum/SBPS>

- Visibility: public
- Default branch: `main`
- Description is set; topics are empty; homepage is empty
- The published tree has source and docs, and does not include `bin/`, `obj/`, or `.vs/`
- The published tree has no `.gitignore`
- `src/SBPS/SBPS.csproj.user` is published. The local copy contains only designer subtype entries, not credentials
- `23102026_14_04_gelistirme.md` is published. Local `.gitignore` now excludes that working note
- Later local changes — 1368 × 882 startup size, the Exit button, and the sectioned help window — are not in that published commit

Push, force-update, or topic changes need a separate approval. `gh` is not installed on this machine, and this folder is not a Git repository.
