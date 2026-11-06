# Scenes

Part of the [code bank](README.md), built 2026-11-05 from commit `d6944f7b`.

| Scene | Kind | Build Settings | Scripts present | Prefab instances |
|---|---|---|---|---|
| [Bootstrap](#bootstrap) | Bootstrap | index 0 | SceneLoader | 0 |
| [Home](#home) | Home | index 6 | HomeScreen, LevelButton x2 | 0 |
| [Hill01_Gameplay](#hill01_gameplay) | Gameplay | listed, disabled | TowerPlacer, RunControls, LevelRun, EnemySpawner, Queen, EnemyPath, HoneydewWallet, WaveReward | 0 |
| [Hill01_Terrain](#hill01_terrain) | Terrain | listed, disabled |  | 0 |
| [L00_TutorialHill_Gameplay](#l00_tutorialhill_gameplay) | Gameplay | index 3 | TowerPlacer, LevelCameraFit, RunControls, LevelRun, EnemySpawner, Queen, EnemyPath, LevelPowerUps, HoneydewWallet, WaveReward, SilverfishClickReader, HazardSche… | 0 |
| [L00_TutorialHill_Terrain](#l00_tutorialhill_terrain) | Terrain | index 2 |  | 0 |
| [L01_Meadow_Gameplay](#l01_meadow_gameplay) | Gameplay | index 5 | TowerPlacer, LevelCameraFit, RunControls, LevelRun, EnemySpawner, Queen, EnemyPath, LevelPowerUps, HoneydewWallet, WaveReward, SilverfishClickReader, HazardSche… | 0 |
| [L01_Meadow_Terrain](#l01_meadow_terrain) | Terrain | index 4 |  | 0 |
| [UI](#ui) | UI | index 1 | ShopPlacerBinding, HoneydewLabel x2, PowerUpSlotPanel, PauseMenu, WaveLabel, QueenMantisBossBar, HudPanel, QueenHealthBar, TowerCardPanel, ControlsOverlay, Next… | 1 |
| [New Scene](#new-scene) | Sandbox | not listed |  | 1 |
| [UITest](#uitest) | Sandbox | not listed |  | 1 |
| [Path and Queen Test](#path-and-queen-test) | Sandbox | not listed | EnemySpawner, Queen, EnemyPath | 0 |

### Bootstrap

`unity/Assets/_Game/Scenes/Bootstrap.unity` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Scenes/Bootstrap.unity)) · Scene

Build Settings: index 0

**SceneLoader** · SceneLoader

| Field | Value | Points to |
|---|---|---|
| `_persistentScenes` | 1 item(s) |  |
| `_persistentScenes[0]` | UI |  |
| `_homeScene` | Home |  |

### Home

`unity/Assets/_Game/Scenes/Home.unity` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Scenes/Home.unity)) · Scene

Build Settings: index 6

**Canvas/Home Screen** · HomeScreen

| Field | Value | Points to |
|---|---|---|
| `_titlePage` | Canvas/Home Screen/Title Page | GameObject: unity/Assets/_Game/Scenes/Home.unity |
| `_levelSelectPage` | Canvas/Home Screen/Level Select Page | GameObject: unity/Assets/_Game/Scenes/Home.unity |
| `_play` | Canvas/Home Screen/Title Page/Play (Button) | Component: unity/Assets/_Game/Scenes/Home.unity |
| `_quit` | Canvas/Home Screen/Title Page/Quit (Button) | Component: unity/Assets/_Game/Scenes/Home.unity |
| `_back` | Canvas/Home Screen/Level Select Page/Back (Button) | Component: unity/Assets/_Game/Scenes/Home.unity |

**Canvas/Home Screen/Level Select Page/L01_Meadow** · LevelButton

| Field | Value | Points to |
|---|---|---|
| `_levelId` | L01_Meadow |  |

**Canvas/Home Screen/Level Select Page/L00_TutorialHill** · LevelButton

| Field | Value | Points to |
|---|---|---|
| `_levelId` | L00_TutorialHill |  |

### Hill01_Gameplay

`unity/Assets/_Game/Scenes/Levels/Hill01/Hill01_Gameplay.unity` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Scenes/Levels/Hill01/Hill01_Gameplay.unity)) · Scene

Build Settings: listed, disabled

**Tower Placer** · TowerPlacer

| Field | Value | Points to |
|---|---|---|
| `_cellSize` | 1 |  |
| `_path` | Enemy Path (EnemyPath) | Component: unity/Assets/_Game/Scenes/Levels/Hill01/Hill01_Gameplay.unity |
| `_wallet` | Honeydew Wallet (HoneydewWallet) | Component: unity/Assets/_Game/Scenes/Levels/Hill01/Hill01_Gameplay.unity |
| `_offers` | 6 item(s) |  |
| `_offers[0].Data` | Ant | Data asset: unity/Assets/_Game/Features/Towers/Data/Ant.asset |
| `_offers[0].Prefab` | Tower_Ant › Tower | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_Ant.prefab |
| `_offers[1].Data` | BulletAnt | Data asset: unity/Assets/_Game/Features/Towers/Data/BulletAnt.asset |
| `_offers[1].Prefab` | Tower_BulletAnt › Tower | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_BulletAnt.prefab |
| `_offers[2].Data` | CrazyAnt | Data asset: unity/Assets/_Game/Features/Towers/Data/CrazyAnt.asset |
| `_offers[2].Prefab` | Tower_CrazyAnt › Tower | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_CrazyAnt.prefab |
| `_offers[3].Data` | FireAnt | Data asset: unity/Assets/_Game/Features/Towers/Data/FireAnt.asset |
| `_offers[3].Prefab` | Tower_FireAnt › Tower | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_FireAnt.prefab |
| `_offers[4].Data` | SapAnt | Data asset: unity/Assets/_Game/Features/Towers/Data/SapAnt.asset |
| `_offers[4].Prefab` | Tower_SapAnt › Tower | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_SapAnt.prefab |
| `_offers[5].Data` | CarpenterAnt | Data asset: unity/Assets/_Game/Features/Towers/Data/CarpenterAnt.asset |
| `_offers[5].Prefab` | Tower_CarpenterAnt › Tower | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_CarpenterAnt.prefab |
| `_sellRefund` | 0.75 |  |
| `_actions` | HoldTheHill | Input actions: unity/Assets/_Game/Input/HoldTheHill.inputactions |
| `_freeCellColor` | (1, 1, 1, 0.35) |  |
| `_blockedCellColor` | (1, 0.3, 0.3, 0.35) |  |
| `_unaffordableColor` | (1, 0.8, 0.2, 0.35) |  |
| `_selectionColor` | (0.4, 0.9, 1, 0.45) |  |

**Level Run** · RunControls

| Field | Value | Points to |
|---|---|---|
| `_actions` | HoldTheHill | Input actions: unity/Assets/_Game/Input/HoldTheHill.inputactions |

**Level Run** · LevelRun

| Field | Value | Points to |
|---|---|---|
| `_queen` | Queen (Queen) | Component: unity/Assets/_Game/Scenes/Levels/Hill01/Hill01_Gameplay.unity |
| `_spawner` | Enemy Spawner (EnemySpawner) | Component: unity/Assets/_Game/Scenes/Levels/Hill01/Hill01_Gameplay.unity |
| `_wallet` | Honeydew Wallet (HoneydewWallet) | Component: unity/Assets/_Game/Scenes/Levels/Hill01/Hill01_Gameplay.unity |
| `_placer` | Tower Placer (TowerPlacer) | Component: unity/Assets/_Game/Scenes/Levels/Hill01/Hill01_Gameplay.unity |
| `_powerUps` | (none) |  |
| `_fastTimeScale` | 2 |  |

**Enemy Spawner** · EnemySpawner

| Field | Value | Points to |
|---|---|---|
| `defaultSpawnPoint` | Enemy Path/Waypoint 0 (Transform) | Component: unity/Assets/_Game/Scenes/Levels/Hill01/Hill01_Gameplay.unity |
| `enemyContainer` | (none) |  |
| `activeMapId` | Map_01 |  |
| `autoDetectMapFromScene` | 0 |  |
| `activeMapConfigOverride` | Hill01_Waves | Data asset: unity/Assets/_Game/Features/Enemies/Data/Hill01_Waves.asset |
| `scriptableMapConfigs` | 0 item(s) |  |
| `inlineMapConfigs` | 0 item(s) |  |
| `autoStartFirstWave` | 0 |  |
| `requireEnemiesClearedBeforeNextWave` | 0 |  |
| `onEnemySpawned` | 0 listener(s) |  |
| `onWaveStarted` | 0 listener(s) |  |
| `onWaveCompleted` | 0 listener(s) |  |
| `onAllWavesCompleted` | 0 listener(s) |  |
| `onMapChanged` | 0 listener(s) |  |

**Queen** · Queen

| Field | Value | Points to |
|---|---|---|
| `_maxHealth` | 100 |  |
| `_autoListenToEnemyEvents` | 1 |  |
| `_onGameOver` | 0 listener(s) |  |
| `_onHealthChanged` | 0 listener(s) |  |

**Enemy Path** · EnemyPath

| Field | Value | Points to |
|---|---|---|
| `_roadWidth` | 1 |  |
| `_roadColor` | (0.55, 0.42, 0.3, 1) |  |

**Honeydew Wallet** · HoneydewWallet

| Field | Value | Points to |
|---|---|---|
| `_startingHoneydew` | 300 |  |

**Honeydew Wallet** · WaveReward

| Field | Value | Points to |
|---|---|---|
| `_spawner` | Enemy Spawner (EnemySpawner) | Component: unity/Assets/_Game/Scenes/Levels/Hill01/Hill01_Gameplay.unity |
| `_honeydewPerWave` | 50 |  |

### Hill01_Terrain

`unity/Assets/_Game/Scenes/Levels/Hill01/Hill01_Terrain.unity` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Scenes/Levels/Hill01/Hill01_Terrain.unity)) · Scene

Build Settings: listed, disabled

### L00_TutorialHill_Gameplay

`unity/Assets/_Game/Scenes/Levels/L00_TutorialHill/L00_TutorialHill_Gameplay.unity` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Scenes/Levels/L00_TutorialHill/L00_TutorialHill_Gameplay.unity)) · Scene

Build Settings: index 3

**Tower Placer** · TowerPlacer

| Field | Value | Points to |
|---|---|---|
| `_cellSize` | 1 |  |
| `_buildArea.x` | -10 |  |
| `_buildArea.y` | -5 |  |
| `_buildArea.width` | 20 |  |
| `_buildArea.height` | 10 |  |
| `_path` | Enemy Path (EnemyPath) | Component: unity/Assets/_Game/Scenes/Levels/L00_TutorialHill/L00_TutorialHill_Gameplay.unity |
| `_wallet` | Honeydew Wallet (HoneydewWallet) | Component: unity/Assets/_Game/Scenes/Levels/L00_TutorialHill/L00_TutorialHill_Gameplay.unity |
| `_offers` | 6 item(s) |  |
| `_offers[0].Data` | Ant | Data asset: unity/Assets/_Game/Features/Towers/Data/Ant.asset |
| `_offers[0].Prefab` | Tower_Ant › Tower | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_Ant.prefab |
| `_offers[1].Data` | BulletAnt | Data asset: unity/Assets/_Game/Features/Towers/Data/BulletAnt.asset |
| `_offers[1].Prefab` | Tower_BulletAnt › Tower | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_BulletAnt.prefab |
| `_offers[2].Data` | CrazyAnt | Data asset: unity/Assets/_Game/Features/Towers/Data/CrazyAnt.asset |
| `_offers[2].Prefab` | Tower_CrazyAnt › Tower | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_CrazyAnt.prefab |
| `_offers[3].Data` | FireAnt | Data asset: unity/Assets/_Game/Features/Towers/Data/FireAnt.asset |
| `_offers[3].Prefab` | Tower_FireAnt › Tower | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_FireAnt.prefab |
| `_offers[4].Data` | SapAnt | Data asset: unity/Assets/_Game/Features/Towers/Data/SapAnt.asset |
| `_offers[4].Prefab` | Tower_SapAnt › Tower | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_SapAnt.prefab |
| `_offers[5].Data` | CarpenterAnt | Data asset: unity/Assets/_Game/Features/Towers/Data/CarpenterAnt.asset |
| `_offers[5].Prefab` | Tower_CarpenterAnt › Tower | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_CarpenterAnt.prefab |
| `_sellRefund` | 0.75 |  |
| `_actions` | HoldTheHill | Input actions: unity/Assets/_Game/Input/HoldTheHill.inputactions |
| `_freeCellColor` | (1, 1, 1, 0.35) |  |
| `_blockedCellColor` | (1, 0.3, 0.3, 0.35) |  |
| `_unaffordableColor` | (1, 0.8, 0.2, 0.35) |  |
| `_selectionColor` | (0.4, 0.9, 1, 0.45) |  |

**Main Camera** · LevelCameraFit

| Field | Value | Points to |
|---|---|---|
| `_halfWidth` | 10 |  |
| `_halfHeight` | 5 |  |
| `_bottomBand` | 0.09 |  |
| `_topBand` | 0.15 |  |
| `_rightBand` | 0.3 |  |

**Level Run** · RunControls

| Field | Value | Points to |
|---|---|---|
| `_actions` | HoldTheHill | Input actions: unity/Assets/_Game/Input/HoldTheHill.inputactions |

**Level Run** · LevelRun

| Field | Value | Points to |
|---|---|---|
| `_queen` | Queen (Queen) | Component: unity/Assets/_Game/Scenes/Levels/L00_TutorialHill/L00_TutorialHill_Gameplay.unity |
| `_spawner` | Enemy Spawner (EnemySpawner) | Component: unity/Assets/_Game/Scenes/Levels/L00_TutorialHill/L00_TutorialHill_Gameplay.unity |
| `_wallet` | Honeydew Wallet (HoneydewWallet) | Component: unity/Assets/_Game/Scenes/Levels/L00_TutorialHill/L00_TutorialHill_Gameplay.unity |
| `_placer` | Tower Placer (TowerPlacer) | Component: unity/Assets/_Game/Scenes/Levels/L00_TutorialHill/L00_TutorialHill_Gameplay.unity |
| `_powerUps` | Power Ups (LevelPowerUps) | Component: unity/Assets/_Game/Scenes/Levels/L00_TutorialHill/L00_TutorialHill_Gameplay.unity |
| `_fastTimeScale` | 2 |  |

**Enemy Spawner** · EnemySpawner

| Field | Value | Points to |
|---|---|---|
| `defaultSpawnPoint` | Enemy Path/Waypoint 0 (Transform) | Component: unity/Assets/_Game/Scenes/Levels/L00_TutorialHill/L00_TutorialHill_Gameplay.unity |
| `enemyContainer` | (none) |  |
| `activeMapId` | Map_01 |  |
| `autoDetectMapFromScene` | 0 |  |
| `activeMapConfigOverride` | L00_TutorialHill_Waves | Data asset: unity/Assets/_Game/Features/Enemies/Data/L00_TutorialHill_Waves.asset |
| `scriptableMapConfigs` | 0 item(s) |  |
| `inlineMapConfigs` | 0 item(s) |  |
| `autoStartFirstWave` | 0 |  |
| `requireEnemiesClearedBeforeNextWave` | 0 |  |
| `onEnemySpawned` | 0 listener(s) |  |
| `onWaveStarted` | 0 listener(s) |  |
| `onWaveCompleted` | 0 listener(s) |  |
| `onAllWavesCompleted` | 0 listener(s) |  |
| `onMapChanged` | 0 listener(s) |  |

**Queen** · Queen

| Field | Value | Points to |
|---|---|---|
| `_maxHealth` | 100 |  |
| `_autoListenToEnemyEvents` | 1 |  |
| `_onGameOver` | 0 listener(s) |  |
| `_onHealthChanged` | 0 listener(s) |  |

**Enemy Path** · EnemyPath

| Field | Value | Points to |
|---|---|---|
| `_roadWidth` | 1 |  |
| `_roadColor` | (0.55, 0.42, 0.3, 1) |  |

**Power Ups** · LevelPowerUps

| Field | Value | Points to |
|---|---|---|
| `_settings` | PowerUpSettings | Data asset: unity/Assets/_Game/Features/PowerUps/Data/PowerUpSettings.asset |
| `_pickupPrefab` | PowerUp_Pickup › PowerUpPickup | Component: unity/Assets/_Game/Features/PowerUps/Prefabs/PowerUp_Pickup.prefab |
| `_placer` | Tower Placer (TowerPlacer) | Component: unity/Assets/_Game/Scenes/Levels/L00_TutorialHill/L00_TutorialHill_Gameplay.unity |
| `_actions` | HoldTheHill | Input actions: unity/Assets/_Game/Input/HoldTheHill.inputactions |

**Honeydew Wallet** · HoneydewWallet

| Field | Value | Points to |
|---|---|---|
| `_startingHoneydew` | 300 |  |

**Honeydew Wallet** · WaveReward

| Field | Value | Points to |
|---|---|---|
| `_spawner` | Enemy Spawner (EnemySpawner) | Component: unity/Assets/_Game/Scenes/Levels/L00_TutorialHill/L00_TutorialHill_Gameplay.unity |
| `_honeydewPerWave` | 50 |  |

**Silverfish Click Reader** · SilverfishClickReader

| Field | Value | Points to |
|---|---|---|
| `_actions` | HoldTheHill | Input actions: unity/Assets/_Game/Input/HoldTheHill.inputactions |
| `_fallbackRadius` | 0.4 |  |

**Hazard Scheduler** · HazardScheduler

| Field | Value | Points to |
|---|---|---|
| `_spawner` | Enemy Spawner (EnemySpawner) | Component: unity/Assets/_Game/Scenes/Levels/L00_TutorialHill/L00_TutorialHill_Gameplay.unity |
| `_schedule` | L00_TutorialHill_Hazards | Data asset: unity/Assets/_Game/Features/Hazards/Data/L00_TutorialHill_Hazards.asset |

### L00_TutorialHill_Terrain

`unity/Assets/_Game/Scenes/Levels/L00_TutorialHill/L00_TutorialHill_Terrain.unity` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Scenes/Levels/L00_TutorialHill/L00_TutorialHill_Terrain.unity)) · Scene

