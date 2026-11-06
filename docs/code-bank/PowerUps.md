# PowerUps

Part of the [code bank](README.md), built 2026-11-05 from commit `d6944f7b`.

## Scripts

| Script | Kind | Owner | What it does |
|---|---|---|---|
| [LevelPowerUps](#levelpowerups) | MonoBehaviour | Nilly | The run's power-ups, in the level's gameplay scene. Rolls a drop when an enemy dies (through EnemyDied, so Jason's enemy class is untouched), lets a click on th… |
| [PowerUpKind](#powerupkind) | Enum | Nilly | The two power-ups of the Alpha. Nothing else drops. |
| [PowerUpPickup](#poweruppickup) | MonoBehaviour | Nilly | A power-up lying on the map where an enemy died. Bobs so it is seen, fades away after its lifetime, and says whether a click lands on it. LevelPowerUps makes an… |
| [PowerUpRule](#poweruprule) | Static class | Nilly | The power-up rules as plain C#, so EditMode tests can cover them: whether a kill drops one and which, and whether a Royal Gift can do anything for a tower. Leve… |
| [PowerUpSettings](#powerupsettings) | ScriptableObject | Nilly | The numbers behind power-ups: how often they drop, how long a drop waits on the map, and what each one does. One asset per game; data, not code (UNITY_RULES 32)… |
| [PowerUpSlot](#powerupslot) | Class | Nilly | The one HUD slot: holds at most one collected power-up until it is used. Plain C#, so the rule can be tested; LevelPowerUps owns one. A drop cannot be collected… |

### LevelPowerUps

`unity/Assets/_Game/Features/PowerUps/Scripts/LevelPowerUps.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/PowerUps/Scripts/LevelPowerUps.cs)) · MonoBehaviour · `HoldTheHill.Features.PowerUps` · HoldTheHill.Runtime · owner: Nilly

The run's power-ups, in the level's gameplay scene. Rolls a drop when an enemy dies (through EnemyDied, so Jason's enemy class is untouched), lets a click on th…

Derives from / implements: MonoBehaviour

Used on: L00_TutorialHill_Gameplay, L01_Meadow_Gameplay

Named in: AlphaPowerUpsBuilder, LevelRun, PowerUpSlotPanel

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_settings` | `PowerUpSettings` |  |  | L00_TutorialHill_Gameplay = PowerUpSettings; L01_Meadow_Gameplay = PowerUpSettings | Drop chance, lifetimes, what each power-up does, and how they look. |
| `_pickupPrefab` | `PowerUpPickup` |  |  | L00_TutorialHill_Gameplay = PowerUp_Pickup › PowerUpPickup; L01_Meadow_Gameplay = PowerUp_Pickup › PowerUpPickup | The drop that appears where an enemy died. |
| `_placer` | `TowerPlacer` |  |  | L00_TutorialHill_Gameplay = Tower Placer (TowerPlacer); L01_Meadow_Gameplay = Tower Placer (TowerPlacer) | The level's placer, to find the tower a Royal Gift is aimed at. |
| `_actions` | `InputActionAsset` |  |  | L00_TutorialHill_Gameplay = HoldTheHill; L01_Meadow_Gameplay = HoldTheHill | The game's actions asset. Uses its Gameplay map: Point, Place, Cancel. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Changed` | Event | `Action` |  | Raised when the slot, the gift targeting or the frenzy state changes. The HUD listens. | LevelRun, PowerUpSlotPanel |
| `GiftRefused` | Event | `Action` |  | Raised when a Royal Gift was aimed at a tower already at its last level, so the HUD can say so. | PowerUpSlotPanel |
| `SlotFull` | Event | `Action` |  | Raised when a click lands on a drop while the slot is already full, so the HUD can say so. | PowerUpSlotPanel |
| `Settings` | Property | `PowerUpSettings` | `=> _settings` | The numbers and looks. The HUD reads icons from here. | PowerUpSlotPanel |
| `Slot` | Property | `PowerUpSlot` | `=> _slot` | The one slot. | PowerUpSlotPanel |
| `PickingGiftTarget` | Property | `bool` | `{ get; private set; }` | True while a used Royal Gift waits for the player to click a tower. | PowerUpSlotPanel |
| `FrenzySecondsLeft` | Property | `float` | `=> _frenzyLeft` | Seconds of Swarm Frenzy left, or 0. | PowerUpSlotPanel |
| `FrenzyActive` | Property | `bool` | `=> _frenzyLeft > 0f` |  | PowerUpSlotPanel |
| `AcceptsInput` | Property | `bool` | `{ get; set; }` | False while the run is paused or over: clicks and right-click then leave drops and the gift pick alone. The level's run sets it, as it does for the placer. | LevelRun |
| `CollectAt` | Method | `bool` | `(Vector3 world)` | Collects the drop under world into the slot. A full slot leaves the drop where it is and raises SlotFull. True if something was collected. |  |
| `PickupAt` | Method | `PowerUpPickup` | `(Vector3 world)` | The first living drop a click at world lands on, or null. |  |
| `Use` | Method | `void` | `()` | The HUD slot was clicked. Swarm Frenzy starts at once; Royal Gift starts (or stops) waiting for a tower to be clicked. Nothing happens with an empty slot. | LevelRun, PowerUpSlotPanel |

Private members not listed: 17.

### PowerUpKind

`unity/Assets/_Game/Features/PowerUps/Scripts/PowerUpKind.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/PowerUps/Scripts/PowerUpKind.cs)) · Enum · `HoldTheHill.Features.PowerUps` · HoldTheHill.Runtime · owner: Nilly

The two power-ups of the Alpha. Nothing else drops.

Named in: LevelPowerUps, PowerUpPickup, PowerUpRule, PowerUpRuleTests, PowerUpSettings, PowerUpSlot, PowerUpSlotPanel, PowerUpSlotTests

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `SwarmFrenzy` | Enum member (static) | `PowerUpKind` |  | Every tower fires faster for a while. | PowerUpRuleTests, PowerUpSettings, PowerUpSlotPanel, PowerUpSlotTests |
| `RoyalGift` | Enum member (static) | `PowerUpKind` |  | Pick a standing tower; it gets its next level for free. | LevelPowerUps, PowerUpRuleTests, PowerUpSlotPanel, PowerUpSlotTests |

### PowerUpPickup

`unity/Assets/_Game/Features/PowerUps/Scripts/PowerUpPickup.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/PowerUps/Scripts/PowerUpPickup.cs)) · MonoBehaviour · `HoldTheHill.Features.PowerUps` · HoldTheHill.Runtime · owner: Nilly

A power-up lying on the map where an enemy died. Bobs so it is seen, fades away after its lifetime, and says whether a click lands on it. LevelPowerUps makes an…

Derives from / implements: MonoBehaviour

Used on: PowerUp_Pickup

Named in: AlphaPowerUpsBuilder, LevelPowerUps

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_pickRadius` | `float` | `0.45f` |  | PowerUp_Pickup = 0.45 | A click within this distance of the centre, in world units, collects the drop. |
| `_bobHeight` | `float` | `0.12f` |  | PowerUp_Pickup = 0.12 | How far the drop bobs up and down, in world units. |
| `_bobRate` | `float` | `1.5f` |  | PowerUp_Pickup = 1.5 | Bobs per second. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Kind` | Property | `PowerUpKind` | `{ get; private set; }` | Which power-up this drop gives. | LevelPowerUps |
| `Init` | Method | `void` | `(PowerUpKind kind, Sprite icon, Color color, float lifetime)` | Sets what the drop is and how it looks. Called right after it is spawned. | LevelPowerUps |
| `Covers` | Method | `bool` | `(Vector3 world)` | True if a click at world lands on this drop. | LevelPowerUps |

Private members not listed: 6.

### PowerUpRule

`unity/Assets/_Game/Features/PowerUps/Scripts/PowerUpRule.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/PowerUps/Scripts/PowerUpRule.cs)) · Static class · `HoldTheHill.Features.PowerUps` · HoldTheHill.Runtime · owner: Nilly

The power-up rules as plain C#, so EditMode tests can cover them: whether a kill drops one and which, and whether a Royal Gift can do anything for a tower. Leve…

Named in: LevelPowerUps, PowerUpRuleTests

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `KindCount` | Field (static readonly) | `int` |  | How many kinds a roll is split between. | PowerUpRuleTests |
| `RollDrop` | Method (static) | `bool` | `(double roll, float dropChance, out PowerUpKind kind)` | Decides from one random roll in [0, 1) whether a kill drops a power-up and which: a roll under dropChance drops, and that range is split evenly between the kind… | LevelPowerUps, PowerUpRuleTests |
| `CanGift` | Method (static) | `bool` | `(int level, int maxLevel)` | A Royal Gift is a free upgrade, so it helps any tower that is not at its last level. Reuses the upgrade rule with nothing to pay. | LevelPowerUps, PowerUpRuleTests |

### PowerUpSettings

`unity/Assets/_Game/Features/PowerUps/Scripts/PowerUpSettings.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/PowerUps/Scripts/PowerUpSettings.cs)) · ScriptableObject · `HoldTheHill.Features.PowerUps` · HoldTheHill.Runtime · owner: Nilly

The numbers behind power-ups: how often they drop, how long a drop waits on the map, and what each one does. One asset per game; data, not code (UNITY_RULES 32)…

Derives from / implements: ScriptableObject

Used on: PowerUpSettings

Named in: AlphaPowerUpsBuilder, LevelPowerUps

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `DropChance` | `float` | `0.08f` | Drops | PowerUpSettings = 0.08 | Chance, 0 to 1, that a killed enemy leaves a power-up behind. Which one is an even split. |
| `DropLifetime` | `float` | `15f` | Drops | PowerUpSettings = 15 | Seconds a drop waits on the map before it fades away. |
| `FrenzyDuration` | `float` | `10f` | Swarm Frenzy | PowerUpSettings = 10 | Seconds every tower fires faster after the slot is used. |
| `FrenzyFireRateFactor` | `float` | `2f` | Swarm Frenzy | PowerUpSettings = 2 | How many times faster towers fire during a frenzy. 2 halves the time between attacks. |
| `FrenzyIcon` | `Sprite` |  | Looks | PowerUpSettings = Circle | Sprite for a Swarm Frenzy drop and its HUD slot. |
| `GiftIcon` | `Sprite` |  | Looks | PowerUpSettings = Square | Sprite for a Royal Gift drop and its HUD slot. A different shape from the frenzy, not only a different colour. |
| `FrenzyColor` | `Color` | `new Color(1f, 0.75f, 0.2f)` | Looks | PowerUpSettings = (1, 0.75, 0.2, 1) |  |
| `GiftColor` | `Color` | `new Color(0.75f, 0.45f, 1f)` | Looks | PowerUpSettings = (0.75, 0.45, 1, 1) |  |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `IconFor` | Method | `Sprite` | `(PowerUpKind kind)` | The sprite for kind. | LevelPowerUps |
| `ColorFor` | Method | `Color` | `(PowerUpKind kind)` | The colour for kind. | LevelPowerUps |

### PowerUpSlot

`unity/Assets/_Game/Features/PowerUps/Scripts/PowerUpSlot.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/PowerUps/Scripts/PowerUpSlot.cs)) · Class · `HoldTheHill.Features.PowerUps` · HoldTheHill.Runtime · owner: Nilly

The one HUD slot: holds at most one collected power-up until it is used. Plain C#, so the rule can be tested; LevelPowerUps owns one. A drop cannot be collected…

Named in: LevelPowerUps, PowerUpSlotTests

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Changed` | Event | `Action` |  | Raised when the slot fills or empties. | PowerUpSlotTests |
| `Held` | Property | `PowerUpKind?` | `{ get; private set; }` | What the slot holds, or null when empty. | LevelPowerUps, PowerUpSlotTests |
| `IsEmpty` | Property | `bool` | `=> Held == null` |  | PowerUpSlotTests |
| `TryStore` | Method | `bool` | `(PowerUpKind kind)` | Puts kind in the slot. False, and nothing changes, when it is full. | LevelPowerUps, PowerUpSlotTests |
| `Take` | Method | `PowerUpKind?` | `()` | Empties the slot and hands back what it held, or null when it was empty. | LevelPowerUps, PowerUpSlotTests |

## Prefabs

### PowerUp_Pickup

`unity/Assets/_Game/Features/PowerUps/Prefabs/PowerUp_Pickup.prefab` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/PowerUps/Prefabs/PowerUp_Pickup.prefab)) · Prefab

