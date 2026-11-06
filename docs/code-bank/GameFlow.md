# GameFlow

Part of the [code bank](README.md), built 2026-11-05 from commit `d6944f7b`.

## Scripts

| Script | Kind | Owner | What it does |
|---|---|---|---|
| [DevKeys](#devkeys) | MonoBehaviour | Nilly | Test keys for the Editor and development builds; a release build turns them off. F5 sends a centipede down the last stretch of road to the Queen, F6 adds Honeyd… |
| [LevelRun](#levelrun) | MonoBehaviour | Nilly | Where a level's run comes together: the Queen, the spawner and the wallet, the pause and speed RunClock, and the win or lose decision. Lives in the level's game… |
| [NextWaveRule](#nextwaverule) | Static class | Nilly | When the player may send the next wave, as a pure function so EditMode tests cover it: only while the run is on and not paused, and only when the spawner is wai… |
| [Queen](#queen) | MonoBehaviour | Nilly + Jason | Represents the Queen object (Ant Queen), which acts as the core health pool for the player / hill. Reduces health by enemy damage when a BasicEnemy reaches its … |
| [RunClock](#runclock) | Class | Nilly | Pause and speed of a run, as plain C#. TimeScale is what the scene writes to Time.timeScale: zero while paused, otherwise the normal or fast speed. Pausing keep… |
| [RunControls](#runcontrols) | MonoBehaviour | Nilly | Reads the run keys from the game's actions asset and hands them to the LevelRun beside it: NextWave sends a wave, ToggleSpeed switches 1x / 2x, Pause opens or c… |
| [RunOutcome](#runoutcome) | Enum | Nilly | How a level run stands: still going, won or lost. |
| [RunOutcomeRule](#runoutcomerule) | Static class | Nilly | The win and lose rule of a level, as a pure function so EditMode tests cover it. Lost when the Queen is beaten; won when every wave has been sent and no enemy i… |

### DevKeys

`unity/Assets/_Game/Features/GameFlow/Scripts/DevKeys.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/GameFlow/Scripts/DevKeys.cs)) · MonoBehaviour · `HoldTheHill.Features.GameFlow` · HoldTheHill.Runtime · owner: Nilly

Test keys for the Editor and development builds; a release build turns them off. F5 sends a centipede down the last stretch of road to the Queen, F6 adds Honeyd…

Derives from / implements: MonoBehaviour

Used on: UI

Named in: AlphaPlaytestToolsBuilder, QueenMantisBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_centipedePrefab` | `BasicEnemy` |  |  | UI = Enemy_Centipede › CentipedeEnemy | What F5 sends at the Queen. |
| `_queenMantisPrefab` | `BasicEnemy` |  |  | UI = Enemy_QueenMantis › QueenMantisBoss | What F8 sends down the road. |
| `_honeydewGift` | `int` | `500` |  | UI = 500 | How much Honeydew F6 adds. |

Private members not listed: 5.

### LevelRun

`unity/Assets/_Game/Features/GameFlow/Scripts/LevelRun.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/GameFlow/Scripts/LevelRun.cs)) · MonoBehaviour · `HoldTheHill.Features.GameFlow` · HoldTheHill.Runtime · owner: Nilly

Where a level's run comes together: the Queen, the spawner and the wallet, the pause and speed RunClock, and the win or lose decision. Lives in the level's game…

Derives from / implements: MonoBehaviour

Used on: Hill01_Gameplay, L00_TutorialHill_Gameplay, L01_Meadow_Gameplay

Named in: AlphaCoreLoopBuilder, AlphaPowerUpsBuilder, AlphaRunFrameBuilder, AlphaTowersBuilder, DevKeys, HoneydewLabel, HudPanel, LevelRunListener, NextWaveButton, PauseMenu, PowerUpSlotPanel, QueenHealthBar, ResultPanel, RunControls, ShopPlacerBinding, TowerCardPanel, WaveLabel

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_queen` | `Queen` |  | Level | Hill01_Gameplay = Queen (Queen); L00_TutorialHill_Gameplay = Queen (Queen); L01_Meadow_Gameplay = Queen (Queen) | The hill's health pool. Its Game Over loses the run. |
| `_spawner` | `EnemySpawner` |  | Level | Hill01_Gameplay = Enemy Spawner (EnemySpawner); L00_TutorialHill_Gameplay = Enemy Spawner (EnemySpawner); L01_Meadow_Gameplay = Enemy Spawner (EnemySpawner) | The level's spawner. The run is won when it has sent every wave and no enemy is left. |
| `_wallet` | `HoneydewWallet` |  | Level | Hill01_Gameplay = Honeydew Wallet (HoneydewWallet); L00_TutorialHill_Gameplay = Honeydew Wallet (HoneydewWallet); L01_Meadow_Gameplay = Honeydew Wallet (Honeyde… | The run's Honeydew, shown on the HUD. |
| `_placer` | `TowerPlacer` |  | Level | Hill01_Gameplay = Tower Placer (TowerPlacer); L00_TutorialHill_Gameplay = Tower Placer (TowerPlacer); L01_Meadow_Gameplay = Tower Placer (TowerPlacer) | Places and selects towers; the tower card and the U key reach it through the run. |
| `_powerUps` | `PowerUps.LevelPowerUps` |  | Level | Hill01_Gameplay = (none); L00_TutorialHill_Gameplay = Power Ups (LevelPowerUps); L01_Meadow_Gameplay = Power Ups (LevelPowerUps) | Drops, the slot and the two power-ups; the HUD slot reaches it through the run. |
| `_fastTimeScale` | `float` | `2f` | Speed | Hill01_Gameplay = 2; L00_TutorialHill_Gameplay = 2; L01_Meadow_Gameplay = 2 | Time scale while the fast speed is on. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Current` | Property (static) | `LevelRun` | `=> s_current` | The run in progress, or null between levels. | DevKeys, LevelRunListener |
| `Started` | Event (static) | `Action<LevelRun>` |  | Raised when a run is ready: its scene objects exist and their Awake has run. | LevelRunListener |
| `Ended` | Event (static) | `Action<LevelRun>` |  | Raised when the current run goes away (the level scene unloads). | LevelRunListener |
| `Finished` | Event | `Action<RunOutcome>` |  | Raised once, when the run is won or lost. | PauseMenu, ResultPanel |
| `Queen` | Property | `Queen` | `=> _queen` | The hill's health pool. | QueenHealthBar |
| `Spawner` | Property | `EnemySpawner` | `=> _spawner` | The level's spawner. | WaveLabel |
| `Wallet` | Property | `HoneydewWallet` | `=> _wallet` | The run's Honeydew. | DevKeys, HoneydewLabel, TowerCardPanel |
| `Placer` | Property | `TowerPlacer` | `=> _placer` | The level's tower placer, for the tower card. May be null in a scene without one. | ShopPlacerBinding, TowerCardPanel |
| `PowerUps` | Property | `PowerUps.LevelPowerUps` | `=> _powerUps` | The level's power-ups, for the HUD slot. May be null in a scene without them. | AlphaPowerUpsBuilder, PowerUpSlotPanel |
| `Clock` | Property | `RunClock` | `{ get; private set; }` | Pause and speed. Created in Awake. | PauseMenu |
| `Outcome` | Property | `RunOutcome` | `{ get; private set; }` | Playing until the run is won or lost. | DevKeys, PauseMenu, ResultPanel |
| `CanSendNextWave` | Property | `bool` | `=> _spawner != null && NextWaveRule.CanSend(Outcome, Clock.Paused, _spawner.CurrentState)` | True when a wave can be sent now: the run is on and not paused, and the spawner is waiting for the next wave (not still sending one, not done). The Next Wave bu… | NextWaveButton |
| `StartNextWave` | Method | `void` | `()` | Sends the next wave. Does nothing while paused, while a wave is still coming out, after the last wave, or once the run is over. | AlphaCoreLoopBuilder, DevKeys, NextWaveButton, RunControls |
| `TogglePause` | Method | `void` | `()` | Pauses, or resumes when paused. Does nothing once the run is over. | RunControls |
| `Resume` | Method | `void` | `()` | Resumes from the pause menu. | AlphaRunFrameBuilder, PauseMenu |
| `ToggleSpeed` | Method | `void` | `()` | Switches between normal and fast speed. | RunControls |
| `UpgradeSelectedTower` | Method | `void` | `()` | Buys the next level of the selected tower, if any. The U key and the card's Upgrade button come here. | RunControls, TowerCardPanel |
| `SellSelectedTower` | Method | `void` | `()` | Sells the selected tower, if any. The card's Sell button comes here. | TowerCardPanel |
| `ToggleBurrowSelectedTower` | Method | `void` | `()` | Burrows the selected tower, or brings it back up. The card's Burrow button comes here. | TowerCardPanel |
| `ToggleBurrowAll` | Method | `void` | `()` | The B key: burrows every tower, or brings them all up when every one is already down. | RunControls |
| `UsePowerUp` | Method | `void` | `()` | Uses the power-up in the HUD slot, if any. The slot button comes here. | PowerUpSlotPanel |
| `RestartLevel` | Method | `void` | `()` | Reloads this level from the start. | AlphaRunFrameBuilder, PauseMenu, ResultPanel |
| `GoHome` | Method | `void` | `()` | Leaves the level for the home screen. | PauseMenu, ResultPanel |

Private members not listed: 11.

### NextWaveRule

`unity/Assets/_Game/Features/GameFlow/Scripts/NextWaveRule.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/GameFlow/Scripts/NextWaveRule.cs)) · Static class · `HoldTheHill.Features.GameFlow` · HoldTheHill.Runtime · owner: Nilly

When the player may send the next wave, as a pure function so EditMode tests cover it: only while the run is on and not paused, and only when the spawner is wai…

Named in: LevelRun, NextWaveRuleTests

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `CanSend` | Method (static) | `bool` | `(RunOutcome outcome, bool paused, EnemySpawner.SpawnerState spawnerState)` |  | LevelRun, NextWaveRuleTests |

### Queen

`unity/Assets/_Game/Features/GameFlow/Scripts/Queen.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/GameFlow/Scripts/Queen.cs)) · MonoBehaviour · `HoldTheHill.Features.GameFlow` · HoldTheHill.Runtime · owner: Nilly + Jason

Represents the Queen object (Ant Queen), which acts as the core health pool for the player / hill. Reduces health by enemy damage when a BasicEnemy reaches its …

Derives from / implements: MonoBehaviour

Used on: Hill01_Gameplay, L00_TutorialHill_Gameplay, L01_Meadow_Gameplay, Path and Queen Test

Named in: AlphaHomeAndLevelsBuilder, AlphaRunFrameBuilder, BossDeathLogTests, LevelRun, QueenHealthBar, QueenTests

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_maxHealth` | `double` | `100.0` | Health Settings | Hill01_Gameplay = 100; L00_TutorialHill_Gameplay = 100; L01_Meadow_Gameplay = 100; Path and Queen Test = 100 | Maximum health pool of the Queen as a double. |
| `_autoListenToEnemyEvents` | `bool` | `true` | Health Settings | Hill01_Gameplay = 1; L00_TutorialHill_Gameplay = 1; L01_Meadow_Gameplay = 1; Path and Queen Test = 1 | If true, automatically listens to all BasicEnemy instances reaching their destination via global static event. |
| `_onGameOver` | `UnityEvent` | `new UnityEvent()` | Events | Hill01_Gameplay = 0 listener(s); L00_TutorialHill_Gameplay = 0 listener(s); L01_Meadow_Gameplay = 0 listener(s); Path and Queen Test = 0 listener(s) | UnityEvent invoked when the Queen's health drops to 0 or below (Game Over). |
| `_onHealthChanged` | `UnityEvent<double, double>` | `new UnityEvent<double, double>()` | Events | Hill01_Gameplay = 0 listener(s); L00_TutorialHill_Gameplay = 0 listener(s); L01_Meadow_Gameplay = 0 listener(s); Path and Queen Test = 0 listener(s) | UnityEvent invoked whenever the Queen's health changes. Passes (currentHealth, maxHealth). |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `OnGameOver` | Event | `Action` |  |  | LevelRun, QueenTests |
| `OnAnyGameOver` | Event (static) | `Action` |  |  |  |
| `OnHealthChanged` | Event | `Action<double, double>` |  |  | QueenHealthBar, QueenTests |
| `MaxHealth` | Property | `double` | `=> _maxHealth` | Maximum health pool value as a double. | QueenHealthBar, QueenTests |
| `CurrentHealth` | Property | `double` | `=> _currentHealth` | Current health value as a double. | QueenHealthBar, QueenTests |
| `IsGameOver` | Property | `bool` | `=> _isGameOver` | True if health has depleted to 0 or below and the Game Over event has fired. | QueenTests |
| `OnGameOverEvent` | Property | `UnityEvent` | `=> _onGameOver` | UnityEvent for Game Over (for inspector wiring). |  |
| `OnHealthChangedEvent` | Property | `UnityEvent<double, double>` | `=> _onHealthChanged` | UnityEvent for health change (for inspector wiring). |  |
| `Awake` | Method (protected virtual) | `void` | `()` |  | LevelRun |
| `OnEnable` | Method (protected virtual) | `void` | `()` |  | LevelRun |
| `OnDisable` | Method (protected virtual) | `void` | `()` |  | LevelRun |
| `HandleEnemyReachedDestination` | Method (virtual) | `void` | `(BasicEnemy enemy)` | Handler for when an enemy reaches the Queen's location (the end of the path). Subtracts the enemy's damage value from the Queen's health pool. | QueenTests |
| `TakeDamage` | Method (virtual) | `void` | `(double damageAmount)` | Subtracts damage from the Queen's health pool. If health reaches or drops below 0, triggers the Game Over event. | QueenTests |
| `Heal` | Method (virtual) | `void` | `(double healAmount)` | Restores health up to the maximum health limit. | QueenTests |
| `ResetHealth` | Method (virtual) | `void` | `()` | Resets the Queen's health back to maximum and clears the Game Over state. | QueenTests |
| `SetMaxHealth` | Method | `void` | `(double newMaxHealth, bool resetCurrentHealth = false)` | Updates the maximum health pool and optionally clamps/resets current health. |  |
| `RegisterEnemy` | Method | `void` | `(BasicEnemy enemy)` | Explicitly subscribes to a specific enemy's destination reached event. Useful when auto-listening is disabled or for manual spawner wiring. |  |
| `UnregisterEnemy` | Method | `void` | `(BasicEnemy enemy)` | Explicitly unsubscribes from a specific enemy's destination reached event. |  |
| `TriggerGameOver` | Method (protected virtual) | `void` | `()` | Executes the Game Over sequence once when health is depleted. |  |

Private members not listed: 2.

### RunClock

`unity/Assets/_Game/Features/GameFlow/Scripts/RunClock.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/GameFlow/Scripts/RunClock.cs)) · Class · `HoldTheHill.Features.GameFlow` · HoldTheHill.Runtime · owner: Nilly

Pause and speed of a run, as plain C#. TimeScale is what the scene writes to Time.timeScale: zero while paused, otherwise the normal or fast speed. Pausing keep…

Named in: LevelRun, RunClockTests

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Changed` | Event | `Action` |  | Raised when pause or speed changes. | LevelRun, RunClockTests |
| `RunClock` | Constructor |  | `(float fastScale)` |  | LevelRun, RunClockTests |
| `FastScale` | Property | `float` | `{ get; }` | Time scale of the fast speed. | RunClockTests |
| `Paused` | Property | `bool` | `{ get; private set; }` | True while the run is paused. | LevelRun, RunClockTests |
| `Fast` | Property | `bool` | `{ get; private set; }` | True while the run plays at the fast speed. | RunClockTests |
| `TimeScale` | Property | `float` | `=> Paused ? 0f : Fast ? FastScale : 1f` | Zero while paused, otherwise 1 or FastScale. | LevelRun, RunClockTests |
| `SetPaused` | Method | `void` | `(bool paused)` | Pauses or resumes. | LevelRun, RunClockTests |
| `TogglePause` | Method | `void` | `()` | Resumes when paused, pauses when running. | LevelRun, RunClockTests |
| `ToggleSpeed` | Method | `void` | `()` | Switches between normal and fast speed. Works while paused; it shows on resume. | LevelRun, RunClockTests |

### RunControls

`unity/Assets/_Game/Features/GameFlow/Scripts/RunControls.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/GameFlow/Scripts/RunControls.cs)) · MonoBehaviour · `HoldTheHill.Features.GameFlow` · HoldTheHill.Runtime · owner: Nilly

Reads the run keys from the game's actions asset and hands them to the LevelRun beside it: NextWave sends a wave, ToggleSpeed switches 1x / 2x, Pause opens or c…

Derives from / implements: MonoBehaviour

Used on: Hill01_Gameplay, L00_TutorialHill_Gameplay, L01_Meadow_Gameplay

Named in: AlphaRunFrameBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_actions` | `InputActionAsset` |  |  | Hill01_Gameplay = HoldTheHill; L00_TutorialHill_Gameplay = HoldTheHill; L01_Meadow_Gameplay = HoldTheHill | The game's actions asset. Uses its Gameplay map: NextWave, ToggleSpeed, Pause, Upgrade, Burrow. |

Private members not listed: 11.

### RunOutcome

`unity/Assets/_Game/Features/GameFlow/Scripts/RunOutcome.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/GameFlow/Scripts/RunOutcome.cs)) · Enum · `HoldTheHill.Features.GameFlow` · HoldTheHill.Runtime · owner: Nilly

How a level run stands: still going, won or lost.

Named in: DevKeys, LevelRun, NextWaveRule, NextWaveRuleTests, PauseMenu, ResultPanel, RunOutcomeRule, RunOutcomeRuleTests

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Playing` | Enum member (static) | `RunOutcome` |  |  | DevKeys, LevelRun, NextWaveRule, NextWaveRuleTests, PauseMenu, ResultPanel, RunOutcomeRule, RunOutcomeRuleTests |
| `Won` | Enum member (static) | `RunOutcome` |  |  | NextWaveRuleTests, ResultPanel, RunOutcomeRule, RunOutcomeRuleTests |
| `Lost` | Enum member (static) | `RunOutcome` |  |  | NextWaveRuleTests, RunOutcomeRule, RunOutcomeRuleTests |

### RunOutcomeRule

`unity/Assets/_Game/Features/GameFlow/Scripts/RunOutcomeRule.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/GameFlow/Scripts/RunOutcomeRule.cs)) · Static class · `HoldTheHill.Features.GameFlow` · HoldTheHill.Runtime · owner: Nilly

The win and lose rule of a level, as a pure function so EditMode tests cover it. Lost when the Queen is beaten; won when every wave has been sent and no enemy i…

Named in: LevelRun, RunOutcomeRuleTests

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Decide` | Method (static) | `RunOutcome` | `(bool queenDefeated, bool allWavesSent, int livingEnemies)` |  | LevelRun, RunOutcomeRuleTests |
