# Core

Part of the [code bank](README.md), built 2026-11-05 from commit `d6944f7b`.

## Scripts

| Script | Kind | Owner | What it does |
|---|---|---|---|
| [FaceTravel](#facetravel) | MonoBehaviour | Nilly | Turns a child (the drawn body) to face the way this object moved since the last frame. Reads only the object's own position, after the movement code has run, so… |
| [SpriteFlipbook](#spriteflipbook) | MonoBehaviour | Nilly | Plays a looping row of sprites on one SpriteRenderer, at a fixed time per frame. No Animator: the frames and their timing come from the sheet (see AlphaSpritesB… |
| [SpriteRow](#spriterow) | Static class | Nilly | Timing for a row of sprites played once (a hurt flash, a death), as pure functions so EditMode tests cover it. |
| [LevelCatalogue](#levelcatalogue) | Static class | Nilly | The levels the home screen offers, in order. The Alpha has two. |
| [LevelScenes](#levelscenes) | Static class | Nilly | The scene-name rule for levels: a level with id L00_TutorialHill is the scene pair L00_TutorialHill_Terrain + L00_TutorialHill_Gameplay. Plain C#, so the rule i… |
| [SceneLoader](#sceneloader) | MonoBehaviour | Nilly | Lives in the Bootstrap scene. Loads the persistent scenes (UI), then either the home screen or a level's scene pair additively, so pressing Play in ANY scene (B… |
| [SpriteUtil](#spriteutil) | Static class | Nilly | Shared placeholder art so features run before real sprites exist. |

### FaceTravel

`unity/Assets/_Game/Core/Animation/FaceTravel.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Core/Animation/FaceTravel.cs)) · MonoBehaviour · `HoldTheHill.Core.Animation` · HoldTheHill.Runtime · owner: Nilly

Turns a child (the drawn body) to face the way this object moved since the last frame. Reads only the object's own position, after the movement code has run, so…

Derives from / implements: MonoBehaviour

Used on: Enemy_Basic, Enemy_Centipede, Enemy_RhinoBeetle, Enemy_RollyPolly, Enemy_Silverfish, Enemy_Wasp

Named in: AlphaSpritesBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_body` | `Transform` |  |  | Enemy_Basic = Enemy_Basic/Body (Transform); Enemy_Centipede = Enemy_Centipede/Body (Transform); Enemy_RhinoBeetle = Enemy_RhinoBeetle/Body (Transform); Enemy_Ro… | The child that turns. Its sprite's front points along Art Facing. |
| `_artFacingDegrees` | `float` |  |  | Enemy_Basic = 0; Enemy_Centipede = 0; Enemy_RhinoBeetle = 0; Enemy_RollyPolly = 0; Enemy_Silverfish = 0; Enemy_Wasp = 0 | Which way the art faces, in degrees: 0 is right, 90 is up. |
| `_turnDegreesPerSecond` | `float` | `720f` |  | Enemy_Basic = 720; Enemy_Centipede = 720; Enemy_RhinoBeetle = 720; Enemy_RollyPolly = 720; Enemy_Silverfish = 720; Enemy_Wasp = 720 | How fast the body turns, in degrees per second. |
| `_teleportDistance` | `float` | `1f` |  | Enemy_Basic = 1; Enemy_Centipede = 1; Enemy_RhinoBeetle = 1; Enemy_RollyPolly = 1; Enemy_Silverfish = 1; Enemy_Wasp = 1 | A jump longer than this in one frame is a teleport (spawn, split), not a step, and is ignored. |

Private members not listed: 4.

### SpriteFlipbook

`unity/Assets/_Game/Core/Animation/SpriteFlipbook.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Core/Animation/SpriteFlipbook.cs)) · MonoBehaviour · `HoldTheHill.Core.Animation` · HoldTheHill.Runtime · owner: Nilly

Plays a looping row of sprites on one SpriteRenderer, at a fixed time per frame. No Animator: the frames and their timing come from the sheet (see AlphaSpritesB…

Derives from / implements: MonoBehaviour

Used on: Enemy_Basic, Enemy_Centipede, Enemy_RhinoBeetle, Enemy_RollyPolly, Enemy_Silverfish, Enemy_Wasp, Tower_Ant, Tower_BulletAnt, Tower_CarpenterAnt, Tower_CrazyAnt, Tower_FireAnt, Tower_SapAnt

Named in: AlphaSpritesBuilder, EnemyHurtAndDeath, EnemyHurtAndDeathDataTests, TowerAttackAnimation, TowerAttackDataTests

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_renderer` | `SpriteRenderer` |  |  | Enemy_Basic; Enemy_Centipede; Enemy_RhinoBeetle; Enemy_RollyPolly; Enemy_Silverfish; Enemy_Wasp; Tower_Ant; Tower_BulletAnt (+4 more) | The renderer to animate. Empty means the one on this object. |
| `_frames` | `Sprite[]` | `System.Array.Empty<Sprite>()` |  | Enemy_Basic; Enemy_Centipede; Enemy_RhinoBeetle; Enemy_RollyPolly; Enemy_Silverfish; Enemy_Wasp; Tower_Ant; Tower_BulletAnt (+4 more) | The loop, in order. |
| `_frameSeconds` | `float` | `0.1f` |  | Enemy_Basic; Enemy_Centipede; Enemy_MantisSpawn; Enemy_RhinoBeetle; Enemy_RollyPolly; Enemy_Scarab; Enemy_Silverfish; Enemy_Wasp (+6 more) | Seconds each frame shows (the sheet's frame duration). |
| `_randomStart` | `bool` | `true` |  | Enemy_Basic; Enemy_Centipede; Enemy_RhinoBeetle; Enemy_RollyPolly; Enemy_Silverfish; Enemy_Wasp; Tower_Ant; Tower_BulletAnt (+4 more) | Start each copy on a random frame, so a crowd does not step in time. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `FrameCount` | Property | `int` | `=> _frames != null ? _frames.Length : 0` | How many frames the loop has. |  |
| `Held` | Property | `bool` | `{ get; set; }` | While held, the loop keeps time but leaves the sprite alone, so a one-shot row (a hurt flash) can show over it without the walk starting again from a new frame.… | EnemyHurtAndDeath |

Private members not listed: 7.

### SpriteRow

`unity/Assets/_Game/Core/Animation/SpriteRow.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Core/Animation/SpriteRow.cs)) · Static class · `HoldTheHill.Core.Animation` · HoldTheHill.Runtime · owner: Nilly

Timing for a row of sprites played once (a hurt flash, a death), as pure functions so EditMode tests cover it.

Named in: EnemyDeathEffect, EnemyHurtAndDeath, SpriteRowTests

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `FrameAt` | Method (static) | `int` | `(float clock, float frameSeconds, int frameCount)` | The frame to show clock seconds in, or -1 once the row is over. | EnemyDeathEffect, EnemyHurtAndDeath, SpriteRowTests |
| `Length` | Method (static) | `float` | `(float frameSeconds, int frameCount)` | How long the row lasts, played once. | EnemyDeathEffect, SpriteRowTests |

### LevelCatalogue

`unity/Assets/_Game/Core/Bootstrap/LevelCatalogue.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Core/Bootstrap/LevelCatalogue.cs)) · Static class · `HoldTheHill.Core` · HoldTheHill.Runtime · owner: Nilly

The levels the home screen offers, in order. The Alpha has two.

Named in: AlphaHomeAndLevelsBuilder, ControlsOverlay, LevelButton, LevelCatalogueTests

#### LevelInfo (struct)

One level the player can pick: its id (the scene pair's name) and its title.

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `LevelInfo` | Constructor |  | `(string id, string title, bool showsControlsHelp = false)` |  |  |
| `Id` | Property | `string` | `{ get; }` | The scene pair's name, see LevelScenes. | AlphaHomeAndLevelsBuilder, LevelCatalogueTests |
| `Title` | Property | `string` | `{ get; }` | What the level select shows. | AlphaHomeAndLevelsBuilder, LevelCatalogueTests |
| `ShowsControlsHelp` | Property | `bool` | `{ get; }` | The controls help box opens with the level. Only on the tutorial: on later maps it would cover the road, and F1 still brings it up. | ControlsOverlay, LevelCatalogueTests |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `IReadOnlyList<LevelInfo> All = new[]` | Property (static readonly) |  | `{ ... }` |  |  |
| `TryFind` | Method (static) | `bool` | `(string levelId, out LevelInfo level)` | Finds a level by id. False for an id that is not on the list. | AlphaHomeAndLevelsBuilder, ControlsOverlay, LevelButton, LevelCatalogueTests |

### LevelScenes

`unity/Assets/_Game/Core/Bootstrap/LevelScenes.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Core/Bootstrap/LevelScenes.cs)) · Static class · `HoldTheHill.Core` · HoldTheHill.Runtime · owner: Nilly

The scene-name rule for levels: a level with id L00_TutorialHill is the scene pair L00_TutorialHill_Terrain + L00_TutorialHill_Gameplay. Plain C#, so the rule i…

Named in: AlphaPowerUpsBuilder, LevelCatalogueTests, LevelScenesTests, SceneLoader

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `TerrainSuffix` | Const | `string` |  |  |  |
| `GameplaySuffix` | Const | `string` |  |  |  |
| `Terrain` | Method (static) | `string` | `(string levelId)` | The tilemap scene of a level. | LevelCatalogueTests, LevelScenesTests, SceneLoader |
| `Gameplay` | Method (static) | `string` | `(string levelId)` | The camera, spawner and run scene of a level. It becomes the active scene. | LevelCatalogueTests, LevelScenesTests, SceneLoader |
| `IsLevelScene` | Method (static) | `bool` | `(string sceneName)` | True for either scene of a level. | LevelScenesTests |
| `TryGetLevelId` | Method (static) | `bool` | `(string sceneName, out string levelId)` | Reads the level id back from one of its scene names. False for every other scene (Bootstrap, UI, Home, a sandbox scene) and for a bare suffix with no id in fron… | LevelCatalogueTests, LevelScenesTests, SceneLoader |
| `UnloadOrder` | Method (static) | `List<string>` | `(IReadOnlyList<string> loaded)` | The order to unload loaded scenes in (given in load order): every level's Gameplay scene first, so a run never plays on for a few frames on a map that has alrea… | LevelScenesTests, SceneLoader |

Private members not listed: 1.

### SceneLoader

`unity/Assets/_Game/Core/Bootstrap/SceneLoader.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Core/Bootstrap/SceneLoader.cs)) · MonoBehaviour · `HoldTheHill.Core` · HoldTheHill.Runtime · owner: Nilly

Lives in the Bootstrap scene. Loads the persistent scenes (UI), then either the home screen or a level's scene pair additively, so pressing Play in ANY scene (B…

Derives from / implements: MonoBehaviour

Used on: Bootstrap

Named in: AlphaHomeAndLevelsBuilder, ControlsOverlay, LevelButton, LevelRun

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_homeScene` | `string` | `"Home"` |  | Bootstrap = Home | The home screen scene, shown when no level is open. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `BootstrapSceneName` | Const | `string` |  |  |  |
| `Current` | Property (static) | `SceneLoader` | `=> s_current` | The loader in the Bootstrap scene, for menus that live in other scenes. | ControlsOverlay, LevelButton, LevelRun |
| `Busy` | Property | `bool` | `=> _busy` | True while scenes are being swapped; calls made meanwhile are ignored. |  |
| `OpenLevelId` | Property | `string` | `{ get; }` | The id of the level whose scenes are open, or null on the home screen. | ControlsOverlay |
| `LoadLevel` | Method | `void` | `(string levelId)` | Leaves whatever is open (home screen or level) and opens the level's scene pair. | LevelButton |
| `RestartLevel` | Method | `void` | `()` | Unloads the level that is open and loads the same one again. | LevelRun |
| `GoHome` | Method | `void` | `()` | Leaves the level for the home screen. | LevelRun |

Private members not listed: 14.

### SpriteUtil

`unity/Assets/_Game/Core/Utilities/SpriteUtil.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Core/Utilities/SpriteUtil.cs)) · Static class · `HoldTheHill.Core` · HoldTheHill.Runtime · owner: Nilly

Shared placeholder art so features run before real sprites exist.

Named in: EnemyPath, TowerPlacer

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Square` | Property (static) | `Sprite` | `{ get; }` | A plain white square, one world unit across. | EnemyPath, TowerPlacer |

Private members not listed: 2.