PowerUpPickup: A power-up lying on the map where an enemy died. Bobs so it is seen, fades away after its lifetime, and says whether a click lands on it. LevelPo…

Made by: AlphaPowerUpsBuilder (Tools/Hold the Hill/Alpha/Build Power-ups (drops, slot, Swarm Frenzy, Royal Gift))

Used in: L00_TutorialHill_Gameplay, L01_Meadow_Gameplay

**PowerUp_Pickup** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (0.45, 0.45, 0.45) |  |

**PowerUp_Pickup** · SpriteRenderer

| Field | Value | Points to |
|---|---|---|
| `m_Sprite` | Circle | Sprite: unity/Assets/_Game/Shared/Art/Placeholders/Circle.png |
| `m_Color` | (1, 1, 1, 1) |  |
| `m_SortingOrder` | 30 |  |
| `m_SortingLayer` | 0 |  |

**PowerUp_Pickup** · PowerUpPickup

| Field | Value | Points to |
|---|---|---|
| `_pickRadius` | 0.45 |  |
| `_bobHeight` | 0.12 |  |
| `_bobRate` | 1.5 |  |

## Data assets

### PowerUpSettings

`unity/Assets/_Game/Features/PowerUps/Data/PowerUpSettings.asset` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/PowerUps/Data/PowerUpSettings.asset)) · Data asset

Type: `PowerUpSettings`: The numbers behind power-ups: how often they drop, how long a drop waits on the map, and what each one does. One asset per game; data, not code (UNITY_RULES 32)…
Made by: AlphaPowerUpsBuilder (Tools/Hold the Hill/Alpha/Build Power-ups (drops, slot, Swarm Frenzy, Royal Gift))

Used by: L00_TutorialHill_Gameplay, L01_Meadow_Gameplay

**PowerUpSettings**

| Field | Value | Points to |
|---|---|---|
| `DropChance` | 0.08 |  |
| `DropLifetime` | 15 |  |
| `FrenzyDuration` | 10 |  |
| `FrenzyFireRateFactor` | 2 |  |
| `FrenzyIcon` | Circle | Sprite: unity/Assets/_Game/Shared/Art/Placeholders/Circle.png |
| `GiftIcon` | Square | Sprite: unity/Assets/_Game/Shared/Art/Placeholders/Square.png |
| `FrenzyColor` | (1, 0.75, 0.2, 1) |  |
| `GiftColor` | (0.75, 0.45, 1, 1) |  |