Build Settings: index 2

### L01_Meadow_Gameplay

`unity/Assets/_Game/Scenes/Levels/L01_Meadow/L01_Meadow_Gameplay.unity` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Scenes/Levels/L01_Meadow/L01_Meadow_Gameplay.unity)) · Scene

Build Settings: index 5

**Tower Placer** · TowerPlacer

| Field | Value | Points to |
|---|---|---|
| `_cellSize` | 1 |  |
| `_buildArea.x` | -10 |  |
| `_buildArea.y` | -5 |  |
| `_buildArea.width` | 20 |  |
| `_buildArea.height` | 10 |  |
| `_path` | Enemy Path (EnemyPath) | Component: unity/Assets/_Game/Scenes/Levels/L01_Meadow/L01_Meadow_Gameplay.unity |
| `_wallet` | Honeydew Wallet (HoneydewWallet) | Component: unity/Assets/_Game/Scenes/Levels/L01_Meadow/L01_Meadow_Gameplay.unity |
| `_offers` | 6 item(s) |  |
| `_offers[0].Data` | Ant | Data asset: unity/Assets/_Game/Features/Towers/Data/Ant.asset |
| `_offers[0].Prefab` | Tower_Ant › Tower | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_Ant.prefab |
| `_offers[1].Data` | BulletAnt | Data asset: unity/Assets/_Game/Features/Towers/Data/BulletAnt.asset |
| `_offers[1].Prefab` | Tower_BulletAnt › Tower | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_BulletAnt.prefab |
| `_offers[2].Data` | CrazyAnt | Data asset: unity/Assets/_Game/Features/Towers/Data/CrazyAnt.asset |
| `_offers[2].Prefab` | Tower_CrazyAnt › Tower | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_CrazyAnt.prefab |
| `_offers[3].Data` | FireAnt | Data asset: unity/Assets/_Game/Features/Towers/Data/FireAnt.asset |
| `_offers[3].Prefab` | Tower_FireAnt › Tower | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_FireAnt.prefab |
| `_offers[4].Data` | SapAnt | Data asset: unity/Assets/_Game/Features/Towers/Data/SapAnt.asset |
| `_offers[4].Prefab` | Tower_SapAnt › Tower | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_SapAnt.prefab |
| `_offers[5].Data` | CarpenterAnt | Data asset: unity/Assets/_Game/Features/Towers/Data/CarpenterAnt.asset |
| `_offers[5].Prefab` | Tower_CarpenterAnt › Tower | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_CarpenterAnt.prefab |
| `_sellRefund` | 0.75 |  |
| `_actions` | HoldTheHill | Input actions: unity/Assets/_Game/Input/HoldTheHill.inputactions |
| `_freeCellColor` | (1, 1, 1, 0.35) |  |
| `_blockedCellColor` | (1, 0.3, 0.3, 0.35) |  |
| `_unaffordableColor` | (1, 0.8, 0.2, 0.35) |  |
| `_selectionColor` | (0.4, 0.9, 1, 0.45) |  |

