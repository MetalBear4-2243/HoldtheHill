# Levels

Part of the [code bank](README.md), built 2026-11-05 from commit `d6944f7b`.

## Scripts

| Script | Kind | Owner | What it does |
|---|---|---|---|
| [LevelButton](#levelbutton) | MonoBehaviour | Nilly | A level select button: clicking it asks the SceneLoader to open its level. |
| [LevelCameraFit](#levelcamerafit) | MonoBehaviour | Nilly | Frames the level's map on any screen shape, with bands left free above and below for the HUD, so the top and bottom rows are not under the Honeydew line, the Qu… |

### LevelButton

`unity/Assets/_Game/Features/Levels/Scripts/LevelButton.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Levels/Scripts/LevelButton.cs)) · MonoBehaviour · `HoldTheHill.Features.Levels` · HoldTheHill.Runtime · owner: Nilly

A level select button: clicking it asks the SceneLoader to open its level.

Derives from / implements: MonoBehaviour

Used on: Home

Named in: AlphaHomeAndLevelsBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_levelId` | `string` |  |  | Home = L00_TutorialHill, L01_Meadow | The level's id from the LevelCatalogue, e.g. L00_TutorialHill. Names the scene pair to load. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `LevelId` | Property | `string` | `=> _levelId` | The level this button opens. |  |

Private members not listed: 5.

### LevelCameraFit

`unity/Assets/_Game/Features/Levels/Scripts/LevelCameraFit.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Levels/Scripts/LevelCameraFit.cs)) · MonoBehaviour · `HoldTheHill.Features.Levels` · HoldTheHill.Runtime · owner: Nilly

Frames the level's map on any screen shape, with bands left free above and below for the HUD, so the top and bottom rows are not under the Honeydew line, the Qu…

Derives from / implements: MonoBehaviour

Used on: L00_TutorialHill_Gameplay, L01_Meadow_Gameplay

Named in: AlphaAlignmentBuilder, AlphaPlaytestToolsBuilder, LevelCameraFitTests

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_halfWidth` | `float` | `10f` |  | L00_TutorialHill_Gameplay = 10; L01_Meadow_Gameplay = 10 | Half the map's width to keep on screen, in world units. The spawn and the Queen stand at x -9.5 and 9.5. |
| `_halfHeight` | `float` | `5f` |  | L00_TutorialHill_Gameplay = 5; L01_Meadow_Gameplay = 5 | Half the map's height to keep on screen, in world units. |
| `_bottomBand` | `float` | `0.09f` |  | L00_TutorialHill_Gameplay = 0.09; L01_Meadow_Gameplay = 0.09 | Share of the screen height kept free at the bottom, for the power-up slot and Next Wave. |
| `_topBand` | `float` | `0.15f` |  | L00_TutorialHill_Gameplay = 0.15; L01_Meadow_Gameplay = 0.15 | Share of the screen height kept free at the top, for the Honeydew and wave lines and the Queen bar under them. |
| `_rightBand` | `float` | `0.3f` |  | L00_TutorialHill_Gameplay = 0.3; L01_Meadow_Gameplay = 0.3 | Share of the screen width kept free on the right, for Adam's tower list. 0 uses the whole width. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `OrthographicSize` | Method (static) | `float` | `(float aspect, float halfWidth, float halfHeight, float bottomBand, float topBand, float rightBand = 0f)` | The smallest orthographic size that shows the whole map: its width left of the right band and its height between the two HUD bands. | LevelCameraFitTests |
| `CameraX` | Method (static) | `float` | `(float size, float aspect, float rightBand)` | The camera's x that puts the map's centre in the middle of the space left of the right band. | LevelCameraFitTests |
| `CameraHeight` | Method (static) | `float` | `(float size, float bottomBand, float topBand)` | The camera height that puts the map's centre in the middle of the space between the bands. | LevelCameraFitTests |

Private members not listed: 5.
