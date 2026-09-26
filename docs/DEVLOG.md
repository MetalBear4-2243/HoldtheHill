# Hold the Hill — Devlog

What the team worked on, what we decided, and why. **Newest entries at the top.**

Add an entry when you finish a task or make a decision the rest of the team should know about. Keep it short. If AI helped with the work, also log it in the [AI devlog](AI_DEVLOG.md).

## Entry template

Copy this to the top of the log:

```markdown
## YYYY-MM-DD — Short title
**Who:** your name / GitHub username
**Branch:** branch-name

**Worked on**
- What you built or changed

**Decisions**
- What you chose and why (so nobody has to ask later)

**Problems / blockers**
- Anything that broke, is unfinished, or needs someone else

**Next**
- What you or the team should do next
```

---

## 2026-09-23 — Baseline BasicEnemy Script and Tests
**Who:** Jason_Kimoto (MetalBear4-2243), with AI assistance — see [AI use log](AI_DEVLOG.md)
**Branch:** `BasicEnemyScript`

**Worked on**
- Created `BasicEnemy.cs` in `Hold the Hill/Assets/_Game/Features/Enemies/Scripts/` as the baseline component for all basic enemies.
- Configured health as a `double` (`_maxHealth`, `_currentHealth`, `TakeDamage(double)`, `Heal(double)`).
- Configured damage as an `int` (`_damage`, `Damage`).
- Implemented path traversal based on world coordinates (`_pathCoordinates`, `SetPath(...)`), with overloads for `Vector3`, `Vector2`, and `EnemyPath`.
- Added automatic sprite facing direction flip on the X-axis based on travel direction.
- Added event hooks (`OnDied`, `OnDestinationReached`, `OnHealthChanged`) and Scene view path gizmos.
- Added EditMode tests in `Hold the Hill/Assets/_Game/Tests/EditMode/BasicEnemyTests.cs`.
- Wrote comprehensive technical documentation in `Documentation/EnemyScripts_Documentation.md`.

**Decisions**
- Kept movement transform-based (`Vector3.MoveTowards`) for reliable 2D grid/road waypoint traversal.
- Supported both direct coordinate lists (`Vector2`/`Vector3`) and scene `EnemyPath` components for flexible spawning and testing.

**Problems / blockers**
- None.

**Next**
- Integrate `BasicEnemy` prefab variants into `EnemySpawner` wave definitions and test in Play mode.

---

## 2026-09-13 — Project setup, team conventions, and folder structure

**Who:** Nilly (nilly-ctrl), with AI assistance — see [AI use log](AI_DEVLOG.md)
**Branch:** `BryceTest` (not merged into `main`)

**Worked on**
- Set up Git for Unity teamwork: Git LFS for art/audio, Unity scene merging, fixed `.gitignore` rules for the `Hold the Hill/` subfolder.
- Made source art files (Aseprite, PSD, Krita, Affinity, Blender) lockable so two people can't overwrite each other's art.
- Added `.editorconfig` so Rider and Visual Studio format C# the same way.
- Removed template leftovers: 2D tutorial, Unity Version Control, Visual Scripting.
- Reorganized `Assets/_Game/` by feature: `Core/`, `Features/` (10 systems), `Shared/`, plus `ThirdParty/` and per-person `_Sandbox/` folders.
- Split levels into additive scenes (`Bootstrap`, `UI`, `Hill01_Terrain`, `Hill01_Gameplay`) with a `SceneLoader` script.
- Added assembly definitions for game code, editor tools, and tests.
- Wrote the README.

**Decisions**
- **Game scope:** single-player hybrid tower defense (player character + buildable towers), semester-long project.
- **Company name:** Turbulent Towers Studio.
- **Layout:** organize by feature, with small systems merged (e.g. currency + harvestables + loot → `Economy/`).
- **Scenes:** split each level into Terrain and Gameplay scenes so tile painting and object placement don't conflict.
- **Play mode:** domain reload turned **on** — slightly slower Play, but no leftover static values between runs.
- **Aseprite files** stay inside Unity, imported with the Aseprite Importer.
- **Branches:** all of this is on `BryceTest`; `main` only changes through a pull request.

**Problems / blockers**
- `SceneLoader` compiles but hasn't been tested by pressing Play yet.
- The Windows scene-merging path in the README hasn't been checked on a Windows PC.
- `main` isn't protected yet — the repo owner needs to require pull requests in GitHub settings.

**Next**
- Open `Hill01_Gameplay`, press Play, and confirm all four scenes load.
- A Windows teammate confirms the merge tool path.
- Team reviews `BryceTest` and opens a pull request into `main`.

---

## 2026-09-11 — Repository created
**Who:** Jason_Kimoto (MetalBear4-2243)
**Branch:** `main`

**Worked on**
- Created the GitHub repository and a Unity 6000.6.0f1 2D (URP) project in `Hold the Hill/`.
- Added a standard Unity `.gitignore`.