**Main Camera** · LevelCameraFit

| Field | Value | Points to |
|---|---|---|
| `_halfWidth` | 10 |  |
| `_halfHeight` | 5 |  |
| `_bottomBand` | 0.09 |  |
| `_topBand` | 0.15 |  |
| `_rightBand` | 0.3 |  |

**Level Run** · RunControls

| Field | Value | Points to |
|---|---|---|
| `_actions` | HoldTheHill | Input actions: unity/Assets/_Game/Input/HoldTheHill.inputactions |

**Level Run** · LevelRun

| Field | Value | Points to |
|---|---|---|
| `_queen` | Queen (Queen) | Component: unity/Assets/_Game/Scenes/Levels/L01_Meadow/L01_Meadow_Gameplay.unity |
| `_spawner` | Enemy Spawner (EnemySpawner) | Component: unity/Assets/_Game/Scenes/Levels/L01_Meadow/L01_Meadow_Gameplay.unity |
| `_wallet` | Honeydew Wallet (HoneydewWallet) | Component: unity/Assets/_Game/Scenes/Levels/L01_Meadow/L01_Meadow_Gameplay.unity |
| `_placer` | Tower Placer (TowerPlacer) | Component: unity/Assets/_Game/Scenes/Levels/L01_Meadow/L01_Meadow_Gameplay.unity |
| `_powerUps` | Power Ups (LevelPowerUps) | Component: unity/Assets/_Game/Scenes/Levels/L01_Meadow/L01_Meadow_Gameplay.unity |
| `_fastTimeScale` | 2 |  |

