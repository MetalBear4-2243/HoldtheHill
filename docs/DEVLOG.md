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

## 2026-11-04 — Enemy hurt and death animations
**Who:** Nilly (nilly-ctrl), with AI assistance — see [AI use log](AI_DEVLOG.md)
**Branch:** `nilly/alpha-animations`

**Worked on**
- Each hit that takes health plays the enemy sheet's 2-frame Hurt row over its walk (`EnemyHurtAndDeath`). The walk keeps time underneath, so it carries on from where it was instead of jumping.
- A death leaves the body behind for a moment: an `EnemyDeathEffect` plays the 7-frame Death row where the enemy died, facing the way it walked, then fades out. It sits in the level scene, so Restart and Home clear it.
- An enemy that reaches the Queen leaves no body. Shield hits on the Rhino Beetle don't flinch it; only hits on its health do.
- Seven enemies get it. The Rolly Polly's pill-bug sheet has no Hurt or Death rows, so it stays as it was; the Queen Mantis keeps her own art.
- Build Sprites adds the rows to each enemy prefab. Rerunning it changes nothing.

**Decisions**
- The enemy is still removed the instant it dies, as before; the body left behind is only a picture, so bounties, the win check and the Queen Mantis's raises are unchanged.

**Next**
- Tower Upgrade rows; the Rhino's ShieldHit and ShieldBreak; the Rolly Polly's Roll while curled.

## 2026-11-04 — Tower attack animations
**Who:** Nilly (nilly-ctrl), with AI assistance — see [AI use log](AI_DEVLOG.md)
**Branch:** `nilly/alpha-animations`

**Worked on**
- Every tower plays its sheet's 6-frame Attack row over its idle loop (`TowerAttackAnimation`). Frames 0-1 wind up and frame 2 lands the blow, as the art is drawn.
- A tower that is about to fire at a target starts winding up early, so the hit frame shows the moment the swing lands or the shot leaves. If an enemy walks into range of a tower that is already ready, it jumps straight to the hit frame.
- The row is shortened to fit when a tower fires faster than it plays (Crazy Ant, upgrades, Swarm Frenzy). Stunned or burrowed towers go back to idle.
- The Carpenter Ant plays it when it lays a mine, the Sap Ant while its aura is slowing someone.
- `Tools > Hold the Hill > Alpha > Build Sprites` adds the attack row to each tower prefab. Rerunning it changes nothing.

**Decisions**
- The animation is only visual: damage and fire rate are unchanged, and the wind-up never delays a shot.

**Next**
- Tower Upgrade rows, then enemy Hurt and Death, from the same sheets.

## 2026-11-04 — Adam's tower list fixes
**Who:** Nilly (nilly-ctrl), with AI assistance — see [AI use log](AI_DEVLOG.md). Adam's files, edited with the team's go-ahead in the 11-04 meeting.
**Branch:** `nilly/adam-ui-fixes`

**Worked on**
- The tower list's open blocker from 10-24: the card of the tower in hand is highlighted (gold tint), whether it was picked by card, number key or is still in hand after a build. Clicking that card again puts the tower away. Cards do nothing while paused.
- `TowerList.prefab` lists the six real towers in number-key order instead of the six sandbox test towers (UI-2). The UI scene already did this as an override; now the prefab itself is right. A data test checks it.
- `TowerData` moved beside the tower code, `Features/Towers/Scripts/TowerData.cs`, in the `HoldTheHill.Features.Towers` namespace (UI-10). Same GUID, same fields, so every asset keeps its values.
- Create menu spelled "Hold the Hill" everywhere (PU-4). `TowerCardUI` uses `FindAnyObjectByType` (the old call is deprecated in 6000.6).

**Decisions**
- `TowerCardUI` and `TowerShopManager` keep their global namespace for now: the card's click is wired in the prefab by type name, and renaming it unplayed could break every card.
- The balance box's live amount stays a scene override (HoneydewLabel); the prefab's text still says 150.

**Problems / blockers**
- Two EventSystems when playing a sandbox scene with the UI scene (CO-3) is still a team call.

**Next**
- Adam checks the highlight colour on the real card art.
## 2026-11-04 — Jason's enemy fixes from the 10-20 review
**Who:** Nilly (nilly-ctrl), with AI assistance — see [AI use log](AI_DEVLOG.md). Jason's files, edited with the team's go-ahead in the 11-04 meeting.
**Branch:** `nilly/jason-enemy-fixes`

**Worked on**
- An enemy that reaches the Queen is out of the fight at once (`BasicEnemy.ReachedEnd`): a hit in the same frame no longer pays a bounty or splits a centipede into children that never move (known item 11, EN-1).
- A centipede killed within a unit of the end of the path no longer drops a child on the Queen: the second half starts beside the first (known item 10). No child gets a waypoint past the end (TS-6).
- Centipede part damage (EN-3): Jason halved it himself in PR 49 while this was in progress; his rule (half, rounded down, at least 1, zero stays zero) and his tests are kept.
- A split prefab without a `CentipedeEnemy` is refused with an error instead of leaving the parent walking at 0 HP (EN-10).
- Both `TakeDamage` overloads raise `OnDamaged`, once per hit (TW-4, EN-5). A damage-over-time refreshed faster than it ticks still ticks, and a refresh no longer allocates (EN-8, TW-8).
- The Queen no longer logs her health on every hit (GF-2). The spawner's wave and map messages are off unless "Log Progress" is ticked; warnings stay. Its living-enemy count now drops when enemies are destroyed.
- His 7 failing EditMode tests pass: health set in SetUp, enemies removed with DestroyImmediate outside Play mode, the Queen test subscribes by hand (TS-1, TS-2, TS-3). 9 new tests.

