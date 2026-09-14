# Hold the Hill — AI Devlog

A record of every change made with AI help: what was asked, what the AI did, what people decided, and how the result was checked. **Newest entries at the top.**

## Why we keep this

- **Transparency:** anyone (teammates, instructors) can see which work was AI-assisted.
- **Accountability:** a person asked for and approved every change; the AI doesn't decide game design.
- **Trust but verify:** each entry lists what was actually tested and what wasn't, so nobody assumes AI output works.

## Rules

1. **Log every AI-assisted commit or change**, even small ones.
2. **Name the tool and model** (e.g. Claude Code, Claude Opus 5).
3. **Separate human decisions from AI work.** People choose; the AI proposes and implements.
4. **List how it was verified** and **what is still untested**.
5. **Link the commits** so the change can be reviewed or reverted.
6. AI-made commits also carry a `Co-Authored-By` line naming the AI.

## Entry template

Copy this to the top of the log:

```markdown
## YYYY-MM-DD — Short title
**Requested by:** name / GitHub username
**Tool / model:** e.g. Claude Code — Claude Opus 5
**Branch:** branch-name

**What was asked**
- The request, in plain words

**Human decisions**
- Choices the person made (the AI can suggest options, but these are the team's calls)

**What the AI did**
- Changes made, research done

**Verification**
- How it was checked (compiled, played, tested, reviewed)

**Not verified / risks**
- Anything untested or uncertain

**Commits**
- `hash` message
```

---

## 2026-09-13 — Project setup and folder structure
**Requested by:** Nilly (nilly-ctrl)
**Tool / model:** Claude Code (desktop app) — Claude Opus 5, including one AI research sub-agent for web research
**Branch:** `BryceTest` — nothing was committed to `main`

### Part 1: Git setup and team conventions

**What was asked**
- Add a Unity-friendly Git setup to the group repo, then recommend and apply changes that make 3-person development easier.

**Human decisions**
- Apply changes to the `BryceTest` branch, not `main`.
- Apply all recommended changes, including art file locking.
- Company name: **Turbulent Towers Studio**.
- Standing rule: the AI never commits to `main` in group repos unless explicitly told to.

**What the AI did**
- `.gitattributes`: Git LFS for images, audio, 3D, fonts, and binaries; Unity SmartMerge for scenes/prefabs/assets; normalized line endings; moved the existing template PNG into LFS.
- `.gitignore`: fixed rules that only matched the repo root so they also match `Hold the Hill/`; stopped tracking the auto-generated `.slnx`.
- Made `.aseprite`, `.psd`, `.kra`, `.afphoto`, `.blend` lockable in LFS.
- Added `.editorconfig` for consistent C# formatting across Rider and Visual Studio.
- Removed the 2D template tutorial, Unity Version Control (`com.unity.collab-proxy`), and Visual Scripting after checking nothing referenced them.
- Created the first `_Game/` and per-person `_Sandbox/<github-username>/` folders; set the company name; wrote the README.

**Verification**
- Searched the project for references to every deleted file and for any Visual Scripting use before removing it.
- Ran Unity 6000.6.0f1 in batch mode after the package removal and after the folder changes: exited successfully, no compile errors; `packages-lock.json` only lost the removed packages and their unused dependencies.
- Confirmed Git LFS uploaded the converted PNG, and that the pushed branch matches the local branch.