**Enemy Spawner** · EnemySpawner

| Field | Value | Points to |
|---|---|---|
| `defaultSpawnPoint` | Enemy Path/Waypoint 0 (Transform) | Component: unity/Assets/_Game/Scenes/Levels/L01_Meadow/L01_Meadow_Gameplay.unity |
| `enemyContainer` | (none) |  |
| `activeMapId` | Map_01 |  |
| `autoDetectMapFromScene` | 0 |  |
| `activeMapConfigOverride` | L01_Meadow_Waves | Data asset: unity/Assets/_Game/Features/Enemies/Data/L01_Meadow_Waves.asset |
| `scriptableMapConfigs` | 0 item(s) |  |
| `inlineMapConfigs` | 0 item(s) |  |
| `autoStartFirstWave` | 0 |  |
| `requireEnemiesClearedBeforeNextWave` | 0 |  |
| `onEnemySpawned` | 0 listener(s) |  |
| `onWaveStarted` | 0 listener(s) |  |
| `onWaveCompleted` | 0 listener(s) |  |
| `onAllWavesCompleted` | 0 listener(s) |  |
| `onMapChanged` | 0 listener(s) |  |

**Queen** · Queen

| Field | Value | Points to |
|---|---|---|
| `_maxHealth` | 100 |  |
| `_autoListenToEnemyEvents` | 1 |  |
| `_onGameOver` | 0 listener(s) |  |
| `_onHealthChanged` | 0 listener(s) |  |

