# Hazards

Part of the [code bank](README.md), built 2026-11-05 from commit `d6944f7b`.

## Scripts

| Script | Kind | Owner | What it does |
|---|---|---|---|
| [BirdShadow](#birdshadow) | MonoBehaviour | Nilly | A bird's shadow drifting across the map. It is placed on the spot it will strike, flies in from one side, and when it passes over that spot it takes whatever is… |
| [DiatomaceousEarth](#diatomaceousearth) | MonoBehaviour | Nilly | A patch of diatomaceous earth on the map. It first settles: the dust fades in and does nothing yet, which is the warning (time to burrow, or to see which towers… |
| [HazardScheduler](#hazardscheduler) | MonoBehaviour | Nilly | Spawns the level's hazards on their waves, from its LevelHazardSchedule. Lives in the gameplay scene beside the spawner and listens to its wave-started event. |
| [LevelHazardSchedule](#levelhazardschedule) | ScriptableObject | Nilly | When the hazards of one level happen: which wave, which hazard, where, how long after the wave starts. One asset per level, read by the level's HazardScheduler.… |
| [PesticideHuman](#pesticidehuman) | MonoBehaviour | Nilly | The human with the pesticide. A cloud thickens over the map for a warning spell, then every tower that is not burrowed is stunned for a while. Burrow first and … |
| [WaspStinger](#waspstinger) | MonoBehaviour | Nilly | A stinger a wasp dropped. Lies where it fell, then bursts: every tower within reach misses for a while. The sprite swells over the fuse so the player sees it co… |
| [WaspStingerDropper](#waspstingerdropper) | MonoBehaviour | Nilly | Makes a wasp drop a WaspStinger every few seconds while it is alive and in the air. Sits on the Wasp prefab beside WaspEnemy; the wasp class is untouched. |

### BirdShadow

`unity/Assets/_Game/Features/Hazards/Scripts/BirdShadow.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Hazards/Scripts/BirdShadow.cs)) · MonoBehaviour · `HoldTheHill.Features.Hazards` · HoldTheHill.Runtime · owner: Nilly

A bird's shadow drifting across the map. It is placed on the spot it will strike, flies in from one side, and when it passes over that spot it takes whatever is…

Derives from / implements: MonoBehaviour

Used on: Hazard_BirdShadow

Named in: AlphaHazardsBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_travel` | `float` | `12f` |  | Hazard_BirdShadow = 12 | How far from the strike spot the shadow starts and ends, in world units. |
| `_speed` | `float` | `3f` |  | Hazard_BirdShadow = 3 | Drift speed in world units per second. |
| `_radius` | `float` | `0.8f` |  | Hazard_BirdShadow = 0.8 | Everything this close to the strike spot is taken. |
| `_direction` | `Vector2` | `Vector2.right` |  | Hazard_BirdShadow = (1, 0) | Direction of flight. Positive x flies left to right. |

Private members not listed: 10.

### DiatomaceousEarth

`unity/Assets/_Game/Features/Hazards/Scripts/DiatomaceousEarth.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Hazards/Scripts/DiatomaceousEarth.cs)) · MonoBehaviour · `HoldTheHill.Features.Hazards` · HoldTheHill.Runtime · owner: Nilly

A patch of diatomaceous earth on the map. It first settles: the dust fades in and does nothing yet, which is the warning (time to burrow, or to see which towers…

Derives from / implements: MonoBehaviour

Used on: Hazard_DiatomaceousEarth

Named in: AlphaHazardsBuilder, DiatomaceousEarthTests

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_radius` | `float` | `1.2f` |  | Hazard_DiatomaceousEarth = 1.2 | Radius of the patch in world units. The sprite is scaled to match. |
| `_settleSeconds` | `float` | `1.5f` |  |  | Seconds the dust takes to settle, fading in, before it hurts or stuns anything. The warning. |
| `_duration` | `float` | `12f` |  | Hazard_DiatomaceousEarth = 12 | Seconds the patch hurts and stuns once it has settled. |
| `_tickInterval` | `float` | `0.5f` |  | Hazard_DiatomaceousEarth = 0.5 | Seconds between ticks of damage and stun. |
| `_damagePerTick` | `float` | `4f` |  | Hazard_DiatomaceousEarth = 4 | Damage dealt to each enemy inside on every tick. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Radius` | Property | `float` | `=> _radius` | Radius of the patch in world units. |  |
| `Settled` | Property | `bool` | `=> _age >= _settleSeconds` | True once the dust has settled and hurts and stuns. |  |
| `SettleShare` | Method (static) | `float` | `(float age, float settleSeconds)` | How opaque the dust is while it settles, as a share of its settled look: it fades in from a faint haze to full over the settle time, so the player sees where it… | DiatomaceousEarthTests |

Private members not listed: 10.

### HazardScheduler

`unity/Assets/_Game/Features/Hazards/Scripts/HazardScheduler.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Hazards/Scripts/HazardScheduler.cs)) · MonoBehaviour · `HoldTheHill.Features.Hazards` · HoldTheHill.Runtime · owner: Nilly

Spawns the level's hazards on their waves, from its LevelHazardSchedule. Lives in the gameplay scene beside the spawner and listens to its wave-started event.

Derives from / implements: MonoBehaviour

Used on: L00_TutorialHill_Gameplay, L01_Meadow_Gameplay

Named in: AlphaHazardsBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_spawner` | `EnemySpawner` |  |  | L00_TutorialHill_Gameplay = Enemy Spawner (EnemySpawner); L01_Meadow_Gameplay = Enemy Spawner (EnemySpawner) | The level's spawner. Its wave-started event triggers the schedule. |
| `_schedule` | `LevelHazardSchedule` |  |  | L00_TutorialHill_Gameplay = L00_TutorialHill_Hazards; L01_Meadow_Gameplay = L01_Meadow_Hazards | What happens on which wave. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Schedule` | Property | `LevelHazardSchedule` | `=> _schedule` | The schedule in use, for the Inspector and tests. |  |

Private members not listed: 4.

### LevelHazardSchedule

`unity/Assets/_Game/Features/Hazards/Scripts/LevelHazardSchedule.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Hazards/Scripts/LevelHazardSchedule.cs)) · ScriptableObject · `HoldTheHill.Features.Hazards` · HoldTheHill.Runtime · owner: Nilly

When the hazards of one level happen: which wave, which hazard, where, how long after the wave starts. One asset per level, read by the level's HazardScheduler.…

Derives from / implements: ScriptableObject

Used on: L00_TutorialHill_Hazards, L01_Meadow_Hazards

Named in: AlphaHazardsBuilder, HazardScheduler

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `Entries` | `List<Entry>` | `new List<Entry>()` |  | L00_TutorialHill_Hazards = 3 item(s); L01_Meadow_Hazards = 5 item(s) | Every hazard of the level, in any order. |

#### LevelHazardSchedule.Entry (struct)

One scheduled hazard.

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `Wave` | `int` |  |  |  | The wave (from 1) whose start triggers this hazard. |
| `Delay` | `float` |  |  |  | Seconds after the wave starts. |
| `Hazard` | `GameObject` |  |  |  | The hazard prefab to spawn. |
| `Position` | `Vector2` |  |  |  | Where it appears, in world units. A bird strikes here; a patch lies here; the pesticide ignores it. |

### PesticideHuman

`unity/Assets/_Game/Features/Hazards/Scripts/PesticideHuman.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Hazards/Scripts/PesticideHuman.cs)) · MonoBehaviour · `HoldTheHill.Features.Hazards` · HoldTheHill.Runtime · owner: Nilly

The human with the pesticide. A cloud thickens over the map for a warning spell, then every tower that is not burrowed is stunned for a while. Burrow first and …

Derives from / implements: MonoBehaviour

Used on: Hazard_PesticideHuman

Named in: AlphaHazardsBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_warningSeconds` | `float` | `2.5f` |  | Hazard_PesticideHuman = 2.5 | Seconds the cloud thickens before it lands. Time enough to burrow. |
| `_stunSeconds` | `float` | `4f` |  | Hazard_PesticideHuman = 4 | Seconds every unburrowed tower is stunned when the cloud lands. |
| `_cloud` | `SpriteRenderer` |  |  | Hazard_PesticideHuman = Hazard_PesticideHuman/Cloud (SpriteRenderer) | The cloud sprite. Fades in over the warning and out over the stun. |
| `_peakAlpha` | `float` | `0.45f` |  | Hazard_PesticideHuman = 0.45 | How opaque the cloud is at its thickest. |

Private members not listed: 4.

### WaspStinger

`unity/Assets/_Game/Features/Hazards/Scripts/WaspStinger.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Hazards/Scripts/WaspStinger.cs)) · MonoBehaviour · `HoldTheHill.Features.Hazards` · HoldTheHill.Runtime · owner: Nilly

A stinger a wasp dropped. Lies where it fell, then bursts: every tower within reach misses for a while. The sprite swells over the fuse so the player sees it co…

Derives from / implements: MonoBehaviour

Used on: Hazard_WaspStinger

Named in: AlphaHazardsBuilder, WaspStingerDropper

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_fuse` | `float` | `2f` |  | Hazard_WaspStinger = 2 | Seconds between landing and bursting. |
| `_radius` | `float` | `1.5f` |  | Hazard_WaspStinger = 1.5 | Towers this close miss when the stinger bursts. |
| `_missDuration` | `float` | `4f` |  | Hazard_WaspStinger = 4 | Seconds the towers caught in the burst keep missing. |
| `_swell` | `float` | `1.8f` |  | Hazard_WaspStinger = 1.8 | How much bigger the sprite is drawn at the moment it bursts. |

Private members not listed: 5.

### WaspStingerDropper

`unity/Assets/_Game/Features/Hazards/Scripts/WaspStingerDropper.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Hazards/Scripts/WaspStingerDropper.cs)) · MonoBehaviour · `HoldTheHill.Features.Hazards` · HoldTheHill.Runtime · owner: Nilly

