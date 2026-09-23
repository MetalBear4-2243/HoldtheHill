# Hold the Hill — AI Use Log

Short disclosure of AI-assisted work on this project. Our instructor encourages using AI and asks us to disclose it.

- **Add one row** whenever AI helps with a task (code, research, docs, art tools).
- **A person reviews AI work** before it goes into `main`.
- **Details are in Git:** AI-made commits carry a `Co-Authored-By` line. See them with `git log --grep="Co-Authored-By: Claude"`.

| Date | Who | Tool | What the AI helped with | What we decided | Not tested yet |
|---|---|---|---|---|---|
| 2026-09-13 | nilly-ctrl | Claude Code (Claude Opus 5) | Git LFS and scene-merge setup, art file locking, `.editorconfig`, removed template packages, first README | Work on `BryceTest`; lock art files; studio name Turbulent Towers Studio | Windows merge-tool path in README |
| 2026-09-13 | nilly-ctrl | Claude Code (Claude Opus 5) | Researched Unity folder layouts ([Unity guide](https://unity.com/how-to/organizing-your-project), [Unity blog](https://unity.com/blog/author-scenes-and-prefabs-with-verson-control)); reorganized into feature folders; additive scenes + `SceneLoader`; assembly definitions | Merge small features; split level scenes; domain reload **on** (AI suggested off) | `SceneLoader` in Play mode |
| 2026-09-13 | nilly-ctrl | Claude Code (Claude Opus 5) | Wrote the team devlog and this log | Keep AI disclosure to a short table | — |
| 2026-09-16 | nilly-ctrl | Claude Code (Claude Opus 5) | Wrote `CLAUDE.md`, a guide for Claude summarizing the README, branch rules, LFS locking and code layout | Commit it on `BryceTest`; README stays the source of truth | — |
| 2026-09-16 | nilly-ctrl | Claude Code (Claude Opus 5) | Ported grid tower placement, enemy path/road, placeholder sprite, and "Create Example Path" menu from the solo tower-defense project into the feature folders | Code only (no scenes/prefabs); Towers depend on Enemies; menu under Tools > Hold the Hill | Placement and road in Play mode (compiles clean in Unity 6.6) |
| 2026-09-23 | nilly-ctrl | Claude Code (Claude Opus 5) | Updated `CLAUDE.md` to describe the real branch model — `Indev` as the group integration/test branch, `main` as stale-and-deliberate — replacing the stale "working branch is `BryceTest`, PR into `main`" rule; also recorded the fork/upstream remotes and the noreply-email requirement | `Indev` is what we branch from and PR into until docs and structure move to `main`; flagged that README "Team workflow" still says `main` and needs the same change | — docs only, no code touched |