**Enemy Path** · EnemyPath

| Field | Value | Points to |
|---|---|---|
| `_roadWidth` | 1 |  |
| `_roadColor` | (0.55, 0.42, 0.3, 1) |  |

**Power Ups** · LevelPowerUps

| Field | Value | Points to |
|---|---|---|
| `_settings` | PowerUpSettings | Data asset: unity/Assets/_Game/Features/PowerUps/Data/PowerUpSettings.asset |
| `_pickupPrefab` | PowerUp_Pickup › PowerUpPickup | Component: unity/Assets/_Game/Features/PowerUps/Prefabs/PowerUp_Pickup.prefab |
| `_placer` | Tower Placer (TowerPlacer) | Component: unity/Assets/_Game/Scenes/Levels/L01_Meadow/L01_Meadow_Gameplay.unity |
| `_actions` | HoldTheHill | Input actions: unity/Assets/_Game/Input/HoldTheHill.inputactions |

**Honeydew Wallet** · HoneydewWallet

| Field | Value | Points to |
|---|---|---|
| `_startingHoneydew` | 300 |  |

**Honeydew Wallet** · WaveReward

| Field | Value | Points to |
|---|---|---|
| `_spawner` | Enemy Spawner (EnemySpawner) | Component: unity/Assets/_Game/Scenes/Levels/L01_Meadow/L01_Meadow_Gameplay.unity |
| `_honeydewPerWave` | 50 |  |

