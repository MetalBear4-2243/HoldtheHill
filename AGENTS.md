# Hold the Hill — Agent Guidelines

Single-player 2D hybrid tower defense by **Turbulent Towers Studio**.
The primary source of truth for setup, folder structure, and architecture is [README.md](file:///c:/Users/metal/Documents/HoldtheHill/HoldtheHill/README.md).

---

## 1. Safety & Git Workflow Rules
- **Group Repository:** `MetalBear4-2243/HoldtheHill` is shared. Never commit, push, merge, or open auto-merge PRs directly into `Indev` or `main` without explicit approval.
- **Branches:**
  - `Indev`: Active integration & testing branch. Always branch from `Indev` and target PRs to `Indev`.
  - `main`: Stale/stable release branch. Do not target or branch from `main`.
  - Feature branches: `name/what-it-does` (e.g. `adam/enemy-spawner`).
- **Git Identity:** Commits from this machine must use the user's GitHub noreply address (`322642514+nilly-ctrl@users.noreply.github.com` or local git user.email). Do not expose private emails.

---

## 2. Mandatory AI Disclosure
- **Log every AI-assisted session:** Any code, docs, or research generated with AI assistance must have a 1-line entry added to [docs/AI_DEVLOG.md](file:///c:/Users/metal/Documents/HoldtheHill/HoldtheHill/docs/AI_DEVLOG.md) (Date, Who, Tool, What the AI helped with, What we decided, Not tested yet) in the same commit set.
- Commits must preserve `Co-Authored-By` metadata.

---

## 3. Unity Environment & Engine Constraints
- **Unity Version:** **`6000.6.0f1` strictly**. Never touch `ProjectSettings/ProjectVersion.txt`.
- **Unity Root:** The Unity project root is `unity/`, not the repository root.
- **Domain Reload:** Domain reload must remain **ON**. Do not change Enter Play Mode Settings.
- **Git LFS:** Art & audio assets (`.aseprite`, `.ase`, `.psd`, Blender, etc.) use Git LFS. Run `git lfs lock "<path>"` before modifying and `git lfs unlock "<path>"` after pushing.
- **Never commit:** `Library/`, `Logs/`, `Temp/`, `UserSettings/`, or generated `*.csproj`/`*.sln`.

---

## 4. Code Architecture & Conventions
- **Feature-first folders:** All gameplay systems live in `unity/Assets/_Game/Features/<Feature>/` (e.g., `Player`, `Enemies`, `Towers`, `Combat`).
- **Core Separation:** `unity/Assets/_Game/Core/` contains shared foundational systems and must **never** depend on `Features/`.
- **Namespaces & Naming:**
  - Namespaces must mirror folder structure (e.g., `HoldTheHill.Features.Towers`, `HoldTheHill.Core`).
  - PascalCase for all files and classes. File name must match class name exactly (`ArcherTower.cs` -> `class ArcherTower`).
- **Assemblies (`.asmdef`):**
  - Game code belongs to `HoldTheHill.Runtime`.
  - If using a new Unity/package dependency, add its assembly reference to `HoldTheHill.Runtime.asmdef`.
  - Editor scripts go in `HoldTheHill.Editor`, tests in `HoldTheHill.Tests.EditMode` / `.PlayMode`.
- **ScriptableObjects:** C# class goes in `Scripts/`, `.asset` instance data in `Data/`.
- **No `Resources/` folders:** Always reference assets directly or via ScriptableObjects.
- **Sandboxes:** Teammate test areas in `_Sandbox/` must not be modified. Only touch your own sandbox.
