# Editor

Part of the [code bank](README.md), built 2026-11-05 from commit `d6944f7b`.

## Scripts

| Script | Kind | Owner | What it does |
|---|---|---|---|
| [AlphaAlignmentBuilder](#alphaalignmentbuilder) | Static class | Nilly | What items 7-20 of the 10-29 alignment report need in the scenes and settings. In the UI scene: the tower card's order heading wired, so it hides with the order… |
| [AlphaBossesBuilder](#alphabossesbuilder) | Static class | Nilly | Alpha step 6, the bosses. Makes the Centipede prefab (Jason's CentipedeEnemy with a CentipedeSpeedUp beside it) and, through QueenMantisBuilder, the Queen Manti… |
| [AlphaCoreLoopBuilder](#alphacoreloopbuilder) | Static class | Nilly | Sets up everything the Alpha core loop needs and nothing else: placeholder sprites, the first two tower prefabs and their shop data, a basic enemy prefab, a wav… |
| [AlphaEnemiesBuilder](#alphaenemiesbuilder) | Static class | Nilly | Alpha step 5, the enemies. Makes a prefab for each GDD enemy beside the basic one (Rhino Beetle, Wasp, Scarab, Rolly Polly, Mantis Spawn, Silverfish), fills the… |
| [AlphaHazardsBuilder](#alphahazardsbuilder) | Static class | Nilly | Alpha step 7, burrow and the hazards. Makes the four hazard prefabs and the wasp's stinger, gives the Wasp prefab its stinger dropper, writes one hazard schedul… |
| [AlphaHomeAndLevelsBuilder](#alphahomeandlevelsbuilder) | Static class | Nilly | Alpha step 3, home and levels. The Home scene: a title page (Play, Quit) and a level select page with one button per LevelCatalogue entry. The levels L00_Tutori… |
| [AlphaPlayerBuild](#alphaplayerbuild) | Static class | Nilly | Builds a Windows Development player of whatever scenes are enabled in Build Settings (UNITY_RULES rule 48: a player is built and run before each milestone). |
| [AlphaPlaytestToolsBuilder](#alphaplaytesttoolsbuilder) | Static class | Nilly | What the 10-31 play pass asked for in the scenes: DevKeys in the UI scene, with the centipede its F5 key sends, and a LevelCameraFit on each level's camera so t… |
| [AlphaPowerUpsBuilder](#alphapowerupsbuilder) | Static class | Nilly | Alpha step 8, power-ups. Makes the settings asset and the drop prefab, puts a Power Ups object in each level's gameplay scene (wired to the run, the placer and … |
| [AlphaRunFrameBuilder](#alpharunframebuilder) | Static class | Nilly | Alpha step 2, the run frame. In Hill01_Gameplay: the Queen, and a Level Run object that reads the run keys and decides win or lose. In the UI scene: one canvas … |
| [AlphaSellAndHelpBuilder](#alphasellandhelpbuilder) | Static class | Nilly | After the first playtest: a Sell button on the tower card, and a developer help box on the HUD that lists the controls (F1 hides it). Both go into the UI scene. |
| [AlphaShopBindingBuilder](#alphashopbindingbuilder) | Static class | Nilly | Fits Adam's TowerList (his #45) into the run, in the UI scene only: a ShopPlacerBinding as an added component on the prefab instance, so his cards follow Restar… |
| [AlphaSpritesBuilder](#alphaspritesbuilder) | Static class | Nilly | Alpha sprites (2026-11-03): Nilly's top-down pixel sheets on the placeholder enemies and towers, each playing one looping row through a SpriteFlipbook (no Anima… |
| [AlphaTowersBuilder](#alphatowersbuilder) | Static class | Nilly | Alpha step 4, the rest of the towers. Makes the four remaining tower prefabs and their shop data, gives every tower its upgrade levels, puts all six on offer in… |
| [HoldTheHillMenu](#holdthehillmenu) | Static class | Nilly | Adds a Tools &gt; Hold the Hill menu to the Unity editor. |
| [QueenMantisBuilder](#queenmantisbuilder) | Static class | Nilly | The Queen Mantis rebuild (2026-11-02): her art's import settings, a ring sprite, her QueenMantisData, the Enemy_QueenMantis prefab (QueenMantisBoss with the hea… |

### AlphaAlignmentBuilder

`unity/Assets/_Game/Editor/AlphaAlignmentBuilder.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Editor/AlphaAlignmentBuilder.cs)) · Static class · `HoldTheHill.EditorTools` · HoldTheHill.Editor · owner: Nilly

What items 7-20 of the 10-29 alignment report need in the scenes and settings. In the UI scene: the tower card's order heading wired, so it hides with the order…

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `UiScenePath` | Const (private) | `string` |  |  |  |
| `BootstrapScenePath` | Const (private) | `string` |  |  |  |
| `LegacyLevelFolder` | Const (private) | `string` |  |  |  |
| `OrderTitleName` | Const (private) | `string` |  |  |  |
| `ListScrollSensitivity` | Const (private) | `float` |  |  |  |
| `CameraTopBand` | Const (private) | `float` |  |  |  |
| `CameraRightBand` | Const (private) | `float` |  |  |  |
| `HelpBoxMinHeight` | Const (private) | `float` |  |  |  |
| `HelpBoxSpare` | Const (private) | `float` |  |  |  |
| `BuildFromCommandLine` | Method (static) | `void` | `()` | Entry point for a batch run: -executeMethod HoldTheHill.EditorTools.AlphaAlignmentBuilder.BuildFromCommandLine. |  |

Private members not listed: 12.

### AlphaBossesBuilder

`unity/Assets/_Game/Editor/AlphaBossesBuilder.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Editor/AlphaBossesBuilder.cs)) · Static class · `HoldTheHill.EditorTools` · HoldTheHill.Editor · owner: Nilly

Alpha step 6, the bosses. Makes the Centipede prefab (Jason's CentipedeEnemy with a CentipedeSpeedUp beside it) and, through QueenMantisBuilder, the Queen Manti…

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `EnemyPrefabFolder` | Const (private) | `string` |  |  |  |
| `WaveDataFolder` | Const (private) | `string` |  |  |  |
| `CentipedePath` | Const (private) | `string` |  |  |  |
| `QueenMantisPath` | Const (private) | `string` |  |  |  |
| `GapBeforeBoss` | Const (private) | `float` |  | Seconds between the last ordinary enemy of a boss wave and the boss itself. |  |
| `BuildFromCommandLine` | Method (static) | `void` | `()` | Entry point for a batch run: -executeMethod HoldTheHill.EditorTools.AlphaBossesBuilder.BuildFromCommandLine. |  |

Private members not listed: 8.

### AlphaCoreLoopBuilder

`unity/Assets/_Game/Editor/AlphaCoreLoopBuilder.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Editor/AlphaCoreLoopBuilder.cs)) · Static class · `HoldTheHill.EditorTools` · HoldTheHill.Editor · owner: Nilly

Sets up everything the Alpha core loop needs and nothing else: placeholder sprites, the first two tower prefabs and their shop data, a basic enemy prefab, a wav…

Named in: AlphaBossesBuilder, AlphaEnemiesBuilder, AlphaHazardsBuilder, AlphaHomeAndLevelsBuilder, AlphaPowerUpsBuilder, AlphaRunFrameBuilder, AlphaTowersBuilder, QueenMantisBuilder

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `PlaceholderArtFolder` | Const (private) | `string` |  |  |  |
| `TowerPrefabFolder` | Const (private) | `string` |  |  |  |
| `TowerDataFolder` | Const (private) | `string` |  |  |  |
| `EnemyPrefabFolder` | Const (private) | `string` |  |  |  |
| `EnemyDataFolder` | Const (private) | `string` |  |  |  |
| `ActionsPath` | Const (private) | `string` |  |  |  |
| `GameplayScenePath` | Const (private) | `string` |  |  |  |
| `Hill01WavesPath` | Const (private) | `string` |  |  |  |
| `SpriteSize` | Const (private) | `int` |  |  |  |
| `EnemyPrefabPath` | Const (internal) | `string` |  | The basic enemy prefab the placeholder waves use. | AlphaHomeAndLevelsBuilder |
| `TowerSpec[] Towers =` | Property (internal static readonly) |  | `{ ... }` | The six towers of the GDD, in number-key and shop order, at the GDD costs. |  |
| `EnsureTowerAssets` | Method (internal static) | `void` | `()` | Makes the sprites, projectiles, mine, the six tower prefabs and their shop data; keeps what exists. | AlphaTowersBuilder |
| `BuildFromCommandLine` | Method (static) | `void` | `()` | Entry point for a batch run: -executeMethod HoldTheHill.EditorTools.AlphaCoreLoopBuilder.BuildFromCommandLine. | AlphaHomeAndLevelsBuilder |
| `EnsureSprite` | Method (internal static) | `Sprite` | `(string name, bool circle)` |  | AlphaBossesBuilder, AlphaEnemiesBuilder, AlphaHazardsBuilder, AlphaPowerUpsBuilder, AlphaRunFrameBuilder |
| `EnsureEnemyPrefab` | Method (internal static) | `GameObject` | `(Sprite sprite)` |  | AlphaEnemiesBuilder |
| `EnsureWaves` | Method (internal static) | `MapWaveDataSO` | `(string path, string mapId, GameObject enemyPrefab, IReadOnlyList<WaveSpec> specs)` | A wave list asset for one level, made only if it is missing so Inspector tuning survives. Placeholder contents: every wave is enemyPrefab repeated. | AlphaHomeAndLevelsBuilder |
| `WireGameplayScene` | Method (internal static) | `void` | `(string scenePath, string wavesAssetPath)` | Opens a level's gameplay scene and makes sure it has a path, a spawner on the given wave list, a wallet with the wave reward, and a placer offering the Ant and … | AlphaHomeAndLevelsBuilder, AlphaTowersBuilder |
| `EnsureFolder` | Method (internal static) | `void` | `(string path)` |  | AlphaBossesBuilder, AlphaEnemiesBuilder, AlphaHazardsBuilder, AlphaHomeAndLevelsBuilder, AlphaPowerUpsBuilder, QueenMantisBuilder |

#### AlphaCoreLoopBuilder.WaveSpec (struct)

One placeholder wave: how many basic enemies, and the gap between them.

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `WaveSpec` | Constructor |  | `(int count, float delay)` |  | AlphaHomeAndLevelsBuilder |
| `Count` | Property | `int` | `{ get; }` |  |  |
| `Delay` | Property | `float` | `{ get; }` |  |  |

#### AlphaCoreLoopBuilder.TowerKind (enum)

How a tower attacks; decides which extra component or projectile its prefab gets.

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Melee` | Enum member (static) | `TowerKind` |  |  |  |
| `Shot` | Enum member (static) | `TowerKind` |  |  |  |
| `Lob` | Enum member (static) | `TowerKind` |  |  |  |
| `Slow` | Enum member (static) | `TowerKind` |  |  |  |
| `Mines` | Enum member (static) | `TowerKind` |  |  |  |

#### AlphaCoreLoopBuilder.TowerSpec (struct)

One of the GDD's six towers: its prefab, shop data and numbers for all three levels.

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `TowerSpec` | Constructor |  | `(string prefabName, string dataFile, string title, int cost, Color color, TowerKind kind, float range, float damage, float interval, TowerUpgrade level2, TowerU…` |  |  |
| `PrefabName` | Property | `string` | `{ get; }` |  |  |
| `DataFile` | Property | `string` | `{ get; }` |  |  |
| `Title` | Property | `string` | `{ get; }` |  |  |
| `Cost` | Property | `int` | `{ get; }` |  |  |
| `Color` | Property | `Color` | `{ get; }` |  |  |
| `Kind` | Property | `TowerKind` | `{ get; }` |  |  |
| `Range` | Property | `float` | `{ get; }` |  |  |
| `Damage` | Property | `float` | `{ get; }` |  |  |
| `Interval` | Property | `float` | `{ get; }` |  |  |
| `Level2` | Property | `TowerUpgrade` | `{ get; }` |  |  |
| `Level3` | Property | `TowerUpgrade` | `{ get; }` |  |  |
| `PrefabPath` | Property | `string` | `=> $"{TowerPrefabFolder}/{PrefabName}.prefab"` |  |  |
| `DataPath` | Property | `string` | `=> $"{TowerDataFolder}/{DataFile}.asset"` |  |  |

Private members not listed: 20.

### AlphaEnemiesBuilder

`unity/Assets/_Game/Editor/AlphaEnemiesBuilder.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Editor/AlphaEnemiesBuilder.cs)) · Static class · `HoldTheHill.EditorTools` · HoldTheHill.Editor · owner: Nilly

Alpha step 5, the enemies. Makes a prefab for each GDD enemy beside the basic one (Rhino Beetle, Wasp, Scarab, Rolly Polly, Mantis Spawn, Silverfish), fills the…

Named in: AlphaCoreLoopBuilder, LevelPowerUps, PowerUpPickup

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `EnemyPrefabFolder` | Const (private) | `string` |  |  |  |
| `WaveDataFolder` | Const (private) | `string` |  |  |  |
| `LevelsFolder` | Const (private) | `string` |  |  |  |
| `ActionsPath` | Const (private) | `string` |  |  |  |
| `EnemySpec[] Enemies =` | Property (internal static readonly) |  | `{ ... }` | The GDD enemies at placeholder numbers; the basic enemy is 30 health, 10 damage, speed 1.5, bounty 5. |  |
| `BuildFromCommandLine` | Method (static) | `void` | `()` | Entry point for a batch run: -executeMethod HoldTheHill.EditorTools.AlphaEnemiesBuilder.BuildFromCommandLine. |  |
| `Describe` | Method (internal static) | `string` | `(Wave wave)` |  |  |

#### AlphaEnemiesBuilder.Kind (enum)

Every enemy the waves can name. Basic is the step 1 prefab; the rest are made here.

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Basic` | Enum member (static) | `Kind` |  |  |  |
| `Scarab` | Enum member (static) | `Kind` |  |  |  |
| `MantisSpawn` | Enum member (static) | `Kind` |  |  |  |
| `RhinoBeetle` | Enum member (static) | `Kind` |  |  |  |
| `Wasp` | Enum member (static) | `Kind` |  |  |  |
| `RollyPolly` | Enum member (static) | `Kind` |  |  |  |
| `Silverfish` | Enum member (static) | `Kind` |  |  |  |

#### AlphaEnemiesBuilder.EnemySpec (struct)

One enemy prefab: a variant of the basic enemy, or a new prefab on its own subclass.

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `EnemySpec` | Constructor |  | `(Kind kind, string prefabName, Color color, float scale, double health, int damage, float speed, int bounty, bool variantOfBasic)` |  |  |
| `Kind` | Property | `Kind` | `{ get; }` |  |  |
| `PrefabName` | Property | `string` | `{ get; }` |  |  |
| `Color` | Property | `Color` | `{ get; }` |  |  |
| `Scale` | Property | `float` | `{ get; }` |  |  |
| `Health` | Property | `double` | `{ get; }` |  |  |
| `Damage` | Property | `int` | `{ get; }` |  |  |
| `Speed` | Property | `float` | `{ get; }` |  |  |
| `Bounty` | Property | `int` | `{ get; }` |  |  |
| `VariantOfBasic` | Property | `bool` | `{ get; }` |  |  |
| `PrefabPath` | Property | `string` | `=> $"{EnemyPrefabFolder}/{PrefabName}.prefab"` |  |  |

#### AlphaEnemiesBuilder.Group (struct)

A run of one enemy kind in a wave: how many, and the gap after each.

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Group` | Constructor |  | `(Kind kind, int count, float delay)` |  |  |
| `Kind` | Property | `Kind` | `{ get; }` |  |  |
| `Count` | Property | `int` | `{ get; }` |  |  |
| `Delay` | Property | `float` | `{ get; }` |  |  |

Private members not listed: 12.

### AlphaHazardsBuilder

`unity/Assets/_Game/Editor/AlphaHazardsBuilder.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Editor/AlphaHazardsBuilder.cs)) · Static class · `HoldTheHill.EditorTools` · HoldTheHill.Editor · owner: Nilly

Alpha step 7, burrow and the hazards. Makes the four hazard prefabs and the wasp's stinger, gives the Wasp prefab its stinger dropper, writes one hazard schedul…

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `HazardPrefabFolder` | Const (private) | `string` |  |  |  |
| `HazardDataFolder` | Const (private) | `string` |  |  |  |
| `EnemyPrefabFolder` | Const (private) | `string` |  |  |  |
| `LevelsFolder` | Const (private) | `string` |  |  |  |
| `UiScenePath` | Const (private) | `string` |  |  |  |
| `DiatomaceousEarthPath` | Const (private) | `string` |  |  |  |
| `WaspStingerPath` | Const (private) | `string` |  |  |  |
| `BirdShadowPath` | Const (private) | `string` |  |  |  |
| `PesticideHumanPath` | Const (private) | `string` |  |  |  |
| `WaspPath` | Const (private) | `string` |  |  |  |
| `CardGrowth` | Const (private) | `float` |  |  |  |
| `BuildFromCommandLine` | Method (static) | `void` | `()` | Entry point for a batch run: -executeMethod HoldTheHill.EditorTools.AlphaHazardsBuilder.BuildFromCommandLine. |  |

#### AlphaHazardsBuilder.EntrySpec (struct)

One scheduled hazard, in plain numbers. Positions are world units on the level's route.

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Wave` | Field (readonly) | `int` |  |  |  |
| `Delay` | Field (readonly) | `float` |  |  |  |
| `Prefab` | Field (readonly) | `string` |  |  |  |
| `Position` | Field (readonly) | `Vector2` |  |  |  |
| `EntrySpec` | Constructor |  | `(int wave, float delay, string prefab, Vector2 position)` |  |  |

Private members not listed: 14.

### AlphaHomeAndLevelsBuilder

`unity/Assets/_Game/Editor/AlphaHomeAndLevelsBuilder.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Editor/AlphaHomeAndLevelsBuilder.cs)) · Static class · `HoldTheHill.EditorTools` · HoldTheHill.Editor · owner: Nilly

Alpha step 3, home and levels. The Home scene: a title page (Play, Quit) and a level select page with one button per LevelCatalogue entry. The levels L00_Tutori…

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `ScenesFolder` | Const (private) | `string` |  |  |  |
| `LevelsFolder` | Const (private) | `string` |  |  |  |
| `HomeScenePath` | Const (private) | `string` |  |  |  |
| `BootstrapScenePath` | Const (private) | `string` |  |  |  |
| `TemplateTerrainPath` | Const (private) | `string` |  |  |  |
| `TemplateGameplayPath` | Const (private) | `string` |  |  |  |
| `WaveDataFolder` | Const (private) | `string` |  |  |  |
| `HomeSceneName` | Const (private) | `string` |  |  |  |
| `GameTitleFontSize` | Const (private) | `int` |  |  |  |
| `HeadingFontSize` | Const (private) | `int` |  |  |  |
| `BuildFromCommandLine` | Method (static) | `void` | `()` | Entry point for a batch run: -executeMethod HoldTheHill.EditorTools.AlphaHomeAndLevelsBuilder.BuildFromCommandLine. |  |

#### AlphaHomeAndLevelsBuilder.LevelSpec (class)

What differs between the placeholder levels: the route, the ground colour, the waves.

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `Route` | `Vector2Int[]` |  |  |  |  |
| `CameraBackground` | `Color` |  |  |  |  |
| `Waves` | `WaveSpec[]` |  |  |  |  |

Private members not listed: 18.

### AlphaPlayerBuild

`unity/Assets/_Game/Editor/AlphaPlayerBuild.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Editor/AlphaPlayerBuild.cs)) · Static class · `HoldTheHill.EditorTools` · HoldTheHill.Editor · owner: Nilly

Builds a Windows Development player of whatever scenes are enabled in Build Settings (UNITY_RULES rule 48: a player is built and run before each milestone).

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `DefaultBuildRoot` | Const (private) | `string` |  |  |  |
| `ExeName` | Const (private) | `string` |  |  |  |
| `OutputArgument` | Const (private) | `string` |  |  |  |
| `BuildFromCommandLine` | Method (static) | `void` | `()` | Entry point for a batch run: -batchmode -quit -buildTarget Win64 -executeMethod HoldTheHill.EditorTools.AlphaPlayerBuild.BuildFromCommandLine [-buildOutput &lt;… |  |

Private members not listed: 6.

### AlphaPlaytestToolsBuilder

`unity/Assets/_Game/Editor/AlphaPlaytestToolsBuilder.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Editor/AlphaPlaytestToolsBuilder.cs)) · Static class · `HoldTheHill.EditorTools` · HoldTheHill.Editor · owner: Nilly

What the 10-31 play pass asked for in the scenes: DevKeys in the UI scene, with the centipede its F5 key sends, and a LevelCameraFit on each level's camera so t…

Named in: AlphaAlignmentBuilder, AlphaHomeAndLevelsBuilder

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `UiScenePath` | Const (private) | `string` |  |  |  |
| `BootstrapScenePath` | Const (private) | `string` |  |  |  |
| `CentipedePrefabPath` | Const (private) | `string` |  |  |  |
| `DevKeysName` | Const (private) | `string` |  |  |  |
| `BuildFromCommandLine` | Method (static) | `void` | `()` | Entry point for a batch run: -executeMethod HoldTheHill.EditorTools.AlphaPlaytestToolsBuilder.BuildFromCommandLine. |  |
| `EnsureCameraFit` | Method (internal static) | `void` | `(Camera camera)` | Puts a LevelCameraFit on a level camera that has none. The level builder calls it too. | AlphaAlignmentBuilder, AlphaHomeAndLevelsBuilder |

Private members not listed: 4.

### AlphaPowerUpsBuilder

`unity/Assets/_Game/Editor/AlphaPowerUpsBuilder.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Editor/AlphaPowerUpsBuilder.cs)) · Static class · `HoldTheHill.EditorTools` · HoldTheHill.Editor · owner: Nilly

Alpha step 8, power-ups. Makes the settings asset and the drop prefab, puts a Power Ups object in each level's gameplay scene (wired to the run, the placer and …

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `DataFolder` | Const (private) | `string` |  |  |  |
| `PrefabFolder` | Const (private) | `string` |  |  |  |
| `SettingsPath` | Const (private) | `string` |  |  |  |
| `PickupPath` | Const (private) | `string` |  |  |  |
| `ActionsPath` | Const (private) | `string` |  |  |  |
| `LevelsFolder` | Const (private) | `string` |  |  |  |
| `UiScenePath` | Const (private) | `string` |  |  |  |
| `Margin` | Const (private) | `float` |  |  |  |
| `SlotIconSize` | Const (private) | `float` |  |  |  |
| `BuildFromCommandLine` | Method (static) | `void` | `()` | Entry point for a batch run: -executeMethod HoldTheHill.EditorTools.AlphaPowerUpsBuilder.BuildFromCommandLine. |  |

Private members not listed: 8.

### AlphaRunFrameBuilder

`unity/Assets/_Game/Editor/AlphaRunFrameBuilder.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Editor/AlphaRunFrameBuilder.cs)) · Static class · `HoldTheHill.EditorTools` · HoldTheHill.Editor · owner: Nilly

Alpha step 2, the run frame. In Hill01_Gameplay: the Queen, and a Level Run object that reads the run keys and decides win or lose. In the UI scene: one canvas …

Named in: AlphaAlignmentBuilder, AlphaEnemiesBuilder, AlphaHazardsBuilder, AlphaHomeAndLevelsBuilder, AlphaPlaytestToolsBuilder, AlphaPowerUpsBuilder, AlphaSellAndHelpBuilder, AlphaShopBindingBuilder, AlphaTowersBuilder, QueenMantisBuilder

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `GameplayScenePath` | Const (private) | `string` |  |  |  |
| `UiScenePath` | Const (private) | `string` |  |  |  |
| `ActionsPath` | Const (private) | `string` |  |  |  |
| `ReferenceResolution` | Field (internal static readonly) | `Vector2` |  |  | AlphaHomeAndLevelsBuilder |
| `Margin` | Const (private) | `float` |  |  |  |
| `HudFontSize` | Const (private) | `int` |  |  |  |
| `TitleFontSize` | Const (internal) | `int` |  |  |  |
| `ButtonFontSize` | Const (private) | `int` |  |  |  |
| `MenuButtonSize` | Field (internal static readonly) | `Vector2` |  |  | AlphaHomeAndLevelsBuilder |
| `BuildFromCommandLine` | Method (static) | `void` | `()` | Entry point for a batch run: -executeMethod HoldTheHill.EditorTools.AlphaRunFrameBuilder.BuildFromCommandLine. | AlphaHomeAndLevelsBuilder |
| `WireLevelGameplay` | Method (internal static) | `void` | `(string scenePath)` | Opens one level's gameplay scene, wires its Queen, Level Run and Run Controls, and saves it. | AlphaHomeAndLevelsBuilder |
| `WireUiSceneFile` | Method (internal static) | `void` | `()` | Opens the UI scene, adds whatever HUD piece is missing, and saves it. | AlphaHomeAndLevelsBuilder |
| `WireGameplayScene` | Method (internal static) | `void` | `(Scene scene, Sprite circle)` |  |  |
| `EnsureStretchedChild` | Method (internal static) | `RectTransform` | `(Transform parent, string name, int layer)` |  | AlphaHomeAndLevelsBuilder |
| `MakeDimPanel` | Method (internal static) | `GameObject` | `(Transform parent, int layer)` |  |  |
| `MakeImage` | Method (internal static) | `Image` | `(Transform parent, string name, Color color, int layer)` |  | AlphaPowerUpsBuilder, AlphaSellAndHelpBuilder, AlphaTowersBuilder, QueenMantisBuilder |
| `MakeText` | Method (internal static) | `TextMeshProUGUI` | `(Transform parent, string name, string content, int fontSize, TextAlignmentOptions alignment, int layer)` |  | AlphaHomeAndLevelsBuilder, AlphaSellAndHelpBuilder, AlphaTowersBuilder, QueenMantisBuilder |
| `MakeButton` | Method (internal static) | `Button` | `(Transform parent, string name, string label, int layer)` |  | AlphaHazardsBuilder, AlphaHomeAndLevelsBuilder, AlphaPowerUpsBuilder, AlphaSellAndHelpBuilder, AlphaTowersBuilder |
| `BuiltinSprite` | Method (internal static) | `Sprite` | `(string path)` |  | QueenMantisBuilder |
| `Anchor` | Method (internal static) | `void` | `(RectTransform rect, Vector2 anchor, Vector2 position, Vector2 size)` |  | AlphaHazardsBuilder, AlphaHomeAndLevelsBuilder, AlphaPowerUpsBuilder, AlphaSellAndHelpBuilder, AlphaTowersBuilder, QueenMantisBuilder |
| `Stretch` | Method (internal static) | `void` | `(RectTransform rect, Vector2 padding)` |  | AlphaSellAndHelpBuilder, QueenMantisBuilder |
| `FindInScene<T>` | Method (internal static) | `T` | `(Scene scene)` |  | AlphaAlignmentBuilder, AlphaEnemiesBuilder, AlphaHazardsBuilder, AlphaHomeAndLevelsBuilder, AlphaPlaytestToolsBuilder, AlphaPowerUpsBuilder, AlphaSellAndHelpBui… |
| `Name` | Method (internal static) | `string` | `(SerializedObject so, string field)` |  | AlphaAlignmentBuilder, AlphaEnemiesBuilder, AlphaHazardsBuilder, AlphaPowerUpsBuilder, AlphaSellAndHelpBuilder, AlphaTowersBuilder |

Private members not listed: 16.

### AlphaSellAndHelpBuilder

`unity/Assets/_Game/Editor/AlphaSellAndHelpBuilder.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Editor/AlphaSellAndHelpBuilder.cs)) · Static class · `HoldTheHill.EditorTools` · HoldTheHill.Editor · owner: Nilly

After the first playtest: a Sell button on the tower card, and a developer help box on the HUD that lists the controls (F1 hides it). Both go into the UI scene.

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `UiScenePath` | Const (private) | `string` |  |  |  |
| `ActionsPath` | Const (private) | `string` |  |  |  |
| `Margin` | Const (private) | `float` |  |  |  |
| `CardGrowth` | Const (private) | `float` |  |  |  |
| `HelpFontSize` | Const (private) | `int` |  |  |  |
| `BuildFromCommandLine` | Method (static) | `void` | `()` | Entry point for a batch run: -executeMethod HoldTheHill.EditorTools.AlphaSellAndHelpBuilder.BuildFromCommandLine. |  |

Private members not listed: 7.

### AlphaShopBindingBuilder

`unity/Assets/_Game/Editor/AlphaShopBindingBuilder.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Editor/AlphaShopBindingBuilder.cs)) · Static class · `HoldTheHill.EditorTools` · HoldTheHill.Editor · owner: Nilly

Fits Adam's TowerList (his #45) into the run, in the UI scene only: a ShopPlacerBinding as an added component on the prefab instance, so his cards follow Restar…

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `UiScenePath` | Const (private) | `string` |  |  |  |
| `TowerDataFolder` | Const (private) | `string` |  |  |  |
| `BalanceTextName` | Const (private) | `string` |  |  |  |
| `BuildFromCommandLine` | Method (static) | `void` | `()` | Entry point for a batch run: -executeMethod HoldTheHill.EditorTools.AlphaShopBindingBuilder.BuildFromCommandLine. |  |

Private members not listed: 7.

### AlphaSpritesBuilder

`unity/Assets/_Game/Editor/AlphaSpritesBuilder.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Editor/AlphaSpritesBuilder.cs)) · Static class · `HoldTheHill.EditorTools` · HoldTheHill.Editor · owner: Nilly

Alpha sprites (2026-11-03): Nilly's top-down pixel sheets on the placeholder enemies and towers, each playing one looping row through a SpriteFlipbook (no Anima…

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `EnemyPrefabFolder` | Const (private) | `string` |  |  |  |
| `TowerPrefabFolder` | Const (private) | `string` |  |  |  |
| `EnemySheetFolder` | Const (private) | `string` |  |  |  |
| `TowerSheetFolder` | Const (private) | `string` |  |  |  |
| `TowerDataFolder` | Const (private) | `string` |  |  |  |
| `BodyName` | Const (private) | `string` |  |  |  |
| `PixelsPerUnit` | Const (private) | `float` |  |  |  |
| `BuildFromCommandLine` | Method (static) | `void` | `()` | Entry point for a batch run: -executeMethod HoldTheHill.EditorTools.AlphaSpritesBuilder.BuildFromCommandLine. |  |
| `HurtRow` | Const (private) | `string` |  |  |  |
| `DeathRow` | Const (private) | `string` |  |  |  |
| `AttackRow` | Const (private) | `string` |  |  |  |
| `AttackHitFrame` | Const (private) | `int` |  |  |  |

#### AlphaSpritesBuilder.Look (struct)

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Prefab` | Field (readonly) | `string` |  |  |  |
| `Sheet` | Field (readonly) | `string` |  |  |  |
| `Loop` | Field (readonly) | `string` |  |  |  |
| `VariantOfBasic` | Field (readonly) | `bool` |  |  |  |
| `Look` | Constructor |  | `(string prefab, string sheet, string loop, bool variantOfBasic = false)` |  |  |

#### AlphaSpritesBuilder.AsepriteSheet (class)

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `frames` | `AsepriteFrame[]` |  |  |  |  |

#### AlphaSpritesBuilder.AsepriteFrame (class)

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `filename` | `string` |  |  |  |  |
| `frame` | `AsepriteRect` |  |  |  |  |
| `duration` | `int` |  |  |  |  |

#### AlphaSpritesBuilder.AsepriteRect (class)

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `x` | `int` |  |  |  |  |
| `y` | `int` |  |  |  |  |
| `w` | `int` |  |  |  |  |
| `h` | `int` |  |  |  |  |

#### AlphaSpritesBuilder.Loop (struct)

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Frames` | Field (readonly) | `Sprite[]` |  |  |  |
| `FrameSeconds` | Field (readonly) | `float` |  |  |  |
| `FrameWidth` | Field (readonly) | `int` |  |  |  |
| `Loop` | Constructor |  | `(Sprite[] frames, float frameSeconds, int frameWidth)` |  |  |
| `IsEmpty` | Property | `bool` | `=> Frames == null \|\| Frames.Length == 0` |  |  |

Private members not listed: 25.

### AlphaTowersBuilder

`unity/Assets/_Game/Editor/AlphaTowersBuilder.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Editor/AlphaTowersBuilder.cs)) · Static class · `HoldTheHill.EditorTools` · HoldTheHill.Editor · owner: Nilly

Alpha step 4, the rest of the towers. Makes the four remaining tower prefabs and their shop data, gives every tower its upgrade levels, puts all six on offer in…

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `UiScenePath` | Const (private) | `string` |  |  |  |
| `LevelsFolder` | Const (private) | `string` |  |  |  |
| `WaveDataFolder` | Const (private) | `string` |  |  |  |
| `Margin` | Const (private) | `float` |  |  |  |
| `CardFontSize` | Const (private) | `int` |  |  |  |
| `SmallFontSize` | Const (private) | `int` |  |  |  |
| `BuildFromCommandLine` | Method (static) | `void` | `()` | Entry point for a batch run: -executeMethod HoldTheHill.EditorTools.AlphaTowersBuilder.BuildFromCommandLine. |  |

Private members not listed: 7.

### HoldTheHillMenu

`unity/Assets/_Game/Editor/HoldTheHillMenu.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Editor/HoldTheHillMenu.cs)) · Static class · `HoldTheHill.EditorTools` · HoldTheHill.Editor · owner: Nilly

Adds a Tools &gt; Hold the Hill menu to the Unity editor.

Named in: AlphaCoreLoopBuilder, AlphaHomeAndLevelsBuilder

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `CreateExamplePathObject` | Method (internal static) | `GameObject` | `()` | Makes an Enemy Path object following the example route, in the active scene. | AlphaCoreLoopBuilder, AlphaHomeAndLevelsBuilder |
| `SetRoute` | Method (internal static) | `void` | `(Transform path, Vector2Int[] route)` | Replaces a path's waypoints with one per grid cell of route. | AlphaHomeAndLevelsBuilder |

Private members not listed: 2.

### QueenMantisBuilder

`unity/Assets/_Game/Editor/QueenMantisBuilder.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Editor/QueenMantisBuilder.cs)) · Static class · `HoldTheHill.EditorTools` · HoldTheHill.Editor · owner: Nilly

The Queen Mantis rebuild (2026-11-02): her art's import settings, a ring sprite, her QueenMantisData, the Enemy_QueenMantis prefab (QueenMantisBoss with the hea…

Named in: AlphaBossesBuilder

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `EnemyPrefabFolder` | Const (private) | `string` |  |  |  |
| `PrefabPath` | Const (private) | `string` |  |  |  |
| `MantisSpawnPath` | Const (private) | `string` |  |  |  |
| `DataFolder` | Const (private) | `string` |  |  |  |
| `DataPath` | Const (private) | `string` |  |  |  |
| `MeadowWavesPath` | Const (private) | `string` |  |  |  |
| `ArtFolder` | Const (private) | `string` |  |  |  |
| `WhitePath` | Const (private) | `string` |  |  |  |
| `BlackPath` | Const (private) | `string` |  |  |  |
| `RingPath` | Const (private) | `string` |  |  |  |
| `UiScenePath` | Const (private) | `string` |  |  |  |
| `BootstrapScenePath` | Const (private) | `string` |  |  |  |
| `ArtPixelsPerUnit` | Const (private) | `float` |  |  |  |
| `RingSize` | Const (private) | `int` |  |  |  |
| `RingThickness` | Const (private) | `int` |  |  |  |
| `BodySortingOrder` | Const (private) | `int` |  |  |  |
| `PulseSortingOrder` | Const (private) | `int` |  |  |  |
| `NewHealth` | Const (private) | `double` |  |  |  |
| `NewDamage` | Const (private) | `int` |  |  |  |
| `NewSpeed` | Const (private) | `float` |  |  |  |
| `NewBounty` | Const (private) | `int` |  |  |  |
| `MeadowWaveSlot` | Const (private) | `int` |  |  |  |
| `GapAfterHer` | Const (private) | `float` |  |  |  |
| `GapBeforeBoss` | Const (private) | `float` |  |  |  |
| `BuildFromCommandLine` | Method (static) | `void` | `()` | Entry point for a batch run: -executeMethod HoldTheHill.EditorTools.QueenMantisBuilder.BuildFromCommandLine. |  |
| `EnsurePrefab` | Method (internal static) | `GameObject` | `()` | Art, ring sprite, data asset and prefab, with no scene opened. Build Bosses calls this too. | AlphaBossesBuilder |

Private members not listed: 26.