**Silverfish Click Reader** · SilverfishClickReader

| Field | Value | Points to |
|---|---|---|
| `_actions` | HoldTheHill | Input actions: unity/Assets/_Game/Input/HoldTheHill.inputactions |
| `_fallbackRadius` | 0.4 |  |

**Hazard Scheduler** · HazardScheduler

| Field | Value | Points to |
|---|---|---|
| `_spawner` | Enemy Spawner (EnemySpawner) | Component: unity/Assets/_Game/Scenes/Levels/L01_Meadow/L01_Meadow_Gameplay.unity |
| `_schedule` | L01_Meadow_Hazards | Data asset: unity/Assets/_Game/Features/Hazards/Data/L01_Meadow_Hazards.asset |

### L01_Meadow_Terrain

`unity/Assets/_Game/Scenes/Levels/L01_Meadow/L01_Meadow_Terrain.unity` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Scenes/Levels/L01_Meadow/L01_Meadow_Terrain.unity)) · Scene

Build Settings: index 4

### UI

`unity/Assets/_Game/Scenes/UI.unity` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Scenes/UI.unity)) · Scene

Build Settings: index 1

**Canvas/HUD/TowerList** · GameObject (override)

| Field | Override | Base value |
|---|---|---|
| `m_Name` | TowerList |  |

**Canvas/HUD/TowerList** · TowerShopManager (override)

| Field | Override | Base value |
|---|---|---|
| `availableTowers (count)` | 6 |  |
| `availableTowers[0]` | Ant | Ant |
| `availableTowers[1]` | BulletAnt | BulletAnt |
| `availableTowers[2]` | CrazyAnt | CrazyAnt |
| `availableTowers[3]` | FireAnt | FireAnt |
| `availableTowers[4]` | SapAnt | SapAnt |
| `availableTowers[5]` | CarpenterAnt | CarpenterAnt |

**Canvas/HUD/TowerList/Scroll/View/ShopContainer** · VerticalLayoutGroup (override)

| Field | Override | Base value |
|---|---|---|
| `(component)` | removed |  |

**Canvas/HUD/TowerList/BalanceAmount** · HoneydewLabel

| Field | Value | Points to |
|---|---|---|
| `_amountOnly` | 1 |  |

**Canvas/HUD/Honeydew** · HoneydewLabel

| Field | Value | Points to |
|---|---|---|
| `_amountOnly` | 0 |  |

**Canvas/HUD/Power-up Slot** · PowerUpSlotPanel

| Field | Value | Points to |
|---|---|---|
| `_button` | Canvas/HUD/Power-up Slot (Button) | Component: unity/Assets/_Game/Scenes/UI.unity |
| `_icon` | Canvas/HUD/Power-up Slot/Icon (Image) | Component: unity/Assets/_Game/Scenes/UI.unity |
| `_label` | Canvas/HUD/Power-up Slot/Text (TextMeshProUGUI) | Component: unity/Assets/_Game/Scenes/UI.unity |
| `_noticeSeconds` | 2 |  |

**Canvas/Pause Menu** · PauseMenu

