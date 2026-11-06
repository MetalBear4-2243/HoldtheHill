# Fork documentation index

**Status on 2026-11-05.** The team chose on 2026-10-14 to keep its own code and build the Alpha on
it, and the fork was retired on 2026-10-17. The fork classes these pages describe (`EnemyHealth`,
`EnemyMover`, `Wallet`, `GameFlowController`, `BuildGrid`, `ProjectileFlight`) are not on `Indev`
and are not coming. The pages stay as a record of how those systems were designed; for the code that
shipped, read the team documents in the folder above and the AI devlog rows from 2026-10-16 onward.

The fork's technical documents, one per system, in the same format as the team's documents in the
folder above. Where the team's folder has a document for the same system, the two are meant to be
read side by side; each fork document ends with what differs. Nothing here replaces a team document;
the team chose its own code on 2026-10-14, as the decision column says.

| Document | Covers | Team counterpart | Decision |
| :--- | :--- | :--- | :--- |
| `EnemyScripts_Documentation.md` | `EnemyHealth`, `EnemyMover`, `EnemyShield`, `EnemyHealer`, `EnemySplitter`, `EnemyPath` | `../EnemyScripts_Documentation.md` (BasicEnemy) | Team's code stays (2026-10-14) |
| `EnemySpawner_Documentation.md` | `EnemySpawner`, `MapWaveLookup` | `../EnemySpawner_Documentation.md` | Team's code stays (2026-10-14) |
| `MapWaveData_Documentation.md` | `EnemySpawnEntry`, `Wave`, `MapWaveData`, `MapWaveDataSO` | `../MapWaveData_Documentation.md` | Team's code stays (2026-10-14) |
| `Combat_Documentation.md` | `IDamageable`, `IHealthReadable`, `IOwned`, `DamageInfo`, `StatusEffectData`, `CombatUtil` | none | Team's code stays (2026-10-14) |
| `Towers_Documentation.md` | `Tower`, `TargetingPriority`, `TowerCredit`, `TowerWeapon` and the weapon catalogue, `Projectile`, `ProjectileFlight`, `ProjectilePool` | none for the weapons; `Tower.cs` on Indev was a range-and-cell stub | Team's code stays (2026-10-14) |
| `Queen_Documentation.md` | `Queen`: the hill's health, breaches from `EnemyMover.ReachedEnd`, defeat | `Queen.cs` on Indev (health pool only) | Team's code stays (2026-10-14) |
| `Economy_Documentation.md` | `ResourceData`, `Wallet`, `WalletBehaviour` | none; the Indev shop reads a cost and spends nothing | Team's code stays (2026-10-14) |
| `GameFlow_Documentation.md` | `GameFlowState`, `GameFlowController` | none; `SceneLoader.cs` is the nearest thing | Team's code stays (2026-10-14) |
| `Placement_Documentation.md` | `TowerPlacer`, `TowerCommands`, `BuildRules`, `BuildGrid`, `IBuildBlocker`, `PathClearanceBlocker`, `PlacementGhost` | `TowerPlacer.cs` on Indev and the path's old `Blocks` rule | Team's code stays (2026-10-14) |
| `TowerData_Documentation.md` | `TowerData` asset: cost, size, upgrade tiers and the modifiers towers and the placer read | `UI/TowerListScripts/TowerData.cs` on Indev (global namespace, read by the shop and card) | Team's code stays (2026-10-14) |

Template for new ones: `_TEMPLATE_Documentation.md`. Plan: `../../docs/DOCUMENTATION_PLAN.md`.
