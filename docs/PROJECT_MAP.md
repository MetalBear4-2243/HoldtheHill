# Hold the Hill — Project Map

Where everything for the project lives, and how the pieces connect. Generated 2026-11-05; the links come from a hand-kept list, so tell nilly-ctrl when something moves.

## Where things live

| System | It is the source of truth for | Links |
|---|---|---|
| **Notion** | Tasks, due dates, who is on what | [Dev Hub](https://app.notion.com/p/3bf1fdfffa614874b9140ad926038a68) · [Tasks board](https://app.notion.com/p/4b57b00acb634df58f3c54dde0165591) · [Dev Activity (AI session summaries)](https://app.notion.com/p/3f2101a0f05381eeb0f3e9ab224f0c13) · [Project Map](https://app.notion.com/p/3f3101a0f05381cab882e5af8416fceb) |
| **GitHub** | Code, scenes, art that ships, the two devlogs | [Team repo](https://github.com/MetalBear4-2243/HoldtheHill) · [Indev branch (integration)](https://github.com/MetalBear4-2243/HoldtheHill/tree/Indev) · [Pull requests](https://github.com/MetalBear4-2243/HoldtheHill/pulls) · [Conventions](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/docs/CONVENTIONS.md) · [Technical documents](https://github.com/MetalBear4-2243/HoldtheHill/tree/Indev/Documentation) |
| **Google Drive** | Design documents, course paperwork, meeting notes, playable builds | [Turbulent Towers (team folder)](https://drive.google.com/drive/folders/1VftIzL7c_CV-h18aVd-WEPmxm3Dfqm7o) · [Design Documentation](https://drive.google.com/drive/folders/18Tsq9rP0p2FIICY5yCtE88WhYD31GSem) · [Meeting Notes](https://drive.google.com/drive/folders/18OUMMoxfp0Kdh2xWe_lK8cFY9K2FcaQB) · [Art](https://drive.google.com/drive/folders/1OxtztLOjM5nvgyxIyRVZEmjaNTL5QpSO) · [Builds](https://drive.google.com/drive/folders/1fLXSzyWoWgpzYwYOOEhTfyrItTJBywpl) · [Robot Help (AI research)](https://drive.google.com/drive/folders/1MUv8jjESt6vZvISKQ-Kd-jGoj3d2oizg) |
| **Unity** | The game project itself (opened from the repo's unity/ folder) | [Setup and folder rules (README)](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/README.md) · [Team devlog](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/docs/DEVLOG.md) · [AI use log](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/docs/AI_DEVLOG.md) |

Unity version: **6000.6.0f1** exactly. Open the `unity/` folder, not the repo root.

**One rule keeps these in step:** a task lives in Notion, its code lives in a branch named `name/what-it-does`, and the pull request into `Indev` links back to the Notion task. A task is Done when that pull request is merged.

## Tasks and their code

| Task | Notion status | Code | Where |
|---|---|---|---|
| [Combat: Targeting helper (nearest enemy in range), reusable by towers](https://app.notion.com/3db101a0f0538170b705fa83ba2282e3) | Done | merged | [PR 32](https://github.com/MetalBear4-2243/HoldtheHill/pull/32) |
| [Combat Mechanic](https://app.notion.com/3d8101a0f05381048cd0cd1aaf6be2bf) | Not Started | merged | [PR 32](https://github.com/MetalBear4-2243/HoldtheHill/pull/32), [PR 35](https://github.com/MetalBear4-2243/HoldtheHill/pull/35) |
| [Combat: Health and damage components shared by player, enemies, hill](https://app.notion.com/3db101a0f05381aa9757d9abb38e1e32) (unconfirmed link) | Not Started | merged | [PR 11](https://github.com/MetalBear4-2243/HoldtheHill/pull/11), [PR 22](https://github.com/MetalBear4-2243/HoldtheHill/pull/22) |
| [Combat: Hit feedback (flash, knockback, placeholder sound)](https://app.notion.com/3db101a0f053814fbc4bd3cc40fc168e) (unconfirmed link) | In Progress | merged | [PR 53](https://github.com/MetalBear4-2243/HoldtheHill/pull/53) |
| [Enemy 1: Basic wave spawner to test in Hill01](https://app.notion.com/3db101a0f053815d86aadcc9881a27e7) | Done | merged | [PR 3](https://github.com/MetalBear4-2243/HoldtheHill/pull/3), [PR 7](https://github.com/MetalBear4-2243/HoldtheHill/pull/7) |
| [Enemy 1: Follow path to the hill (waypoints)](https://app.notion.com/3db101a0f0538114b486f09963aeea09) | Done | merged | [PR 7](https://github.com/MetalBear4-2243/HoldtheHill/pull/7) |
| [Enemy 1 + Unique Mechanic](https://app.notion.com/3d8101a0f053814d8018c898c5325128) | Not Started | merged | [PR 15](https://github.com/MetalBear4-2243/HoldtheHill/pull/15), [PR 21](https://github.com/MetalBear4-2243/HoldtheHill/pull/21), [PR 36](https://github.com/MetalBear4-2243/HoldtheHill/pull/36) |
| [Enemy 1: Sprite with walk, attack, death animations](https://app.notion.com/3db101a0f05381cbbd2ce9ebff964c69) | Not Started | merged | [PR 47](https://github.com/MetalBear4-2243/HoldtheHill/pull/47), [PR 52](https://github.com/MetalBear4-2243/HoldtheHill/pull/52), [PR 53](https://github.com/MetalBear4-2243/HoldtheHill/pull/53) |
| [Boss 1 + 2 Unique Mechanics](https://app.notion.com/3d8101a0f0538142982cc688da975de8) | Not Started | merged | [PR 37](https://github.com/MetalBear4-2243/HoldtheHill/pull/37), [PR 46](https://github.com/MetalBear4-2243/HoldtheHill/pull/46) |
| [Boss 1: Boss health bar UI](https://app.notion.com/3db101a0f0538182bc77e8626d3a026f) | Not Started | merged | [PR 46](https://github.com/MetalBear4-2243/HoldtheHill/pull/46) |
| [Hazard 1 + Unique Mechanic](https://app.notion.com/3d8101a0f0538155853ce2e2338d4f9b) | Not Started | merged | [PR 38](https://github.com/MetalBear4-2243/HoldtheHill/pull/38) |
| [Power-up 1](https://app.notion.com/3d8101a0f053811dab53efc2b49f0e2b) | Not Started | merged | [PR 39](https://github.com/MetalBear4-2243/HoldtheHill/pull/39) |
| [HUD Mechanic](https://app.notion.com/3d8101a0f05381f9acedd2f8dca09ec1) | In Progress | merged | [PR 12](https://github.com/MetalBear4-2243/HoldtheHill/pull/12), [PR 13](https://github.com/MetalBear4-2243/HoldtheHill/pull/13), [PR 16](https://github.com/MetalBear4-2243/HoldtheHill/pull/16), [PR 45](https://github.com/MetalBear4-2243/HoldtheHill/pull/45), [PR 51](https://github.com/MetalBear4-2243/HoldtheHill/pull/51), [PR 57](https://github.com/MetalBear4-2243/HoldtheHill/pull/57) |
| [HUD: Connect HUD to game events (health, currency, wave changes)](https://app.notion.com/3db101a0f053814482b7dd1f600b56c8) | Done | merged | [PR 33](https://github.com/MetalBear4-2243/HoldtheHill/pull/33) |
| [HUD: Hill health bar with damage flash](https://app.notion.com/3db101a0f05381659bffd8e41f1ce66c) | In Progress | merged | [PR 33](https://github.com/MetalBear4-2243/HoldtheHill/pull/33) |
| [HUD: Pick HUD elements and sketch layout (hill HP, player HP, currency, wave)](https://app.notion.com/3db101a0f053816baf7af8cc5f6714e3) | Done | merged | [PR 33](https://github.com/MetalBear4-2243/HoldtheHill/pull/33) |
| [HUD: Pixel-art HUD frame and icons](https://app.notion.com/3db101a0f053812984e2e123f2ef0375) | Done | none | — |
| [Title Screen](https://app.notion.com/3d8101a0f05381daa8a1c1ade6bbb3fd) | In Progress | merged | [PR 34](https://github.com/MetalBear4-2243/HoldtheHill/pull/34) |
| [Title: Logo and background art](https://app.notion.com/3db101a0f05381dea210d63a184c3788) | Done | none | — |
| [Title: Play loads Hill01 through SceneLoader (switch to Level Select once built)](https://app.notion.com/3db101a0f053819baf5accb24e36599d) | Needs Help | merged | [PR 34](https://github.com/MetalBear4-2243/HoldtheHill/pull/34) |
| [Level 1](https://app.notion.com/3d8101a0f05381098acccfc25e8ca926) | Not Started | merged | [PR 34](https://github.com/MetalBear4-2243/HoldtheHill/pull/34), [PR 46](https://github.com/MetalBear4-2243/HoldtheHill/pull/46) |
| [Level 1: Win and lose conditions (hill HP reaches 0 / all waves survived)](https://app.notion.com/3db101a0f05381188d3ad052f613d4f1) | Not Started | merged | [PR 33](https://github.com/MetalBear4-2243/HoldtheHill/pull/33), [PR 58](https://github.com/MetalBear4-2243/HoldtheHill/pull/58) |
| [Level Select](https://app.notion.com/3d8101a0f053816cabe8d11d7d66585e) | Not Started | merged | [PR 34](https://github.com/MetalBear4-2243/HoldtheHill/pull/34) |
| [Camera: Set up Input System actions (move, attack, build/interact, pause)](https://app.notion.com/3db101a0f0538155bb9cf677fc1ee3e6) (unconfirmed link) | In Progress | merged | [PR 33](https://github.com/MetalBear4-2243/HoldtheHill/pull/33) |
| [Alpha demo: Bug bash on a full build](https://app.notion.com/3db101a0f05381a780c6f3db28b023bb) | Not Started | merged | [PR 40](https://github.com/MetalBear4-2243/HoldtheHill/pull/40), [PR 48](https://github.com/MetalBear4-2243/HoldtheHill/pull/48), [PR 49](https://github.com/MetalBear4-2243/HoldtheHill/pull/49), [PR 50](https://github.com/MetalBear4-2243/HoldtheHill/pull/50), [PR 51](https://github.com/MetalBear4-2243/HoldtheHill/pull/51), [PR 53](https://github.com/MetalBear4-2243/HoldtheHill/pull/53), [PR 54](https://github.com/MetalBear4-2243/HoldtheHill/pull/54), [PR 58](https://github.com/MetalBear4-2243/HoldtheHill/pull/58) |

## Needs attention

- **Combat Mechanic**: Code merged, task still open.
- **Combat: Health and damage components shared by player, enemies, hill**: Code merged, task still open.
- **Combat: Hit feedback (flash, knockback, placeholder sound)**: Code merged, task still open.
- **Enemy 1 + Unique Mechanic**: Code merged, task still open.
- **Enemy 1: Sprite with walk, attack, death animations**: Code merged, task still open.
- **Boss 1 + 2 Unique Mechanics**: Code merged, task still open.
- **Boss 1: Boss health bar UI**: Code merged, task still open.
- **Hazard 1 + Unique Mechanic**: Code merged, task still open.
- **Power-up 1**: Code merged, task still open.
- **HUD Mechanic**: Code merged, task still open.
- **HUD: Hill health bar with damage flash**: Code merged, task still open.
- **HUD: Pixel-art HUD frame and icons**: Marked Done, code is not all on Indev.
- **Title Screen**: Code merged, task still open.
- **Title: Logo and background art**: Marked Done, code is not all on Indev.
- **Title: Play loads Hill01 through SceneLoader (switch to Level Select once built)**: Code merged, task still open.
- **Level 1**: Code merged, task still open.
- **Level 1: Win and lose conditions (hill HP reaches 0 / all waves survived)**: Code merged, task still open.
- **Level Select**: Code merged, task still open.
- **Camera: Set up Input System actions (move, attack, build/interact, pause)**: Code merged, task still open.
- **Alpha demo: Bug bash on a full build**: Code merged, task still open.
- Branch `filechecking` (last commit 2026-09-26, Jason_Kimoto) is not merged into Indev and has no pull request.
- Branch `nilly/alpha-core-loop` (last commit 2026-10-16, nilly-ctrl) is not merged into Indev and has no pull request.
- Branch `nilly/alpha-projectile-art` (last commit 2026-11-04, nilly-ctrl) is not merged into Indev and has no pull request.
- Branch `nilly/alpha-review-fixes` (last commit 2026-10-19, nilly-ctrl) is not merged into Indev and has no pull request.

## AI tools

Every AI tool should follow the same rules. These are the files each one reads:

| File | Read by | On Indev |
|---|---|---|
| `AGENTS.md` | Every AI tool: the shared rules | yes |
| `CLAUDE.md` | Claude Code: imports AGENTS.md, adds Claude-only notes | yes |
| `.gemini/rules.md` | Antigravity: points at AGENTS.md | yes |
| `AGENTS.local.md` | Every AI tool, one computer: that machine's rules; never committed | local only |
| `docs/AI_DEVLOG.md` | Everyone: one short row per AI-assisted session | yes |

Any AI-assisted work gets one row in `docs/AI_DEVLOG.md`, and commits keep their `Co-Authored-By` line.