| Field | Value | Points to |
|---|---|---|
| `_panel` | Canvas/Pause Menu/Panel | GameObject: unity/Assets/_Game/Scenes/UI.unity |
| `_resume` | Canvas/Pause Menu/Panel/Resume (Button) | Component: unity/Assets/_Game/Scenes/UI.unity |
| `_restart` | Canvas/Pause Menu/Panel/Restart (Button) | Component: unity/Assets/_Game/Scenes/UI.unity |
| `_home` | Canvas/Pause Menu/Panel/Home (Button) | Component: unity/Assets/_Game/Scenes/UI.unity |

**Canvas/HUD/Queen Mantis Bar** · QueenMantisBossBar

| Field | Value | Points to |
|---|---|---|
| `_panel` | Canvas/HUD/Queen Mantis Bar/Panel | GameObject: unity/Assets/_Game/Scenes/UI.unity |
| `_fill` | Canvas/HUD/Queen Mantis Bar/Panel/Bar/Fill (Image) | Component: unity/Assets/_Game/Scenes/UI.unity |
| `_label` | Canvas/HUD/Queen Mantis Bar/Panel/Label (TextMeshProUGUI) | Component: unity/Assets/_Game/Scenes/UI.unity |
| `_whiteFill` | (0.93, 0.93, 0.85, 1) |  |
| `_blackFill` | (0.55, 0.35, 0.75, 1) |  |

**Canvas** · HudPanel

| Field | Value | Points to |
|---|---|---|
| `_hud` | Canvas/HUD | GameObject: unity/Assets/_Game/Scenes/UI.unity |

**Canvas/HUD/Queen Bar** · QueenHealthBar

| Field | Value | Points to |
|---|---|---|
| `_fill` | Canvas/HUD/Queen Bar/Fill (Image) | Component: unity/Assets/_Game/Scenes/UI.unity |

**Canvas/HUD** · TowerCardPanel

| Field | Value | Points to |
|---|---|---|
| `_panel` | Canvas/HUD/Tower Card | GameObject: unity/Assets/_Game/Scenes/UI.unity |
| `_name` | Canvas/HUD/Tower Card/Name (TextMeshProUGUI) | Component: unity/Assets/_Game/Scenes/UI.unity |
| `_level` | Canvas/HUD/Tower Card/Level (TextMeshProUGUI) | Component: unity/Assets/_Game/Scenes/UI.unity |
| `_upgrade` | Canvas/HUD/Tower Card/Upgrade Button (Button) | Component: unity/Assets/_Game/Scenes/UI.unity |
| `_upgradeLabel` | Canvas/HUD/Tower Card/Upgrade Button/Text (TextMeshProUGUI) | Component: unity/Assets/_Game/Scenes/UI.unity |
| `_burrow` | Canvas/HUD/Tower Card/Burrow Button (Button) | Component: unity/Assets/_Game/Scenes/UI.unity |
| `_burrowLabel` | Canvas/HUD/Tower Card/Burrow Button/Text (TextMeshProUGUI) | Component: unity/Assets/_Game/Scenes/UI.unity |
| `_sell` | Canvas/HUD/Tower Card/Sell Button (Button) | Component: unity/Assets/_Game/Scenes/UI.unity |
| `_sellLabel` | Canvas/HUD/Tower Card/Sell Button/Text (TextMeshProUGUI) | Component: unity/Assets/_Game/Scenes/UI.unity |
| `_orderButtons` | 3 item(s) |  |
| `_orderButtons[0]` | Canvas/HUD/Tower Card/Order First (Button) | Component: unity/Assets/_Game/Scenes/UI.unity |
| `_orderButtons[1]` | Canvas/HUD/Tower Card/Order Last (Button) | Component: unity/Assets/_Game/Scenes/UI.unity |
| `_orderButtons[2]` | Canvas/HUD/Tower Card/Order Closest (Button) | Component: unity/Assets/_Game/Scenes/UI.unity |
| `_orderTitle` | Canvas/HUD/Tower Card/Order Title | GameObject: unity/Assets/_Game/Scenes/UI.unity |
| `_topLeftOffset` | (24, -88) |  |

**Canvas/HUD** · ControlsOverlay

| Field | Value | Points to |
|---|---|---|
| `_panel` | Canvas/HUD/Controls Help | GameObject: unity/Assets/_Game/Scenes/UI.unity |
| `_text` | Canvas/HUD/Controls Help/Text (TextMeshProUGUI) | Component: unity/Assets/_Game/Scenes/UI.unity |
| `_actions` | HoldTheHill | Input actions: unity/Assets/_Game/Input/HoldTheHill.inputactions |

**Canvas/Result** · ResultPanel

