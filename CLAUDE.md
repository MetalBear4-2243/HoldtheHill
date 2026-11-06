# Hold the Hill (team repo)

The rules for every AI tool are in `AGENTS.md`; this file only adds notes for Claude Code.

@AGENTS.md
@AGENTS.local.md

## Claude Code notes
- Open pull requests with `gh pr create --repo MetalBear4-2243/HoldtheHill --base Indev --head <branch>`. Branches live on the team repo; there is no fork since 2026-10-17.
- A push rejected with `GH007` means the commit carries a private email. Fix the repo's local `user.email`; don't change the global one.
- Levels are additive scene pairs: `Levels/<Level>/<Level>_Terrain` (tilemaps) + `<Level>_Gameplay`; `SceneLoader` in `Scenes/Bootstrap` loads the rest on Play. The levels are `L00_TutorialHill` and `L01_Meadow`; `Hill01` is the old template and stays out of the build.
- The Alpha scenes and prefabs are written by the builders in `_Game/Editor/Alpha*Builder.cs` (menu `Tools > Hold the Hill > Alpha`). Change the builder, rerun it, and commit both.