**Not verified / risks**
- The **Windows** UnityYAMLMerge path in the README follows Unity's standard install location but wasn't tested on Windows.
- `.editorconfig` naming rules weren't checked inside Rider or Visual Studio.
- LFS locking makes art files read-only until locked; teammates need to know this (it's in the README).

**Commits**
- `038bfb3` Add Unity Git setup: LFS, scene merging, subfolder ignores
- `fc4791c` Add .editorconfig for consistent C# formatting
- `f6c4b5c` Make source art files lockable in Git LFS
- `c9abd1a` Remove template tutorial, Unity Version Control, and Visual Scripting
- `db5b2b9` Add shared _Game and per-person _Sandbox folder layout
- `3d655af` Set company name to Turbulent Towers Studio
- `f0698c7` Add README with setup, folder layout, and team workflow

### Part 2: Folder structure research and restructure

**What was asked**
- Research the recommended Unity folder/file layout for streamlined development and apply it to `BryceTest`, asking questions as needed.

**Human decisions** (answers to the AI's questions)
- Single-player, hybrid tower defense (player character + towers), semester-long project.
- Everyone on the team does code, art, and level building.
- Planned systems: player, enemies & waves, towers/buildings, power-ups, harvestables, upgrade tree, UI/menus/save, plus the AI's suggested core loop, combat depth, tech foundations, and polish/tools systems.
- Keep Aseprite files inside Unity.
- Use the feature-based layout **with small features merged**.
- Split levels into **additive scenes**.
- Turn **domain reload on** (the AI recommended keeping it off; the team chose the safer option).
- Set up **assembly definitions** now.

**What the AI did**
- Research (by an AI sub-agent reading web sources) — main sources:
  - Unity, *Organizing your project*: https://unity.com/how-to/organizing-your-project
  - Unity blog, *Author scenes and prefabs with version control*: https://unity.com/blog/author-scenes-and-prefabs-with-verson-control
  - Unity Manual, special folders: https://docs.unity3d.com/6000.0/Documentation/Manual/SpecialFolders.html
  - Unity Manual, assembly definitions: https://docs.unity3d.com/6000.6/Documentation/Manual/assembly-definitions-intro.html
  - Unity Manual, Enter Play Mode settings (6000.6): https://docs.unity3d.com/6000.6/Documentation/Manual/configurable-enter-play-mode.html
  - Community guides: https://gamedevbeginner.com/how-to-structure-your-unity-project-best-practice-tips/ and https://www.anchorpoint.app/blog/unity-folder-structure
- Restructured `_Game/` into `Core/`, `Features/` (Player, GameFlow, Enemies, Towers, Combat, Economy, Progression, Levels, UI, DebugTools), `Shared/`, `Scenes/`, `Editor/`, `Tests/`; added `ThirdParty/`.
- Added assembly definitions: `HoldTheHill.Runtime` (with Input System, URP, 2D Animation, SpriteShape, Tilemap Extras, TextMeshPro, UI, Timeline references), `HoldTheHill.Editor`, `HoldTheHill.Tests.EditMode`, `HoldTheHill.Tests.PlayMode`.
- Wrote `Core/Bootstrap/SceneLoader.cs`, which loads the Bootstrap, UI, and level scenes additively when Play is pressed in any scene.
- Created `Bootstrap`, `UI`, and `Hill01_Terrain` scenes with a temporary editor script (deleted, not committed); renamed `SampleScene` to `Hill01_Gameplay`; set the build scene list.
- Turned domain reload back on; tracked `.ase` files in LFS; rewrote the README.

**Verification**
- Checked exact package assembly names against the installed packages before referencing them.
- Unity batch-mode runs: scene setup succeeded (4 scenes in build settings); final run without the temporary script compiled `HoldTheHill.Runtime` with **no errors**. The only warnings: the Editor and Tests assemblies have no scripts yet (expected).
- Confirmed the Bootstrap scene references the `SceneLoader` script and the Terrain scene contains a Grid + Tilemap.
- Confirmed the pushed `BryceTest` matches local and `main` was unchanged.

**Not verified / risks**
- **`SceneLoader` hasn't been tested in Play mode** — batch mode can't press Play. Test: open `Hill01_Gameplay`, press Play, and check all four scenes appear in the Hierarchy.
- The Unity 6 version-control e-book was behind a download form, so only its landing page was read.
- Community sources disagree on some conventions (asset name prefixes, type vs feature folders); the README records the choices the team made.

**Commits**
- `6503021` Reorganize _Game into feature-based layout
- `f154120` Add assembly definitions for runtime, editor, and tests
- `02c2916` Split levels into additive scenes with a SceneLoader
- `051522a` Turn domain reload back on for Play mode
- `eaa4084` Update README for new layout; track .ase files in LFS

### Part 3: Devlogs

**What was asked**
- Create a devlog and an AI devlog for the repository.

**What the AI did**
- Created `docs/DEVLOG.md` (team log with a template, backfilled from commit history) and this file; linked both from the README.

**Verification**
- Dates, authors, and commit hashes copied from `git log`.

**Commits**
- The commit that adds this file (see `git log -- docs/AI_DEVLOG.md`)