| Field | Value | Points to |
|---|---|---|
| `_panel` | Canvas/Result/Panel | GameObject: unity/Assets/_Game/Scenes/UI.unity |
| `_title` | Canvas/Result/Panel/Title (TextMeshProUGUI) | Component: unity/Assets/_Game/Scenes/UI.unity |
| `_restart` | Canvas/Result/Panel/Restart (Button) | Component: unity/Assets/_Game/Scenes/UI.unity |
| `_home` | Canvas/Result/Panel/Home (Button) | Component: unity/Assets/_Game/Scenes/UI.unity |

**Dev Keys** · DevKeys

| Field | Value | Points to |
|---|---|---|
| `_centipedePrefab` | Enemy_Centipede › CentipedeEnemy | Component: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_Centipede.prefab |
| `_queenMantisPrefab` | Enemy_QueenMantis › QueenMantisBoss | Component: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_QueenMantis.prefab |
| `_honeydewGift` | 500 |  |

### New Scene

`unity/Assets/_Sandbox/1103-Montgomery-Adam/Scenes/New Scene.unity` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Sandbox/1103-Montgomery-Adam/Scenes/New Scene.unity)) · Scene

Build Settings: not listed

**Canvas/TowerList** · GameObject (override)

| Field | Override | Base value |
|---|---|---|
| `m_Name` | TowerList |  |

**Canvas/TowerList/Scroll** · Image (override)

| Field | Override | Base value |
|---|---|---|
| `m_Color.b` | 0.21960786 | (1, 1, 1, 0) |
| `m_Color.g` | 0.21960786 | (1, 1, 1, 0) |
| `m_Color.r` | 0.21960786 | (1, 1, 1, 0) |

**Canvas/TowerList/Scroll/Scrollbar** · Scrollbar (override)

| Field | Override | Base value |
|---|---|---|
| `m_Colors.m_NormalColor.a` | 1 |  |
| `m_Colors.m_NormalColor.b` | 0.5 |  |
| `m_Colors.m_NormalColor.g` | 0.5 |  |
| `m_Colors.m_NormalColor.r` | 0.5 |  |

**Canvas/TowerList/Scroll/Scrollbar** · Image (override)

| Field | Override | Base value |
|---|---|---|
| `m_Color.a` | 0.5921569 | (1, 1, 1, 0.10980392) |
| `m_Color.b` | 0.3 | (1, 1, 1, 0.10980392) |
| `m_Color.g` | 0.3 | (1, 1, 1, 0.10980392) |
| `m_Color.r` | 0.3 | (1, 1, 1, 0.10980392) |

### UITest

`unity/Assets/_Sandbox/1103-Montgomery-Adam/Scenes/UITest.unity` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Sandbox/1103-Montgomery-Adam/Scenes/UITest.unity)) · Scene

Build Settings: not listed

**Canvas/TowerCardBase/TowerIconSprite** · Image (override)

| Field | Override | Base value |
|---|---|---|
| `m_Color.a` | 1 | (1, 1, 1, 1) |
| `m_Color.b` | 0.1 | (1, 1, 1, 1) |
| `m_Color.g` | 0.1 | (1, 1, 1, 1) |
| `m_Color.r` | 0.1 | (1, 1, 1, 1) |

**Canvas/TowerCardBase** · GameObject (override)

| Field | Override | Base value |
|---|---|---|
| `m_Name` | TowerCardBase |  |

### Path and Queen Test

`unity/Assets/_Sandbox/MetalBear4-2243/Path and Queen Test.unity` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Sandbox/MetalBear4-2243/Path and Queen Test.unity)) · Scene

Build Settings: not listed

**Spawner/EnemySpawner** · EnemySpawner

| Field | Value | Points to |
|---|---|---|
| `defaultSpawnPoint` | (none) |  |
| `enemyContainer` | (none) |  |
| `activeMapId` | Map_01 |  |
| `autoDetectMapFromScene` | 0 |  |
| `activeMapConfigOverride` | Map_01 | Data asset: unity/Assets/_Game/Features/Enemies/Scripts/EnemySpawner/Map_01.asset |
| `scriptableMapConfigs` | 0 item(s) |  |
| `inlineMapConfigs` | 0 item(s) |  |
| `autoStartFirstWave` | 1 |  |
| `requireEnemiesClearedBeforeNextWave` | 0 |  |
| `onEnemySpawned` | 0 listener(s) |  |
| `onWaveStarted` | 0 listener(s) |  |
| `onWaveCompleted` | 0 listener(s) |  |
| `onAllWavesCompleted` | 0 listener(s) |  |
| `onMapChanged` | 0 listener(s) |  |

**Circle** · Queen

| Field | Value | Points to |
|---|---|---|
| `_maxHealth` | 100 |  |
| `_autoListenToEnemyEvents` | 1 |  |
| `_onGameOver` | 0 listener(s) |  |
| `_onHealthChanged` | 0 listener(s) |  |

**Enemy Path** · EnemyPath

| Field | Value | Points to |
|---|---|---|
| `_roadWidth` | 1 |  |
| `_roadColor` | (0.55, 0.42, 0.3, 1) |  |