**Decisions**
- The silverfish click needed nothing: Jason had already turned `OnMouseDown` off.

**Problems / blockers**
- Still open for Beta: a status effect made with SlowMultiplier left at 0 stops the enemy (CB-1); no `WaveCleared` event on the spawner.

**Next**
- Jason reviews the PR. Play a Meadow run with centipedes reaching the Queen.
## 2026-11-04 — Known bugs from the 10-20 review, before the Alpha
**Who:** Nilly (nilly-ctrl), with AI assistance — see [AI use log](AI_DEVLOG.md)
**Branch:** `nilly/alpha-known-bugs`

**Worked on**
- Fire Ant lobs follow their target, so they land on moving Scarabs instead of where the Scarab was (G1-1).
- Ant and Crazy Ant reach 1.5 / 1.6 / 1.7 (was 1.2 / 1.3 / 1.4), so one on a road corner's outer diagonal can hit the road (G1-3). A data test checks every melee tower.
- A range ring shows around the ghost before you buy and around the selected tower (G2-1).
- The Next Wave button greys out while a wave is still coming out, after the last wave, while paused and after the run ends. Space uses the same check, so it no longer logs spawner warnings (UI-7, GF-3).
- Diatomaceous earth settles for 1.5 s, fading in, before it hurts or stuns anything. Until now it stunned towers on its first frame with no warning (HZ-3 warning half).
- Towers fire at their stated rate: a shot keeps the part of a frame it was late by (TW-6). The Sap Ant and Carpenter Ant no longer search for targets every frame (TW-9).
- A centipede half killed in the frame it split off now pays its bounty (known item 9).
- Restart and Home unload a level's Gameplay scene before its Terrain, also after Play was pressed in a Gameplay scene (CO-1).

**Decisions**
- Jason's files are untouched: `BasicEnemy`, `CentipedeEnemy`, `Queen`, `SilverfishEnemy`, `EnemySpawner` and his tests. His items are below.
- Dust warns but burrowing still doesn't protect from it; that half of HZ-3 is a design call.
- The builders that make the Ant and Crazy Ant carry the new ranges, so a rerun keeps them.

**Problems / blockers**
- Needs Jason: centipede part damage (EN-3), the end-of-path split (known 10), the per-hit Queen log, the per-wave spawner logs, and his 7 failing EditMode tests.
- No 1x / 2x button on the HUD yet (the key works); it needs a UI scene change in the Editor.

**Next**
- Play Tutorial Hill and Meadow on this branch, then in a Windows build, before the 11-06 Alpha.

## 2026-10-24 — UI TowerList additions
**Who:** Adam Montgomery with Google Gemini
**Branch:** adamUIPrefabUpdate

**Worked on**
- Updating current UI scene with addition of TowerList and custom Queen health bar.
- Coupled TowerList with TowerPlacer.
- Updated TowerCardUI to accept call TowerPlacer method on click.


**Decisions**
- Made custom Queen health bar container and changed health color to green

**Problems / blockers**
- TowerList needs some UI improvements such as highlighted selection and ability to unselect.

**Next**
- Bug testing on TowerPlacer.
- fix UI issues with TowerList.
- Continue updating UI elements on the UI scene.
```
## 2026-10-09 — Tower Shop Architecture Complete
**Who:** Adam Montgomery, with AI assistance — see [AI use log](AI_DEVLOG.md)
**Branch:** `Sandbox/Montgomery-Adam`

**Worked on**
- Architected dynamic Tower Shop UI using ScriptableObjects (TowerData). 
- Built TowerShopManager.cs to instantiate UI cards into a ScrollRect and TowerCardUI.cs to map data to visuals. 
- Analyzed nilly's Tower.cs to ensure architecture alignment. 
- Debugged UI scaling issues.

**Decisions**
- Decouple UI from combat logic using data containers. 
- Grouped UI into a TowerShopPanel master prefab. 
- Commented out nilly's namespace and prefab references to maintain clean compilation until their fork is merged into Indev.

**Problems / blockers**
- Actual tower placement logic and prefab linking (pending Indev merge).

## 2026-10-02 — Dynamic Tower Card UI & Prefab Setup
**Who:** Adam Montgomery, with AI assistance — see [AI use log](AI_DEVLOG.md)
**Branch:** `Sandbox/Montgomery-Adam`

**Worked on**
- Fixed asset transparency issues by establishing a PNG pipeline and using the Unity Sprite Editor for non-square icons.
- Built the `TowerCardBase` UI template and saved it as a prefab in the `_Indev/Features/UI/` folder.
- Wrote `TowerCardDisplay.cs` to link UI elements (`Image`, `TextMeshProUGUI`) via the Inspector using `[Header]` organization.
- Fixed a VS Code `.NET SDK not found` error to restore C# IntelliSense and error checking.
- Created `TowerCardTester.cs` to successfully validate dynamic data injection (name, cost, sprite) when entering Play mode.

**Decisions**
- Enforced transparent PNGs over JPGs for all UI sprites to eliminate solid background boxes.
- Used `Image` variable types in scripts for UI framework slots and `Sprite` types for passing the actual art assets.

**Problems / blockers**
- None currently, but the underlying data architecture for storing the actual tower stats (health, damage, etc.) doesn't exist yet.

**Next**
- Establish the data structure for tower stats.
- Build out the Tower list menu to instantiate and populate these card prefabs dynamically.

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