Makes a wasp drop a WaspStinger every few seconds while it is alive and in the air. Sits on the Wasp prefab beside WaspEnemy; the wasp class is untouched.

Derives from / implements: MonoBehaviour

Used on: Enemy_Wasp

Named in: AlphaHazardsBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_stingerPrefab` | `WaspStinger` |  |  | Enemy_Wasp = Hazard_WaspStinger › WaspStinger | The stinger left on the ground. |
| `_firstDropDelay` | `float` | `2f` |  | Enemy_Wasp = 2 | Seconds after spawning before the first stinger. |
| `_interval` | `float` | `3.5f` |  | Enemy_Wasp = 3.5 | Seconds between stingers after the first. |

Private members not listed: 5.

## Prefabs

### Hazard_BirdShadow

`unity/Assets/_Game/Features/Hazards/Prefabs/Hazard_BirdShadow.prefab` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Hazards/Prefabs/Hazard_BirdShadow.prefab)) · Prefab

BirdShadow: A bird's shadow drifting across the map. It is placed on the spot it will strike, flies in from one side, and when it passes over that spot it takes…

Made by: AlphaHazardsBuilder (Tools/Hold the Hill/Alpha/Build Hazards (burrow button, four hazards, schedules))

Used in: L00_TutorialHill_Hazards, L01_Meadow_Hazards

**Hazard_BirdShadow** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (1.8, 1.8, 1.8) |  |

**Hazard_BirdShadow** · SpriteRenderer

| Field | Value | Points to |
|---|---|---|
| `m_Sprite` | Circle | Sprite: unity/Assets/_Game/Shared/Art/Placeholders/Circle.png |
| `m_Color` | (0, 0, 0, 0.45) |  |
| `m_SortingOrder` | 50 |  |
| `m_SortingLayer` | 0 |  |

**Hazard_BirdShadow** · BirdShadow

| Field | Value | Points to |
|---|---|---|
| `_travel` | 12 |  |
| `_speed` | 3 |  |
| `_radius` | 0.8 |  |
| `_direction` | (1, 0) |  |

### Hazard_DiatomaceousEarth

`unity/Assets/_Game/Features/Hazards/Prefabs/Hazard_DiatomaceousEarth.prefab` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Hazards/Prefabs/Hazard_DiatomaceousEarth.prefab)) · Prefab

DiatomaceousEarth: A patch of diatomaceous earth on the map. It first settles: the dust fades in and does nothing yet, which is the warning (time to burrow, or …

Made by: AlphaHazardsBuilder (Tools/Hold the Hill/Alpha/Build Hazards (burrow button, four hazards, schedules))

Used in: L00_TutorialHill_Hazards, L01_Meadow_Hazards

**Hazard_DiatomaceousEarth** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (1, 1, 1) |  |

**Hazard_DiatomaceousEarth** · SpriteRenderer

| Field | Value | Points to |
|---|---|---|
| `m_Sprite` | Circle | Sprite: unity/Assets/_Game/Shared/Art/Placeholders/Circle.png |
| `m_Color` | (0.92, 0.9, 0.78, 0.55) |  |
| `m_SortingOrder` | -5 |  |
| `m_SortingLayer` | 0 |  |

**Hazard_DiatomaceousEarth** · DiatomaceousEarth

| Field | Value | Points to |
|---|---|---|
| `_radius` | 1.2 |  |
| `_duration` | 12 |  |
| `_tickInterval` | 0.5 |  |
| `_damagePerTick` | 4 |  |

### Hazard_PesticideHuman

`unity/Assets/_Game/Features/Hazards/Prefabs/Hazard_PesticideHuman.prefab` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Hazards/Prefabs/Hazard_PesticideHuman.prefab)) · Prefab

PesticideHuman: The human with the pesticide. A cloud thickens over the map for a warning spell, then every tower that is not burrowed is stunned for a while. B…

Made by: AlphaHazardsBuilder (Tools/Hold the Hill/Alpha/Build Hazards (burrow button, four hazards, schedules))

Used in: L00_TutorialHill_Hazards, L01_Meadow_Hazards

**Hazard_PesticideHuman** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (1, 1, 1) |  |

**Hazard_PesticideHuman** · PesticideHuman

| Field | Value | Points to |
|---|---|---|
| `_warningSeconds` | 2.5 |  |
| `_stunSeconds` | 4 |  |
| `_cloud` | Hazard_PesticideHuman/Cloud (SpriteRenderer) | Component: unity/Assets/_Game/Features/Hazards/Prefabs/Hazard_PesticideHuman.prefab |
| `_peakAlpha` | 0.45 |  |

**Hazard_PesticideHuman/Cloud** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (24, 14, 1) |  |

**Hazard_PesticideHuman/Cloud** · SpriteRenderer

| Field | Value | Points to |
|---|---|---|
| `m_Sprite` | Square | Sprite: unity/Assets/_Game/Shared/Art/Placeholders/Square.png |
| `m_Color` | (0.75, 1, 0.6, 0) |  |
| `m_SortingOrder` | 60 |  |
| `m_SortingLayer` | 0 |  |

### Hazard_WaspStinger

`unity/Assets/_Game/Features/Hazards/Prefabs/Hazard_WaspStinger.prefab` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Hazards/Prefabs/Hazard_WaspStinger.prefab)) · Prefab

WaspStinger: A stinger a wasp dropped. Lies where it fell, then bursts: every tower within reach misses for a while. The sprite swells over the fuse so the play…

Made by: AlphaHazardsBuilder (Tools/Hold the Hill/Alpha/Build Hazards (burrow button, four hazards, schedules))

Used in: Enemy_Wasp

**Hazard_WaspStinger** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (0.25, 0.25, 0.25) |  |

**Hazard_WaspStinger** · SpriteRenderer

| Field | Value | Points to |
|---|---|---|
| `m_Sprite` | Circle | Sprite: unity/Assets/_Game/Shared/Art/Placeholders/Circle.png |
| `m_Color` | (0.25, 0.18, 0.05, 1) |  |
| `m_SortingOrder` | 8 |  |
| `m_SortingLayer` | 0 |  |

**Hazard_WaspStinger** · WaspStinger

| Field | Value | Points to |
|---|---|---|
| `_fuse` | 2 |  |
| `_radius` | 1.5 |  |
| `_missDuration` | 4 |  |
| `_swell` | 1.8 |  |

## Data assets

### L00_TutorialHill_Hazards

`unity/Assets/_Game/Features/Hazards/Data/L00_TutorialHill_Hazards.asset` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Hazards/Data/L00_TutorialHill_Hazards.asset)) · Data asset

Type: `LevelHazardSchedule`: When the hazards of one level happen: which wave, which hazard, where, how long after the wave starts. One asset per level, read by the level's HazardScheduler.…
Made by: AlphaHazardsBuilder (Tools/Hold the Hill/Alpha/Build Hazards (burrow button, four hazards, schedules))

Used by: L00_TutorialHill_Gameplay

**LevelHazardSchedule**

| Field | Value | Points to |
|---|---|---|
| `Entries` | 3 item(s) |  |
| `Entries[0].Wave` | 3 |  |
| `Entries[0].Delay` | 2 |  |
| `Entries[0].Hazard` | Hazard_DiatomaceousEarth | Prefab: unity/Assets/_Game/Features/Hazards/Prefabs/Hazard_DiatomaceousEarth.prefab |
| `Entries[0].Position` | (3.5, -2.5) |  |
| `Entries[1].Wave` | 4 |  |
| `Entries[1].Delay` | 6 |  |
| `Entries[1].Hazard` | Hazard_PesticideHuman | Prefab: unity/Assets/_Game/Features/Hazards/Prefabs/Hazard_PesticideHuman.prefab |
| `Entries[1].Position` | (0, 0) |  |
| `Entries[2].Wave` | 5 |  |
| `Entries[2].Delay` | 4 |  |
| `Entries[2].Hazard` | Hazard_BirdShadow | Prefab: unity/Assets/_Game/Features/Hazards/Prefabs/Hazard_BirdShadow.prefab |
| `Entries[2].Position` | (-4.5, 0.5) |  |

### L01_Meadow_Hazards

`unity/Assets/_Game/Features/Hazards/Data/L01_Meadow_Hazards.asset` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Hazards/Data/L01_Meadow_Hazards.asset)) · Data asset

Type: `LevelHazardSchedule`: When the hazards of one level happen: which wave, which hazard, where, how long after the wave starts. One asset per level, read by the level's HazardScheduler.…
Made by: AlphaHazardsBuilder (Tools/Hold the Hill/Alpha/Build Hazards (burrow button, four hazards, schedules))

Used by: L01_Meadow_Gameplay

**LevelHazardSchedule**

| Field | Value | Points to |
|---|---|---|
| `Entries` | 5 item(s) |  |
| `Entries[0].Wave` | 3 |  |
| `Entries[0].Delay` | 3 |  |
| `Entries[0].Hazard` | Hazard_DiatomaceousEarth | Prefab: unity/Assets/_Game/Features/Hazards/Prefabs/Hazard_DiatomaceousEarth.prefab |
| `Entries[0].Position` | (-3.5, -2.5) |  |
| `Entries[1].Wave` | 5 |  |
| `Entries[1].Delay` | 5 |  |
| `Entries[1].Hazard` | Hazard_BirdShadow | Prefab: unity/Assets/_Game/Features/Hazards/Prefabs/Hazard_BirdShadow.prefab |
| `Entries[1].Position` | (1.5, 3.5) |  |
| `Entries[2].Wave` | 6 |  |
| `Entries[2].Delay` | 8 |  |
| `Entries[2].Hazard` | Hazard_PesticideHuman | Prefab: unity/Assets/_Game/Features/Hazards/Prefabs/Hazard_PesticideHuman.prefab |
| `Entries[2].Position` | (0, 0) |  |
| `Entries[3].Wave` | 8 |  |
| `Entries[3].Delay` | 2 |  |
| `Entries[3].Hazard` | Hazard_DiatomaceousEarth | Prefab: unity/Assets/_Game/Features/Hazards/Prefabs/Hazard_DiatomaceousEarth.prefab |
| `Entries[3].Position` | (6.5, -2.5) |  |
| `Entries[4].Wave` | 9 |  |
| `Entries[4].Delay` | 6 |  |
| `Entries[4].Hazard` | Hazard_BirdShadow | Prefab: unity/Assets/_Game/Features/Hazards/Prefabs/Hazard_BirdShadow.prefab |
| `Entries[4].Position` | (-6.5, 0.5) |  |
