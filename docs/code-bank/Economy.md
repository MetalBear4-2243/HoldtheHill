# Economy

Part of the [code bank](README.md), built 2026-11-05 from commit `d6944f7b`.

## Scripts

| Script | Kind | Owner | What it does |
|---|---|---|---|
| [Bounty](#bounty) | MonoBehaviour | Nilly | Honeydew paid to the player when this enemy dies. Sits on the enemy prefab next to BasicEnemy and listens to its death; the enemy class itself is untouched. Bos… |
| [Honeydew](#honeydew) | Class | Nilly | The one currency of a run. Plain C#, so the rules can be tested without a scene; HoneydewWallet puts one in a scene. |
| [HoneydewWallet](#honeydewwallet) | MonoBehaviour | Nilly | Holds the run's Honeydew in the gameplay scene. One per level. Towers spend from it, bounties and wave rewards pay into it, the HUD listens to it. |
| [WaveReward](#wavereward) | MonoBehaviour | Nilly | Pays a flat Honeydew reward each time the spawner finishes a wave. Lives next to the HoneydewWallet in the gameplay scene. |

### Bounty

`unity/Assets/_Game/Features/Economy/Scripts/Bounty.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Economy/Scripts/Bounty.cs)) · MonoBehaviour · `HoldTheHill.Features.Economy` · HoldTheHill.Runtime · owner: Nilly

Honeydew paid to the player when this enemy dies. Sits on the enemy prefab next to BasicEnemy and listens to its death; the enemy class itself is untouched. Bos…

Derives from / implements: MonoBehaviour

Used on: Enemy_Basic, Enemy_Centipede, Enemy_QueenMantis, Enemy_RhinoBeetle, Enemy_RollyPolly, Enemy_Silverfish, Enemy_Wasp

Named in: AlphaBossesBuilder, AlphaCoreLoopBuilder, AlphaEnemiesBuilder, QueenMantisBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_honeydew` | `int` | `5` |  | Enemy_Basic; Enemy_Centipede; Enemy_MantisSpawn; Enemy_QueenMantis; Enemy_RhinoBeetle; Enemy_RollyPolly; Enemy_Scarab; Enemy_Silverfish (+1 more) | Honeydew paid when this enemy is killed. Nothing is paid for reaching the Queen. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Amount` | Property | `int` | `=> _honeydew` | Honeydew paid on death. |  |

Private members not listed: 8.

### Honeydew

`unity/Assets/_Game/Features/Economy/Scripts/Honeydew.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Economy/Scripts/Honeydew.cs)) · Class · `HoldTheHill.Features.Economy` · HoldTheHill.Runtime · owner: Nilly

The one currency of a run. Plain C#, so the rules can be tested without a scene; HoneydewWallet puts one in a scene.

Named in: Bounty, DevKeys, HoneydewLabel, HoneydewTests, HoneydewWallet, TowerCardPanel, TowerPlacer, WaveReward

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Changed` | Event | `Action<int>` |  | Raised when the amount changes, with the new amount. | HoneydewLabel, HoneydewTests, TowerCardPanel |
| `Honeydew` | Constructor |  | `(int startingAmount)` | Starts with startingAmount, or zero if that is negative. | Bounty, DevKeys, HoneydewLabel, HoneydewTests, HoneydewWallet, TowerCardPanel, TowerPlacer, WaveReward |
| `Amount` | Property | `int` | `{ get; private set; }` | How much Honeydew is held. | HoneydewLabel, HoneydewTests, TowerCardPanel, TowerPlacer |
| `Earn` | Method | `void` | `(int amount)` | Adds Honeydew. Nothing happens for zero or less. | Bounty, DevKeys, HoneydewTests, TowerPlacer, WaveReward |
| `CanAfford` | Method | `bool` | `(int amount)` | True if at least amount is held. | HoneydewTests, TowerPlacer |
| `TrySpend` | Method | `bool` | `(int amount)` | Takes Honeydew if enough is held. Zero or less always succeeds and changes nothing. | HoneydewTests, TowerPlacer |
| `Set` | Method | `void` | `(int amount)` | Puts the amount at an exact value, never below zero. For run start and restarts. | HoneydewTests, HoneydewWallet |

### HoneydewWallet

`unity/Assets/_Game/Features/Economy/Scripts/HoneydewWallet.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Economy/Scripts/HoneydewWallet.cs)) · MonoBehaviour · `HoldTheHill.Features.Economy` · HoldTheHill.Runtime · owner: Nilly

Holds the run's Honeydew in the gameplay scene. One per level. Towers spend from it, bounties and wave rewards pay into it, the HUD listens to it.

Derives from / implements: MonoBehaviour

Used on: Hill01_Gameplay, L00_TutorialHill_Gameplay, L01_Meadow_Gameplay

Named in: AlphaCoreLoopBuilder, AlphaRunFrameBuilder, Bounty, LevelRun, TowerPlacer, WaveReward

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_startingHoneydew` | `int` | `300` |  | Hill01_Gameplay = 300; L00_TutorialHill_Gameplay = 300; L01_Meadow_Gameplay = 300 | Honeydew the player starts the level with. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Honeydew` | Property | `Honeydew` | `=> _honeydew` | The counter itself. Created in Awake, so read it from Start or later. | Bounty, TowerPlacer, WaveReward |
| `ResetToStart` | Method | `void` | `()` | Puts the amount back to the starting value, for a level restart. |  |

Private members not listed: 2.

### WaveReward

`unity/Assets/_Game/Features/Economy/Scripts/WaveReward.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Economy/Scripts/WaveReward.cs)) · MonoBehaviour · `HoldTheHill.Features.Economy` · HoldTheHill.Runtime · owner: Nilly

Pays a flat Honeydew reward each time the spawner finishes a wave. Lives next to the HoneydewWallet in the gameplay scene.

Derives from / implements: MonoBehaviour

Used on: Hill01_Gameplay, L00_TutorialHill_Gameplay, L01_Meadow_Gameplay

Named in: AlphaCoreLoopBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_spawner` | `EnemySpawner` |  |  | Hill01_Gameplay = Enemy Spawner (EnemySpawner); L00_TutorialHill_Gameplay = Enemy Spawner (EnemySpawner); L01_Meadow_Gameplay = Enemy Spawner (EnemySpawner) | The level's spawner. Its wave-completed event triggers the reward. |
| `_honeydewPerWave` | `int` | `50` |  | Hill01_Gameplay = 50; L00_TutorialHill_Gameplay = 50; L01_Meadow_Gameplay = 50 | Honeydew paid when a wave has finished spawning. |

Private members not listed: 5.
