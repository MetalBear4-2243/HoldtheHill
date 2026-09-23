# Hold the Hill (team repo)

Single-player 2D hybrid tower defense by Turbulent Towers Studio. **`README.md` is the team's source of truth** for setup, folder layout, naming, scenes, assemblies and workflow; read it before changing anything and follow it exactly.

## Repo rules for Claude
- GitHub `MetalBear4-2243/HoldtheHill` is a **group repo**. Never commit, push, merge, or open auto-merge PRs into `main` without the user's explicit approval for that specific change.
- The user's working branch is `BryceTest`. Team convention for new work is `name/what-it-does` branches with PRs into `main`.
- AI-assisted changes to this repo get a one-line row in `docs/AI_DEVLOG.md` (Date, Who, Tool, What the AI helped with, What we decided, Not tested yet) in the same commit set, and commits keep their `Co-Authored-By` line.
- Unity project root is the `Hold the Hill/` subfolder (with a space), not the repo root. Quote paths.
- Unity **6000.6.0f1** exactly. Don't touch `ProjectSettings/ProjectVersion.txt`.
- Git LFS stores art and audio. `.aseprite`/`.ase`, `.psd`, Krita, Affinity and Blender files are lockable: `git lfs lock "<path>"` before editing, and `git lfs unlock` after pushing.
- Don't commit `Library/`, `Logs/`, `Temp/`, `UserSettings/`, or generated `*.csproj`/`*.sln`/`*.slnx`.

## Where code goes (summary of README)
- Game code: `Hold the Hill/Assets/_Game/` → `Core/` (shared systems; never depends on Features), `Features/<System>/` (Player, GameFlow, Enemies, Towers, Combat, Economy, Progression, Levels, UI, DebugTools), `Shared/` (assets used by 2+ features), `Scenes/`, `Editor/`, `Tests/`.
- Personal experiments: `_Sandbox/<github-username>/` only. The user's sandbox is `_Sandbox/nilly-ctrl/`; never edit teammates' sandboxes.
- Assemblies: `HoldTheHill.Runtime` (`_Game/`), `HoldTheHill.Editor`, `HoldTheHill.Tests.EditMode` / `.PlayMode`. A new package needs adding to `HoldTheHill.Runtime.asmdef` references.
- Namespaces follow folders (`HoldTheHill.Features.Towers`). PascalCase files, file name = class name. ScriptableObject scripts in `Scripts/`, `.asset` data in `Data/`. Tiers are prefab variants. No `Resources/` folders.
- Levels are additive scene pairs: `Levels/<Level>/<Level>_Terrain` (tilemaps) + `<Level>_Gameplay`; `SceneLoader` in `Scenes/Bootstrap` loads the rest on Play.
- Domain reload stays **on** (don't change Enter Play Mode Settings).

## Related assets
Ant tower art (28 classes, 10 themes, animations) lives outside the repo in `/Users/nilly/GameDev/Assets/Ants/`. Anything imported here must follow the feature-folder layout, PascalCase naming, and LFS locking.
