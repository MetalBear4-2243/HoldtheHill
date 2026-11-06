# Towers

Part of the [code bank](README.md), built 2026-11-05 from commit `d6944f7b`.

## Scripts

| Script | Kind | Owner | What it does |
|---|---|---|---|
| [ITowerAbility](#itowerability) | Interface | Nilly | A component that does a tower's work in place of its own attack, such as laying mines or slowing enemies. The Tower beside it keeps its numbers (range, damage, … |
| [Mine](#mine) | MonoBehaviour | Nilly | A mine laid by the Carpenter Ant. Waits on the path; the first living enemy to come within its trigger radius sets it off, and everything within the blast radiu… |
| [MineLayer](#minelayer) | MonoBehaviour | Nilly | The Carpenter Ant: keeps a few Mines lying on the stretch of path inside the tower's range, laying a new one on the tower's cooldown whenever one has gone off. … |
| [Projectile](#projectile) | MonoBehaviour | Nilly | One shot from a ranged tower. A straight shot flies at its target and deals the tower's damage on arrival; if the target dies or reaches the Queen first it carr… |
| [RangeRing](#rangering) | MonoBehaviour | Nilly | A thin circle showing how far a tower reaches. The placer shows one around the ghost before a tower is bought, and around the selected tower, so the player sees… |
| [SlowAura](#slowaura) | MonoBehaviour | Nilly | The Sap Ant: slows every enemy inside the tower's range. Uses Jason's status effects on BasicEnemy, re-applied each tick for a little longer than the tick, so a… |
| [TargetPicker](#targetpicker) | Static class | Nilly | The targeting rules, as plain C# so EditMode tests can cover them: how far along the path an enemy is, and which of several candidates a TargetingPriority picks… |
| [TargetingPriority](#targetingpriority) | Enum | Nilly | Which enemy a tower attacks when several are in range. The player sets it per tower from the tower card (the GDD's standing orders). |
| [Tower](#tower) | MonoBehaviour | Nilly | A placed tower. Knows its grid cell, picks a target in range by its TargetingPriority, and attacks it on a cooldown: with a projectile when it has one, otherwis… |
| [TowerAttackAnimation](#towerattackanimation) | MonoBehaviour | Nilly | Plays the tower's attack row from its sprite sheet over the idle loop. A tower that attacks on its own winds up just before it is ready, so the hit frame shows … |
| [TowerAttackTiming](#towerattacktiming) | Static class | Nilly | The timing of a tower's attack animation, as pure functions so EditMode tests cover it. The art's attack row is wind-up frames, then the hit frame, then follow-… |
| [TowerClickRule](#towerclickrule) | Static class | Nilly | What a left-click on the map does. Plain C# so EditMode tests can cover it; TowerPlacer applies the answer. A standing tower under the cursor is always selected… |
| [TowerCondition](#towercondition) | Class | Nilly | What is being done to a tower, and what that allows: burrowed (the player's order), stunned (a hazard, for a while) or missing (a hazard, for a while). Plain C#… |
| [TowerData](#towerdata) | ScriptableObject | Adam | What the shop shows for one tower: its name, price and icon. The placer's offers and the shop cards both point at these assets (in Features/Towers/Data). |
| [TowerPlacer](#towerplacer) | MonoBehaviour | Nilly | Places and selects towers. A tower is picked from the shop (Select) or with the number keys; a ghost then follows the mouse, snapped to the grid. Left-click bui… |
| [TowerRegistry](#towerregistry) | Static class | Nilly | Every standing Tower, so hazards can find the towers under them without a scene search. The mirror of EnemyRegistry; towers add themselves. |
| [TowerSellRule](#towersellrule) | Static class | Nilly | What the player gets back for selling a tower: a fraction of every Honeydew spent on it (the build price and each upgrade), rounded down. Plain C# so EditMode t… |
| [TowerStunRing](#towerstunring) | MonoBehaviour | Nilly | A ring around a stunned tower that empties like a clock as the stun wears off, so the player can see when the tower fights again. Hidden while the tower is not … |
| [TowerTempo](#towertempo) | Static class | Nilly | How fast every tower fires right now, as one factor: 1 is normal, 2 is twice as fast. Tower divides its cooldown by it in one place; the Swarm Frenzy power-up s… |
| [TowerUpgrade](#towerupgrade) | Struct | Nilly | One upgrade step of a tower: what it costs and the numbers the tower has afterwards. A tower prefab lists two of these for levels 2 and 3. Data, not code (UNITY… |
| [TowerUpgradeRule](#towerupgraderule) | Static class | Nilly | Whether a tower may be upgraded right now. Plain C# so EditMode tests can cover it; UpgradeSelectedTower applies the answer. |

### ITowerAbility

`unity/Assets/_Game/Features/Towers/Scripts/ITowerAbility.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Scripts/ITowerAbility.cs)) · Interface · `HoldTheHill.Features.Towers` · HoldTheHill.Runtime · owner: Nilly

A component that does a tower's work in place of its own attack, such as laying mines or slowing enemies. The Tower beside it keeps its numbers (range, damage, …

Named in: MineLayer, SlowAura, Tower, TowerRangeDataTests

### Mine

`unity/Assets/_Game/Features/Towers/Scripts/Mine.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Scripts/Mine.cs)) · MonoBehaviour · `HoldTheHill.Features.Towers` · HoldTheHill.Runtime · owner: Nilly

A mine laid by the Carpenter Ant. Waits on the path; the first living enemy to come within its trigger radius sets it off, and everything within the blast radiu…

Derives from / implements: MonoBehaviour

Used on: Mine_Carpenter

Named in: AlphaCoreLoopBuilder, MineLayer

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_triggerRadius` | `float` | `0.35f` |  | Mine_Carpenter = 0.35 | An enemy this close sets the mine off. |
| `_blastRadius` | `float` | `0.9f` |  | Mine_Carpenter = 0.9 | Every enemy this close to the mine takes the damage when it goes off. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Exploded` | Event | `System.Action<Mine>` |  | Raised when the mine goes off. The layer uses it to count its mines. | MineLayer |
| `Arm` | Method | `void` | `(double damage)` | Arms the mine with the damage it will deal. Called by the layer. | MineLayer |

Private members not listed: 6.

### MineLayer

`unity/Assets/_Game/Features/Towers/Scripts/MineLayer.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Scripts/MineLayer.cs)) · MonoBehaviour · `HoldTheHill.Features.Towers` · HoldTheHill.Runtime · owner: Nilly

The Carpenter Ant: keeps a few Mines lying on the stretch of path inside the tower's range, laying a new one on the tower's cooldown whenever one has gone off. …

Derives from / implements: MonoBehaviour, ITowerAbility

Used on: Tower_CarpenterAnt

Named in: AlphaCoreLoopBuilder, AlphaTowersBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_minePrefab` | `Mine` |  |  | Tower_CarpenterAnt = Mine_Carpenter › Mine | The mine laid on the path. |
| `_maxMines` | `int` | `3` |  | Tower_CarpenterAnt = 3 | How many of this tower's mines can lie on the path at once. |
| `_spotSpacing` | `float` | `0.5f` |  | Tower_CarpenterAnt = 0.5 | Gap in world units between the candidate spots sampled along the path. |
| `_path` | `EnemyPath` |  |  | Tower_CarpenterAnt = (none) | The path mines go on. Found automatically if left empty. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Spots` | Property | `IReadOnlyList<Vector3>` | `=> _spots` | Points on the path inside range where a mine may be laid. Empty if none. |  |

Private members not listed: 12.

### Projectile

`unity/Assets/_Game/Features/Towers/Scripts/Projectile.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Scripts/Projectile.cs)) · MonoBehaviour · `HoldTheHill.Features.Towers` · HoldTheHill.Runtime · owner: Nilly

One shot from a ranged tower. A straight shot flies at its target and deals the tower's damage on arrival; if the target dies or reaches the Queen first it carr…

Derives from / implements: MonoBehaviour

Used on: Projectile_Bullet, Projectile_Lob

Named in: AlphaCoreLoopBuilder, Tower

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_speed` | `float` | `10f` |  | Projectile_Bullet = 10; Projectile_Lob = 6 | Travel speed in world units per second. |
| `_hitRadius` | `float` | `0.2f` |  | Projectile_Bullet = 0.2; Projectile_Lob = 0.2 | How close the shot must get to the target's centre to count as a hit. |
| `_lifetime` | `float` | `3f` |  | Projectile_Bullet = 3; Projectile_Lob = 3 | Seconds before a shot that missed gives up. |
| `_lobbed` | `bool` |  | Lobbed | Projectile_Lob = 1 | If on, the shot arcs onto its target and splashes there instead of flying straight at it. |
| `_arcHeight` | `float` | `1.5f` | Lobbed | Projectile_Lob = 1.5 | Lobbed only: how high the arc is drawn above the straight line, in world units. |
| `_splashRadius` | `float` | `1f` | Lobbed | Projectile_Lob = 1 | Lobbed only: every enemy within this distance of the landing point takes the damage. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `IsLobbed` | Property | `bool` | `=> _lobbed` | True for a shot that lands and splashes rather than chasing its target. | Tower |
| `Launch` | Method | `void` | `(BasicEnemy target, double damage)` | Aims the shot. Called by the tower right after it is spawned. | Tower |
| `LobLanding` | Method (static) | `Vector3` | `(Vector3 target, float z)` | Where a lob comes down for a target at target: on it, in the shot's own plane. |  |

Private members not listed: 12.

### RangeRing

`unity/Assets/_Game/Features/Towers/Scripts/RangeRing.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Scripts/RangeRing.cs)) · MonoBehaviour · `HoldTheHill.Features.Towers` · HoldTheHill.Runtime · owner: Nilly

A thin circle showing how far a tower reaches. The placer shows one around the ghost before a tower is bought, and around the selected tower, so the player sees…

Derives from / implements: MonoBehaviour

Named in: RangeRingTests, TowerPlacer

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Segments` | Const | `int` |  | Points on the circle; enough to look round at the largest tower range. | RangeRingTests |
| `Width` | Const (private) | `float` |  |  |  |
| `Create` | Method (static) | `RangeRing` | `(string name, Scene scene, SpriteRenderer look, Color color, int sortingOrder)` | Makes a hidden ring in scene, so it unloads with the level. It is drawn with look's material and sorting layer, at sortingOrder. | TowerPlacer |
| `Show` | Method | `void` | `(Vector3 centre, float radius)` | Shows the ring around centre; a radius of zero or less hides it. | TowerPlacer |
| `Hide` | Method | `void` | `()` | Hides the ring. | TowerPlacer |
| `CirclePoints` | Method (static) | `void` | `(Vector3 centre, float radius, Vector3[] into)` | Fills into with evenly spaced points on the circle, at the centre's z. | RangeRingTests |

Private members not listed: 5.

### SlowAura

`unity/Assets/_Game/Features/Towers/Scripts/SlowAura.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Scripts/SlowAura.cs)) · MonoBehaviour · `HoldTheHill.Features.Towers` · HoldTheHill.Runtime · owner: Nilly

The Sap Ant: slows every enemy inside the tower's range. Uses Jason's status effects on BasicEnemy, re-applied each tick for a little longer than the tick, so a…

Derives from / implements: MonoBehaviour, ITowerAbility

Used on: Tower_SapAnt

Named in: AlphaCoreLoopBuilder, AlphaTowersBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_slowMultiplier` | `float` | `0.5f` |  | Tower_SapAnt = 0.5 | Movement speed while inside the aura. 0.5 is half speed. |
| `_tickInterval` | `float` | `0.25f` |  | Tower_SapAnt = 0.25 | Seconds between re-applying the slow to everyone in range. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `EffectName` | Const (private) | `string` |  |  |  |
| `SlowMultiplier` | Property | `float` | `=> _slowMultiplier` | Movement speed multiplier applied inside the aura. |  |

Private members not listed: 5.

### TargetPicker

`unity/Assets/_Game/Features/Towers/Scripts/TargetPicker.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Scripts/TargetPicker.cs)) · Static class · `HoldTheHill.Features.Towers` · HoldTheHill.Runtime · owner: Nilly

The targeting rules, as plain C# so EditMode tests can cover them: how far along the path an enemy is, and which of several candidates a TargetingPriority picks…

Named in: TargetPickerTests, Tower

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Pick` | Method (static) | `int` | `(IReadOnlyList<Candidate> candidates, TargetingPriority priority)` | Index of the candidate the priority prefers, or -1 when the list is empty. Ties go to the earlier candidate. | TargetPickerTests, Tower |
| `CanReach` | Method (static) | `bool` | `(bool enemyFlying, bool hasProjectile, bool lobbed)` | The wasp rule: a tower reaches a flying enemy only with a straight shot. Melee towers (no projectile) and lobbed shots, which land on the ground, skip anything … | TargetPickerTests, Tower |
| `PathProgress` | Method (static) | `float` | `(IReadOnlyList<Vector3> path, int nextWaypointIndex, Vector3 position)` | How far along path something at position has walked, given that it is heading for waypoint nextWaypointIndex: the path length up to that waypoint minus the dist… | TargetPickerTests, Tower |

#### TargetPicker.Candidate (struct)

One enemy in range, reduced to the two numbers the priorities compare.

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `PathProgress` | `float` |  |  |  | Distance walked along the path, in world units. Higher is nearer the Queen. |
| `SqrDistance` | `float` |  |  |  | Squared straight-line distance from the tower. |

Private members not listed: 1.

### TargetingPriority

`unity/Assets/_Game/Features/Towers/Scripts/TargetingPriority.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Scripts/TargetingPriority.cs)) · Enum · `HoldTheHill.Features.Towers` · HoldTheHill.Runtime · owner: Nilly

Which enemy a tower attacks when several are in range. The player sets it per tower from the tower card (the GDD's standing orders).

Named in: TargetPicker, TargetPickerTests, Tower, TowerCardPanel

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `First` | Enum member (static) | `TargetingPriority` |  | Furthest along the path, so closest to the Queen. The default. | TargetPicker, TargetPickerTests, Tower |
| `Last` | Enum member (static) | `TargetingPriority` |  | Least far along the path, hitting new arrivals first. | TargetPicker, TargetPickerTests |
| `Closest` | Enum member (static) | `TargetingPriority` |  | Nearest to the tower in a straight line. | TargetPickerTests |

### Tower

`unity/Assets/_Game/Features/Towers/Scripts/Tower.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Scripts/Tower.cs)) · MonoBehaviour · `HoldTheHill.Features.Towers` · HoldTheHill.Runtime · owner: Nilly

A placed tower. Knows its grid cell, picks a target in range by its TargetingPriority, and attacks it on a cooldown: with a projectile when it has one, otherwis…

Derives from / implements: MonoBehaviour

Used on: Tower_Ant, Tower_BulletAnt, Tower_CarpenterAnt, Tower_CrazyAnt, Tower_FireAnt, Tower_SapAnt

Named in: AlphaCoreLoopBuilder, AlphaTowersBuilder, BirdShadow, DiatomaceousEarth, LevelPowerUps, MineLayer, PesticideHuman, SlowAura, TargetPickerTests, TowerAttackAnimation, TowerCardPanel, TowerPlacer, TowerRangeDataTests, TowerRegistry, TowerStunRing, WaspStinger

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_range` | `float` | `2.5f` | Range | Tower_Ant = 1.5; Tower_BulletAnt = 3; Tower_CarpenterAnt = 2.5; Tower_CrazyAnt = 1.5; Tower_FireAnt = 2.8; Tower_SapAnt = 2 | How far this tower can reach, in world units. |
| `_damage` | `float` | `10f` | Attack | Tower_Ant = 8; Tower_BulletAnt = 12; Tower_CarpenterAnt = 30; Tower_CrazyAnt = 6; Tower_FireAnt = 14; Tower_SapAnt = 0 | Damage dealt per hit or per projectile. Zero with no projectile means the tower does not attack. |
| `_fireInterval` | `float` | `1f` | Attack | Tower_Ant = 0.8; Tower_BulletAnt = 1.2; Tower_CarpenterAnt = 3; Tower_CrazyAnt = 0.35; Tower_FireAnt = 1.6; Tower_SapAnt = 1 | Seconds between attacks. |
| `_projectilePrefab` | `Projectile` |  | Attack | Tower_Ant = (none); Tower_BulletAnt = Projectile_Bullet › Projectile; Tower_CarpenterAnt = (none); Tower_CrazyAnt = (none); Tower_FireAnt = Projectile_Lob › Pro… | Shot fired at the target. Leave empty for a melee tower that hits directly. |
| `_muzzle` | `Transform` |  | Attack | Tower_Ant = (none); Tower_BulletAnt = (none); Tower_CarpenterAnt = (none); Tower_CrazyAnt = (none); Tower_FireAnt = (none); Tower_SapAnt = (none) | Where shots leave the tower. Defaults to the tower itself. |
| `_priority` | `TargetingPriority` | `TargetingPriority.First` | Targeting | Tower_Ant = 0; Tower_BulletAnt = 0; Tower_CarpenterAnt = 0; Tower_CrazyAnt = 0; Tower_FireAnt = 0; Tower_SapAnt = 0 | Which enemy to attack when several are in range. |
| `_upgrades` | `TowerUpgrade[]` | `new TowerUpgrade[0]` | Upgrades | Tower_Ant = 2 item(s); Tower_BulletAnt = 2 item(s); Tower_CarpenterAnt = 2 item(s); Tower_CrazyAnt = 2 item(s); Tower_FireAnt = 2 item(s); Tower_SapAnt = 2 item… | Levels 2 and 3, in order: what each costs and the numbers the tower has afterwards. |
| `_scalePerLevel` | `float` | `1.12f` | Upgrades | Tower_Ant = 1.12; Tower_BulletAnt = 1.12; Tower_CarpenterAnt = 1.12; Tower_CrazyAnt = 1.12; Tower_FireAnt = 1.12; Tower_SapAnt = 1.12 | How much bigger the tower is drawn with each level, so an upgrade is visible. |
| `_burrowedTint` | `Color` | `new Color(0.5f, 0.45f, 0.4f, 0.6f)` | Condition | Tower_Ant = (0.5, 0.45, 0.4, 0.6); Tower_BulletAnt = (0.5, 0.45, 0.4, 0.6); Tower_CarpenterAnt = (0.5, 0.45, 0.4, 0.6); Tower_CrazyAnt = (0.5, 0.45, 0.4, 0.6); … | How the sprite is tinted while burrowed: sunk into the ground, out of the fight. |
| `_stunnedTint` | `Color` | `new Color(0.55f, 0.7f, 1f, 1f)` | Condition | Tower_Ant = (0.55, 0.7, 1, 1); Tower_BulletAnt = (0.55, 0.7, 1, 1); Tower_CarpenterAnt = (0.55, 0.7, 1, 1); Tower_CrazyAnt = (0.55, 0.7, 1, 1); Tower_FireAnt = … | How the sprite is tinted while stunned by a hazard. |
| `_missingTint` | `Color` | `new Color(1f, 0.85f, 0.45f, 1f)` | Condition | Tower_Ant = (1, 0.85, 0.45, 1); Tower_BulletAnt = (1, 0.85, 0.45, 1); Tower_CarpenterAnt = (1, 0.85, 0.45, 1); Tower_CrazyAnt = (1, 0.85, 0.45, 1); Tower_FireAn… | How the sprite is tinted while a hazard makes the tower miss. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Upgraded` | Event | `Action<int>` |  | Raised after an upgrade, with the new level. The card and visuals listen. | MineLayer, TowerCardPanel |
| `BurrowChanged` | Event | `Action<bool>` |  | Raised when the tower is burrowed or brought back up, with the new state. | TowerCardPanel |
| `Removed` | Event | `Action<Tower>` |  | Raised just before a hazard removes the tower, so the placer can free its cell. | TowerPlacer |
| `Attacked` | Event | `Action` |  | Raised the moment the tower attacks: a melee swing lands or a shot leaves. The attack animation lines its hit frame up with it. | TowerAttackAnimation |
| `AbilityUsed` | Event | `Action` |  | Raised when a tower that works through an ability uses it: the Carpenter Ant lays a mine, the Sap Ant's aura slows someone. The attack animation plays once for … | TowerAttackAnimation |
| `Range` | Property | `float` | `=> _range` | How far this tower can reach, in world units. | AlphaCoreLoopBuilder, MineLayer, PesticideHuman, SlowAura, TowerPlacer |
| `Damage` | Property | `float` | `=> _damage` | Damage dealt per hit. | AlphaCoreLoopBuilder, MineLayer |
| `FireInterval` | Property | `float` | `=> _fireInterval` | Seconds between attacks. | AlphaCoreLoopBuilder, MineLayer, TowerAttackAnimation |
| `CooldownRemaining` | Property | `float` | `=> Mathf.Max(0f, _cooldown)` | Seconds until the tower may attack again; zero when it is ready. | TowerAttackAnimation |
| `Cell` | Property | `Vector2Int` | `{ get; set; }` | Which grid cell this tower occupies. | TowerPlacer |
| `ShopData` | Property | `TowerData` | `{ get; set; }` | The shop entry this tower was bought from, for its name on the card. Set by the placer. | TowerCardPanel, TowerPlacer |
| `HoneydewSpent` | Property | `int` | `{ get; set; }` | Every Honeydew paid for this tower so far: the build price plus each upgrade. The placer keeps it. | TowerPlacer |
| `Level` | Property | `int` | `{ get; private set; }` | Current level, from 1. | LevelPowerUps, TowerCardPanel, TowerPlacer |
| `MaxLevel` | Property | `int` | `=> 1 + _upgrades.Length` | The last level this tower can reach. | LevelPowerUps, TowerCardPanel, TowerPlacer |
| `NextUpgradeCost` | Property | `int` | `=> Level < MaxLevel ? _upgrades[Level - 1].Cost : 0` | Honeydew for the next level, or 0 at the last level. | TowerCardPanel, TowerPlacer |
| `Priority` | Property | `TargetingPriority` | `{ get; set; }` | Which enemy this tower prefers to attack. Set from the tower card. | TowerCardPanel |
| `CurrentTarget` | Property | `BasicEnemy` | `{ get; private set; }` | The enemy being attacked this frame, or null. | TowerAttackAnimation |
| `Condition` | Property | `TowerCondition` | `=> _condition` | What is being done to this tower and what it may still do. | TowerStunRing |
| `Burrowed` | Property | `bool` | `{ get; set; }` | The burrow order. A burrowed tower does not act and is safe from the bird and the pesticide. Set from the tower card or the B key. | BirdShadow, PesticideHuman, TowerCardPanel, TowerPlacer |
| `CanAct` | Property | `bool` | `=> _condition.CanAct` | True when the tower may aim, fire, slow or lay mines. Companions check this. | MineLayer, SlowAura, TowerAttackAnimation |
| `IsStunned` | Property | `bool` | `=> _condition.IsStunned` |  |  |
| `IsMissing` | Property | `bool` | `=> _condition.IsMissing` |  | MineLayer, SlowAura |
| `ReachesFlying` | Property | `bool` | `=> TargetPicker.CanReach( enemyFlying: true, hasProjectile: _projectilePrefab != null, lobbed: _project` | False for a melee or lobbed tower: it cannot reach a flying enemy (the wasp rule). Follows from how the tower attacks, so no prefab needs a flag. |  |
| `UsesTargeting` | Property | `bool` | `=> Attacks` | True when the tower picks its own targets, so its standing order (First / Last / Closest) matters. False for the Sap Ant's aura and the Carpenter Ant's mines. T… | TowerAttackAnimation, TowerCardPanel |
| `AttacksOnItsOwn` | Method (static) | `bool` | `(bool hasAbility, float damage, bool hasProjectile)` | The attack rule: a tower with an ITowerAbility (a mine layer or a slow aura) does the work through it and only lends it its numbers; one with no damage and no p… | TargetPickerTests |
| `Stun` | Method | `void` | `(float seconds)` | A hazard stuns the tower for seconds: it does nothing meanwhile. | DiatomaceousEarth, PesticideHuman |
| `Miss` | Method | `void` | `(float seconds)` | A hazard makes the tower miss for seconds: it fires, to no effect. | WaspStinger |
| `Remove` | Method | `void` | `()` | A hazard takes the tower off the map. Tells the placer first, then destroys it. | BirdShadow, LevelPowerUps, MineLayer, TowerPlacer, TowerRegistry |
| `Upgrade` | Method | `bool` | `()` | Moves to the next level and takes its numbers. False at the last level. The caller (the placer) has already checked the price and paid. | LevelPowerUps, TowerCardPanel, TowerPlacer |
| `AcquireTarget` | Method | `BasicEnemy` | `()` | Picks the best living enemy in range for this tower's priority, or null. |  |
| `ReportAbilityUsed` | Method | `void` | `()` | The tower's ability (mine layer, slow aura) tells the tower it acted, for the animation. | MineLayer, SlowAura |

Private members not listed: 16.

### TowerAttackAnimation

`unity/Assets/_Game/Features/Towers/Scripts/TowerAttackAnimation.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Scripts/TowerAttackAnimation.cs)) · MonoBehaviour · `HoldTheHill.Features.Towers` · HoldTheHill.Runtime · owner: Nilly

Plays the tower's attack row from its sprite sheet over the idle loop. A tower that attacks on its own winds up just before it is ready, so the hit frame shows …

Derives from / implements: MonoBehaviour

Used on: Tower_Ant, Tower_BulletAnt, Tower_CarpenterAnt, Tower_CrazyAnt, Tower_FireAnt, Tower_SapAnt

Named in: AlphaSpritesBuilder, TowerAttackDataTests

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_renderer` | `SpriteRenderer` |  |  | Tower_Ant = Tower_Ant (SpriteRenderer); Tower_BulletAnt = Tower_BulletAnt (SpriteRenderer); Tower_CarpenterAnt = Tower_CarpenterAnt (SpriteRenderer); Tower_Craz… | The renderer the tower is drawn with. |
| `_idle` | `SpriteFlipbook` |  |  | Tower_Ant = Tower_Ant (SpriteFlipbook); Tower_BulletAnt = Tower_BulletAnt (SpriteFlipbook); Tower_CarpenterAnt = Tower_CarpenterAnt (SpriteFlipbook); Tower_Craz… | The idle loop, paused while the attack plays. |
| `_frames` | `Sprite[]` | `System.Array.Empty<Sprite>()` |  | Tower_Ant = 6 item(s); Tower_BulletAnt = 6 item(s); Tower_CarpenterAnt = 6 item(s); Tower_CrazyAnt = 6 item(s); Tower_FireAnt = 6 item(s); Tower_SapAnt = 6 item… | The attack row, in order. |
| `_frameSeconds` | `float` | `0.09f` |  | Tower_Ant = 0.06; Tower_BulletAnt = 0.07; Tower_CarpenterAnt = 0.09; Tower_CrazyAnt = 0.075; Tower_FireAnt = 0.08; Tower_SapAnt = 0.06 | The sheet's seconds per attack frame. Shortened when the tower fires faster than the row plays. |
| `_hitFrame` | `int` | `2` |  | Tower_Ant = 2; Tower_BulletAnt = 2; Tower_CarpenterAnt = 2; Tower_CrazyAnt = 2; Tower_FireAnt = 2; Tower_SapAnt = 2 | The frame where the blow lands (0 is the first). Frames before it are the wind-up. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Playing` | Property | `bool` | `=> _playing` | True while the attack row is showing. |  |

Private members not listed: 15.

### TowerAttackTiming

`unity/Assets/_Game/Features/Towers/Scripts/TowerAttackTiming.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Scripts/TowerAttackTiming.cs)) · Static class · `HoldTheHill.Features.Towers` · HoldTheHill.Runtime · owner: Nilly

The timing of a tower's attack animation, as pure functions so EditMode tests cover it. The art's attack row is wind-up frames, then the hit frame, then follow-…

Named in: TowerAttackAnimation, TowerAttackTimingTests

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `MinFrameSeconds` | Const | `float` |  | Never shorter than this per frame, so a very fast tower still shows each frame. | TowerAttackTimingTests |
| `FrameSeconds` | Method (static) | `float` | `(float sheetFrameSeconds, int frameCount, float fireInterval, float fireRateFactor)` | Seconds per attack frame: the sheet's time, or less, so the whole row fits in the time between two attacks. | TowerAttackAnimation, TowerAttackTimingTests |
| `WindUpSeconds` | Method (static) | `float` | `(int hitFrame, float frameSeconds)` | Seconds of wind-up before the hit frame. | TowerAttackAnimation, TowerAttackTimingTests |
| `ShouldWindUp` | Method (static) | `bool` | `(bool attacksOnItsOwn, bool canAct, bool hasTarget, float cooldownRemaining, float windUpSeconds)` | True when the tower should start winding up now: it attacks on its own, can act, has a target, and will be ready to attack within the wind-up. | TowerAttackAnimation, TowerAttackTimingTests |
| `FrameAt` | Method (static) | `int` | `(float clock, float frameSeconds, int frameCount)` | The frame to show clock seconds into the attack, or -1 once it is over. | TowerAttackAnimation, TowerAttackTimingTests |

### TowerClickRule

`unity/Assets/_Game/Features/Towers/Scripts/TowerClickRule.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Scripts/TowerClickRule.cs)) · Static class · `HoldTheHill.Features.Towers` · HoldTheHill.Runtime · owner: Nilly

What a left-click on the map does. Plain C# so EditMode tests can cover it; TowerPlacer applies the answer. A standing tower under the cursor is always selected…

Named in: DevKeys, LevelRun, PauseMenu, ResultPanel, TowerClickRuleTests, TowerPlacer

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Decide` | Method (static) | `Outcome` | `(bool armed, bool towerOnCell, bool blocked, bool affordable)` |  | TowerClickRuleTests, TowerPlacer |

#### TowerClickRule.Outcome (enum)

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Nothing` | Enum member (static) | `Outcome` |  | Nothing happens: a tower in hand over the path, or one the player can't afford. | TowerClickRuleTests |
| `SelectStanding` | Enum member (static) | `Outcome` |  | The tower under the cursor becomes the selected one; any tower in hand is put away. | TowerClickRuleTests, TowerPlacer |
| `ClearSelection` | Enum member (static) | `Outcome` |  | An empty cell with nothing in hand: the selection is cleared. | TowerClickRuleTests, TowerPlacer |
| `Build` | Enum member (static) | `Outcome` |  | The tower in hand is built on this cell. | TowerClickRuleTests, TowerPlacer |

### TowerCondition

`unity/Assets/_Game/Features/Towers/Scripts/TowerCondition.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Scripts/TowerCondition.cs)) · Class · `HoldTheHill.Features.Towers` · HoldTheHill.Runtime · owner: Nilly

What is being done to a tower, and what that allows: burrowed (the player's order), stunned (a hazard, for a while) or missing (a hazard, for a while). Plain C#…

Named in: Tower, TowerConditionTests, TowerPlacer

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Burrowed` | Property | `bool` | `{ get; set; }` | The player's order: a burrowed tower neither fires nor can be hit by a bird or pesticide. | Tower, TowerConditionTests, TowerPlacer |
| `StunnedFor` | Property | `float` | `{ get; private set; }` | Seconds of stun left. A stunned tower does nothing. | TowerConditionTests |
| `StunLength` | Property | `float` | `{ get; private set; }` | How long the current stun was when it began, so its time left can be drawn as a share. |  |
| `MissingFor` | Property | `float` | `{ get; private set; }` | Seconds of missing left. A missing tower fires, but its shots and hits do nothing. | TowerConditionTests |
| `IsStunned` | Property | `bool` | `=> StunnedFor > 0f` |  | Tower, TowerConditionTests |
| `IsMissing` | Property | `bool` | `=> MissingFor > 0f` |  | Tower, TowerConditionTests |
| `CanAct` | Property | `bool` | `=> !Burrowed && !IsStunned` | True when the tower may aim, fire, slow or lay mines. | Tower, TowerConditionTests |
| `HitsLand` | Property | `bool` | `=> !IsMissing` | True when a hit or shot from the tower should deal its damage. | Tower, TowerConditionTests |
| `Stun` | Method | `void` | `(float seconds)` | Stuns for seconds. A shorter stun never cuts a longer one short. | Tower, TowerConditionTests |
| `StunLeftShare` | Property | `float` | `=> StunLength > 0f ? StunnedFor / StunLength : 0f` | The share of the current stun still to run, from 1 when it lands to 0 when it is over. | TowerConditionTests |
| `Miss` | Method | `void` | `(float seconds)` | Makes the tower miss for seconds. Never shortens a longer spell. | Tower, TowerConditionTests |
| `Tick` | Method | `bool` | `(float deltaTime)` | Lets deltaTime pass. True when a stun or a missing spell ended this tick, so the owner can refresh what it shows. | Tower, TowerConditionTests |
| `BurrowAllNext` | Method (static) | `bool` | `(int burrowedCount, int towerCount)` | The B key: if any tower is still up, burrow them all; only when every tower is already burrowed does the key bring them all back up. | TowerConditionTests, TowerPlacer |

### TowerData

`unity/Assets/_Game/Features/Towers/Scripts/TowerData.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Scripts/TowerData.cs)) · ScriptableObject · `HoldTheHill.Features.Towers` · HoldTheHill.Runtime · owner: Adam

What the shop shows for one tower: its name, price and icon. The placer's offers and the shop cards both point at these assets (in Features/Towers/Data).

Derives from / implements: ScriptableObject

Used on: Ant, BulletAnt, CarpenterAnt, CrazyAnt, FireAnt, SapAnt, Tower1, Tower2, Tower3, Tower4, Tower5, Tower6

Named in: AlphaCoreLoopBuilder, AlphaShopBindingBuilder, AlphaSpritesBuilder, Tower, TowerCardUI, TowerIconDataTests, TowerListDataTests, TowerPlacer, TowerShopManager

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `towerName` | `string` |  | Shop Display | Ant; BulletAnt; CarpenterAnt; CrazyAnt; FireAnt; SapAnt; Tower1; Tower2 (+4 more) |  |
| `honeydewCost` | `int` |  | Shop Display | Ant; BulletAnt; CarpenterAnt; CrazyAnt; FireAnt; SapAnt; Tower1; Tower2 (+4 more) |  |
| `towerIcon` | `Sprite` |  | Shop Display | Ant; BulletAnt; CarpenterAnt; CrazyAnt; FireAnt; SapAnt; Tower1; Tower2 (+4 more) |  |

### TowerPlacer

`unity/Assets/_Game/Features/Towers/Scripts/TowerPlacer.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Scripts/TowerPlacer.cs)) · MonoBehaviour · `HoldTheHill.Features.Towers` · HoldTheHill.Runtime · owner: Nilly

Places and selects towers. A tower is picked from the shop (Select) or with the number keys; a ghost then follows the mouse, snapped to the grid. Left-click bui…

Derives from / implements: MonoBehaviour

Used on: Hill01_Gameplay, L00_TutorialHill_Gameplay, L01_Meadow_Gameplay

Named in: AlphaCoreLoopBuilder, AlphaPowerUpsBuilder, AlphaRunFrameBuilder, AlphaTowersBuilder, LevelPowerUps, LevelRun, ShopPlacerBinding, TowerCardPanel, TowerCardUI, TowerClickRuleTests

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_cellSize` | `float` | `1f` | Grid | Hill01_Gameplay = 1; L00_TutorialHill_Gameplay = 1; L01_Meadow_Gameplay = 1 | Size of one grid cell in world units. |
| `_buildArea` | `RectInt` | `MapCells` | Grid |  | The cells towers can be built on, in grid cells. A click outside them builds nothing. |
| `_path` | `EnemyPath` |  | Level | Hill01_Gameplay = Enemy Path (EnemyPath); L00_TutorialHill_Gameplay = Enemy Path (EnemyPath); L01_Meadow_Gameplay = Enemy Path (EnemyPath) | Towers can't be built on this path. Found automatically if left empty. |
| `_wallet` | `HoneydewWallet` |  | Level | Hill01_Gameplay = Honeydew Wallet (HoneydewWallet); L00_TutorialHill_Gameplay = Honeydew Wallet (HoneydewWallet); L01_Meadow_Gameplay = Honeydew Wallet (Honeyde… | The wallet towers are paid from. |
| `_offers` | `List<Offer>` | `new List<Offer>()` | Towers | Hill01_Gameplay = 6 item(s); L00_TutorialHill_Gameplay = 6 item(s); L01_Meadow_Gameplay = 6 item(s) | Towers for sale, in number-key order (1 is the first). |
| `_sellRefund` | `float` | `0.75f` | Selling | Hill01_Gameplay = 0.75; L00_TutorialHill_Gameplay = 0.75; L01_Meadow_Gameplay = 0.75 | Share of everything spent on a tower that comes back when it is sold. |
| `_actions` | `InputActionAsset` |  | Input | Hill01_Gameplay = HoldTheHill; L00_TutorialHill_Gameplay = HoldTheHill; L01_Meadow_Gameplay = HoldTheHill | The game's actions asset. Uses its Gameplay map: Point, Place, Cancel, SelectTower1-6. |
| `_freeCellColor` | `Color` | `new Color(1f, 1f, 1f, 0.35f)` | Preview | Hill01_Gameplay = (1, 1, 1, 0.35); L00_TutorialHill_Gameplay = (1, 1, 1, 0.35); L01_Meadow_Gameplay = (1, 1, 1, 0.35) |  |
| `_blockedCellColor` | `Color` | `new Color(1f, 0.3f, 0.3f, 0.35f)` | Preview | Hill01_Gameplay = (1, 0.3, 0.3, 0.35); L00_TutorialHill_Gameplay = (1, 0.3, 0.3, 0.35); L01_Meadow_Gameplay = (1, 0.3, 0.3, 0.35) |  |
| `_unaffordableColor` | `Color` | `new Color(1f, 0.8f, 0.2f, 0.35f)` | Preview | Hill01_Gameplay = (1, 0.8, 0.2, 0.35); L00_TutorialHill_Gameplay = (1, 0.8, 0.2, 0.35); L01_Meadow_Gameplay = (1, 0.8, 0.2, 0.35) |  |
| `_selectionColor` | `Color` | `new Color(0.4f, 0.9f, 1f, 0.45f)` | Preview | Hill01_Gameplay = (0.4, 0.9, 1, 0.45); L00_TutorialHill_Gameplay = (0.4, 0.9, 1, 0.45); L01_Meadow_Gameplay = (0.4, 0.9, 1, 0.45) |  |
| `_rangeColor` | `Color` | `new Color(1f, 1f, 1f, 0.55f)` | Preview |  | The ring that shows how far the ghost or the selected tower reaches. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `MapCells` | Field (static readonly) | `RectInt` |  | The map's cells, x -10..9 and y -5..4: the area LevelCameraFit frames. Nothing is built outside it. | TowerClickRuleTests |
| `SelectKeys` | Const (private) | `int` |  |  |  |
| `TowerPlaced` | Event | `Action<Tower>` |  | Raised when a tower has been built and paid for. |  |
| `SelectedTowerChanged` | Event | `Action<Tower>` |  | Raised when the selected tower changes; null means nothing is selected. | TowerCardPanel |
| `Selected` | Property | `Offer?` | `=> _selected >= 0 ? _offers[_selected] : (Offer?)null` | The offer the ghost shows, or null when nothing is selected. | LevelPowerUps, TowerCardUI |
| `SelectedTower` | Property | `Tower` | `=> _selectedTower` | The standing tower the card shows, or null. | LevelRun, TowerCardPanel |
| `Wallet` | Property | `HoneydewWallet` | `=> _wallet` | The wallet towers and upgrades are paid from. | TowerCardPanel |
| `Towers` | Property | `IReadOnlyDictionary<Vector2Int, Tower>` | `=> _towers` | Towers currently standing, by grid cell. | AlphaCoreLoopBuilder, AlphaPowerUpsBuilder, AlphaRunFrameBuilder, AlphaTowersBuilder, LevelPowerUps, LevelRun, ShopPlacerBinding, TowerCardPanel, TowerCardUI, T… |
| `AcceptsInput` | Property | `bool` | `{ get; set; }` | False while the run is paused or over: keys and clicks then leave towers alone. The level's run sets it. | LevelRun, TowerCardUI |
| `Select` | Method | `void` | `(TowerData data)` | Starts placing the tower sold by data. The shop card calls this. Does nothing while the run is paused or over, like the number keys. | LevelPowerUps, TowerCardUI |
| `Select` | Method | `void` | `(int index)` | Starts placing offer index; an index out of range cancels. | LevelPowerUps, TowerCardUI |
| `Cancel` | Method | `void` | `()` | Puts the ghost away without building anything, and drops the tower selection. |  |
| `SelectTower` | Method | `void` | `(Tower tower)` | Makes tower the one the card shows; null clears it. |  |
| `UpgradeSelectedTower` | Method | `TowerUpgradeRule.Verdict` | `()` | Buys the selected tower's next level if it has one and the wallet covers it. The card and the U key both come here. | LevelRun, TowerCardPanel |
| `SellValue` | Method | `int` | `(Tower tower)` | Honeydew the player would get back for selling tower now. | TowerCardPanel |
| `SellSelectedTower` | Method | `bool` | `()` | Sells the selected tower: refunds its SellValue and takes it off the map. Removing it frees the cell and clears the selection through its Removed event. The car… | LevelRun, TowerCardPanel |
| `ToggleBurrowAll` | Method | `void` | `()` | Burrows every tower, or brings every tower up when all of them are already down. The B key comes here. | LevelRun |
| `TowerAt` | Method | `Tower` | `(Vector3 world)` | The standing tower on the cell under world, or null. | LevelPowerUps |

#### TowerPlacer.Offer (struct)

A tower the player can buy: the shop entry and the prefab it builds.

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `Data` | `TowerData` |  |  |  | The shop card's data; its Honeydew cost is what the player pays. |
| `Prefab` | `Tower` |  |  |  | The tower prefab built for that card. |

Private members not listed: 23.

### TowerRegistry

`unity/Assets/_Game/Features/Towers/Scripts/TowerRegistry.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Scripts/TowerRegistry.cs)) · Static class · `HoldTheHill.Features.Towers` · HoldTheHill.Runtime · owner: Nilly

Every standing Tower, so hazards can find the towers under them without a scene search. The mirror of EnemyRegistry; towers add themselves.

Named in: BirdShadow, DiatomaceousEarth, PesticideHuman, Tower, WaspStinger

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `All` | Property (static) | `IReadOnlyList<Tower>` | `=> s_all` | Towers currently standing and enabled, in placement order. | BirdShadow, DiatomaceousEarth, PesticideHuman, WaspStinger |
| `Add` | Method (internal static) | `void` | `(Tower tower)` |  | Tower |
| `Remove` | Method (internal static) | `void` | `(Tower tower)` |  | Tower |

Private members not listed: 2.

### TowerSellRule

`unity/Assets/_Game/Features/Towers/Scripts/TowerSellRule.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Scripts/TowerSellRule.cs)) · Static class · `HoldTheHill.Features.Towers` · HoldTheHill.Runtime · owner: Nilly

What the player gets back for selling a tower: a fraction of every Honeydew spent on it (the build price and each upgrade), rounded down. Plain C# so EditMode t…

Named in: TowerPlacer, TowerSellRuleTests

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Refund` | Method (static) | `int` | `(int honeydewSpent, float fraction)` | Honeydew returned for a tower that cost honeydewSpent in all, at a refund fraction between 0 and 1. Never negative, never more than was spent. | TowerPlacer, TowerSellRuleTests |

### TowerStunRing

`unity/Assets/_Game/Features/Towers/Scripts/TowerStunRing.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Scripts/TowerStunRing.cs)) · MonoBehaviour · `HoldTheHill.Features.Towers` · HoldTheHill.Runtime · owner: Nilly

A ring around a stunned tower that empties like a clock as the stun wears off, so the player can see when the tower fights again. Hidden while the tower is not …

Derives from / implements: MonoBehaviour

Named in: Tower

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `FullCircleSegments` | Const (private) | `int` |  |  |  |
| `Radius` | Const (private) | `float` |  |  |  |
| `Width` | Const (private) | `float` |  |  |  |
| `Attach` | Method (static) | `TowerStunRing` | `(Tower tower, SpriteRenderer look, Color color)` | Makes a ring for tower, drawn with look's material just above it, in color. | Tower |

Private members not listed: 5.

### TowerTempo

`unity/Assets/_Game/Features/Towers/Scripts/TowerTempo.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Scripts/TowerTempo.cs)) · Static class · `HoldTheHill.Features.Towers` · HoldTheHill.Runtime · owner: Nilly

How fast every tower fires right now, as one factor: 1 is normal, 2 is twice as fast. Tower divides its cooldown by it in one place; the Swarm Frenzy power-up s…

Named in: LevelPowerUps, MineLayer, Tower, TowerAttackAnimation, TowerTempoTests

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `FireRateFactor` | Property (static) | `float` | `{ get; set; }` | Multiplier on every tower's fire rate. Never below 1. | LevelPowerUps, MineLayer, Tower, TowerAttackAnimation |
| `NextCooldown` | Method (static) | `float` | `(float cooldown, float interval, float deltaTime, float fireRateFactor)` | The cooldown after a shot. A shot fired part-way through a frame keeps the part of the frame it was late by, so a tower fires at its stated rate at any frame ra… | Tower, TowerTempoTests |

Private members not listed: 2.

### TowerUpgrade

`unity/Assets/_Game/Features/Towers/Scripts/TowerUpgrade.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Scripts/TowerUpgrade.cs)) · Struct · `HoldTheHill.Features.Towers` · HoldTheHill.Runtime · owner: Nilly

One upgrade step of a tower: what it costs and the numbers the tower has afterwards. A tower prefab lists two of these for levels 2 and 3. Data, not code (UNITY…

Named in: AlphaCoreLoopBuilder, Tower

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `Cost` | `int` |  |  |  | Honeydew paid to reach this level. |
| `Range` | `float` |  |  |  | Reach in world units at this level. |
| `Damage` | `float` |  |  |  | Damage per hit, projectile or mine at this level. |
| `FireInterval` | `float` |  |  |  | Seconds between attacks at this level. |

### TowerUpgradeRule

`unity/Assets/_Game/Features/Towers/Scripts/TowerUpgradeRule.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Scripts/TowerUpgradeRule.cs)) · Static class · `HoldTheHill.Features.Towers` · HoldTheHill.Runtime · owner: Nilly

Whether a tower may be upgraded right now. Plain C# so EditMode tests can cover it; UpgradeSelectedTower applies the answer.

Named in: PowerUpRule, TowerCardPanel, TowerPlacer, TowerUpgradeRuleTests

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Check` | Method (static) | `Verdict` | `(int level, int maxLevel, int cost, int honeydew)` | Decides for a tower at level of maxLevel whose next level costs cost, with honeydew in the wallet. The level check comes first, so a maxed tower never reports a… | PowerUpRule, TowerCardPanel, TowerPlacer, TowerUpgradeRuleTests |

#### TowerUpgradeRule.Verdict (enum)

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Ready` | Enum member (static) | `Verdict` |  | The upgrade can be bought. | PowerUpRule, TowerCardPanel, TowerPlacer, TowerUpgradeRuleTests |
| `AtMaxLevel` | Enum member (static) | `Verdict` |  | The tower is already at its last level. | TowerCardPanel, TowerPlacer, TowerUpgradeRuleTests |
| `CannotAfford` | Enum member (static) | `Verdict` |  | Not enough Honeydew for the next level. | TowerUpgradeRuleTests |

## Prefabs

### Mine_Carpenter

`unity/Assets/_Game/Features/Towers/Prefabs/Mine_Carpenter.prefab` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Prefabs/Mine_Carpenter.prefab)) · Prefab

Mine: A mine laid by the Carpenter Ant. Waits on the path; the first living enemy to come within its trigger radius sets it off, and everything within the blast…

Made by: AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01))

Used in: Tower_CarpenterAnt

**Mine_Carpenter** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (0.3, 0.3, 0.3) |  |

**Mine_Carpenter** · SpriteRenderer

| Field | Value | Points to |
|---|---|---|
| `m_Sprite` | Circle | Sprite: unity/Assets/_Game/Shared/Art/Placeholders/Circle.png |
| `m_Color` | (0.35, 0.25, 0.2, 1) |  |
| `m_SortingOrder` | 3 |  |
| `m_SortingLayer` | 0 |  |

**Mine_Carpenter** · Mine

| Field | Value | Points to |
|---|---|---|
| `_triggerRadius` | 0.35 |  |
| `_blastRadius` | 0.9 |  |

### Projectile_Bullet

`unity/Assets/_Game/Features/Towers/Prefabs/Projectile_Bullet.prefab` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Prefabs/Projectile_Bullet.prefab)) · Prefab

Projectile: One shot from a ranged tower. A straight shot flies at its target and deals the tower's damage on arrival; if the target dies or reaches the Queen f…

Made by: AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01))

Used in: Tower_BulletAnt

**Projectile_Bullet** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (0.2, 0.2, 0.2) |  |

**Projectile_Bullet** · SpriteRenderer

| Field | Value | Points to |
|---|---|---|
| `m_Sprite` | Circle | Sprite: unity/Assets/_Game/Shared/Art/Placeholders/Circle.png |
| `m_Color` | (1, 0.9, 0.3, 1) |  |
| `m_SortingOrder` | 20 |  |
| `m_SortingLayer` | 0 |  |

**Projectile_Bullet** · Projectile

| Field | Value | Points to |
|---|---|---|
| `_speed` | 10 |  |
| `_hitRadius` | 0.2 |  |
| `_lifetime` | 3 |  |

### Projectile_Lob

`unity/Assets/_Game/Features/Towers/Prefabs/Projectile_Lob.prefab` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Prefabs/Projectile_Lob.prefab)) · Prefab

Projectile: One shot from a ranged tower. A straight shot flies at its target and deals the tower's damage on arrival; if the target dies or reaches the Queen f…

Made by: AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01))

Used in: Tower_FireAnt

**Projectile_Lob** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (0.3, 0.3, 0.3) |  |

**Projectile_Lob** · SpriteRenderer

| Field | Value | Points to |
|---|---|---|
| `m_Sprite` | Circle | Sprite: unity/Assets/_Game/Shared/Art/Placeholders/Circle.png |
| `m_Color` | (1, 0.45, 0.1, 1) |  |
| `m_SortingOrder` | 20 |  |
| `m_SortingLayer` | 0 |  |

**Projectile_Lob** · Projectile

| Field | Value | Points to |
|---|---|---|
| `_speed` | 6 |  |
| `_hitRadius` | 0.2 |  |
| `_lifetime` | 3 |  |
| `_lobbed` | 1 |  |
| `_arcHeight` | 1.5 |  |
| `_splashRadius` | 1 |  |

### Tower_Ant

`unity/Assets/_Game/Features/Towers/Prefabs/Tower_Ant.prefab` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Prefabs/Tower_Ant.prefab)) · Prefab

Tower: A placed tower. Knows its grid cell, picks a target in range by its TargetingPriority, and attacks it on a cooldown: with a projectile when it has one, o…

Made by: AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01)); AlphaSpritesBuilder (Tools/Hold the Hill/Alpha/Build Sprites (enemy and tower sheets,…

Used in: Hill01_Gameplay, L00_TutorialHill_Gameplay, L01_Meadow_Gameplay

**Tower_Ant** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (0.8, 0.8, 0.8) |  |

**Tower_Ant** · SpriteRenderer

| Field | Value | Points to |
|---|---|---|
| `m_Sprite` | TowerSoldier.png › TowerSoldier Idle 0 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerSoldier.png |
| `m_Color` | (1, 1, 1, 1) |  |
| `m_SortingOrder` | 10 |  |
| `m_SortingLayer` | 0 |  |

**Tower_Ant** · Tower

| Field | Value | Points to |
|---|---|---|
| `_range` | 1.5 |  |
| `_damage` | 8 |  |
| `_fireInterval` | 0.8 |  |
| `_projectilePrefab` | (none) |  |
| `_muzzle` | (none) |  |
| `_priority` | 0 |  |
| `_upgrades` | 2 item(s) |  |
| `_upgrades[0].Cost` | 45 |  |
| `_upgrades[0].Range` | 1.6 |  |
| `_upgrades[0].Damage` | 12 |  |
| `_upgrades[0].FireInterval` | 0.7 |  |
| `_upgrades[1].Cost` | 70 |  |
| `_upgrades[1].Range` | 1.7 |  |
| `_upgrades[1].Damage` | 18 |  |
| `_upgrades[1].FireInterval` | 0.6 |  |
| `_scalePerLevel` | 1.12 |  |
| `_burrowedTint` | (0.5, 0.45, 0.4, 0.6) |  |
| `_stunnedTint` | (0.55, 0.7, 1, 1) |  |
| `_missingTint` | (1, 0.85, 0.45, 1) |  |

**Tower_Ant** · SpriteFlipbook

| Field | Value | Points to |
|---|---|---|
| `_renderer` | Tower_Ant (SpriteRenderer) | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_Ant.prefab |
| `_frames` | 4 item(s) |  |
| `_frames[0]` | TowerSoldier.png › TowerSoldier Idle 0 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerSoldier.png |
| `_frames[1]` | TowerSoldier.png › TowerSoldier Idle 1 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerSoldier.png |
| `_frames[2]` | TowerSoldier.png › TowerSoldier Idle 2 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerSoldier.png |
| `_frames[3]` | TowerSoldier.png › TowerSoldier Idle 3 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerSoldier.png |
| `_frameSeconds` | 0.16 |  |
| `_randomStart` | 1 |  |

**Tower_Ant** · TowerAttackAnimation

| Field | Value | Points to |
|---|---|---|
| `_renderer` | Tower_Ant (SpriteRenderer) | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_Ant.prefab |
| `_idle` | Tower_Ant (SpriteFlipbook) | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_Ant.prefab |
| `_frames` | 6 item(s) |  |
| `_frames[0]` | TowerSoldier.png › TowerSoldier Attack 0 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerSoldier.png |
| `_frames[1]` | TowerSoldier.png › TowerSoldier Attack 1 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerSoldier.png |
| `_frames[2]` | TowerSoldier.png › TowerSoldier Attack 2 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerSoldier.png |
| `_frames[3]` | TowerSoldier.png › TowerSoldier Attack 3 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerSoldier.png |
| `_frames[4]` | TowerSoldier.png › TowerSoldier Attack 4 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerSoldier.png |
| `_frames[5]` | TowerSoldier.png › TowerSoldier Attack 5 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerSoldier.png |
| `_frameSeconds` | 0.06 |  |
| `_hitFrame` | 2 |  |

### Tower_BulletAnt

`unity/Assets/_Game/Features/Towers/Prefabs/Tower_BulletAnt.prefab` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Prefabs/Tower_BulletAnt.prefab)) · Prefab

Tower: A placed tower. Knows its grid cell, picks a target in range by its TargetingPriority, and attacks it on a cooldown: with a projectile when it has one, o…

Made by: AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01)); AlphaSpritesBuilder (Tools/Hold the Hill/Alpha/Build Sprites (enemy and tower sheets,…

Used in: Hill01_Gameplay, L00_TutorialHill_Gameplay, L01_Meadow_Gameplay

**Tower_BulletAnt** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (0.8, 0.8, 0.8) |  |

**Tower_BulletAnt** · SpriteRenderer

| Field | Value | Points to |
|---|---|---|
| `m_Sprite` | TowerLinear.png › TowerLinear Idle 0 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerLinear.png |
| `m_Color` | (1, 1, 1, 1) |  |
| `m_SortingOrder` | 10 |  |
| `m_SortingLayer` | 0 |  |

**Tower_BulletAnt** · Tower

| Field | Value | Points to |
|---|---|---|
| `_range` | 3 |  |
| `_damage` | 12 |  |
| `_fireInterval` | 1.2 |  |
| `_projectilePrefab` | Projectile_Bullet › Projectile | Component: unity/Assets/_Game/Features/Towers/Prefabs/Projectile_Bullet.prefab |
| `_muzzle` | (none) |  |
| `_priority` | 0 |  |
| `_upgrades` | 2 item(s) |  |
| `_upgrades[0].Cost` | 60 |  |
| `_upgrades[0].Range` | 3.3 |  |
| `_upgrades[0].Damage` | 18 |  |
| `_upgrades[0].FireInterval` | 1.1 |  |
| `_upgrades[1].Cost` | 90 |  |
| `_upgrades[1].Range` | 3.6 |  |
| `_upgrades[1].Damage` | 26 |  |
| `_upgrades[1].FireInterval` | 1 |  |
| `_scalePerLevel` | 1.12 |  |
| `_burrowedTint` | (0.5, 0.45, 0.4, 0.6) |  |
| `_stunnedTint` | (0.55, 0.7, 1, 1) |  |
| `_missingTint` | (1, 0.85, 0.45, 1) |  |

**Tower_BulletAnt** · SpriteFlipbook

| Field | Value | Points to |
|---|---|---|
| `_renderer` | Tower_BulletAnt (SpriteRenderer) | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_BulletAnt.prefab |
| `_frames` | 4 item(s) |  |
| `_frames[0]` | TowerLinear.png › TowerLinear Idle 0 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerLinear.png |
| `_frames[1]` | TowerLinear.png › TowerLinear Idle 1 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerLinear.png |
| `_frames[2]` | TowerLinear.png › TowerLinear Idle 2 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerLinear.png |
| `_frames[3]` | TowerLinear.png › TowerLinear Idle 3 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerLinear.png |
| `_frameSeconds` | 0.16 |  |
| `_randomStart` | 1 |  |

**Tower_BulletAnt** · TowerAttackAnimation

| Field | Value | Points to |
|---|---|---|
| `_renderer` | Tower_BulletAnt (SpriteRenderer) | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_BulletAnt.prefab |
| `_idle` | Tower_BulletAnt (SpriteFlipbook) | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_BulletAnt.prefab |
| `_frames` | 6 item(s) |  |
| `_frames[0]` | TowerLinear.png › TowerLinear Attack 0 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerLinear.png |
| `_frames[1]` | TowerLinear.png › TowerLinear Attack 1 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerLinear.png |
| `_frames[2]` | TowerLinear.png › TowerLinear Attack 2 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerLinear.png |
| `_frames[3]` | TowerLinear.png › TowerLinear Attack 3 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerLinear.png |
| `_frames[4]` | TowerLinear.png › TowerLinear Attack 4 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerLinear.png |
| `_frames[5]` | TowerLinear.png › TowerLinear Attack 5 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerLinear.png |
| `_frameSeconds` | 0.07 |  |
| `_hitFrame` | 2 |  |

### Tower_CarpenterAnt

`unity/Assets/_Game/Features/Towers/Prefabs/Tower_CarpenterAnt.prefab` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Prefabs/Tower_CarpenterAnt.prefab)) · Prefab

Tower: A placed tower. Knows its grid cell, picks a target in range by its TargetingPriority, and attacks it on a cooldown: with a projectile when it has one, o…

Made by: AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01)); AlphaSpritesBuilder (Tools/Hold the Hill/Alpha/Build Sprites (enemy and tower sheets,…

Used in: Hill01_Gameplay, L00_TutorialHill_Gameplay, L01_Meadow_Gameplay

**Tower_CarpenterAnt** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (0.8, 0.8, 0.8) |  |

**Tower_CarpenterAnt** · SpriteRenderer

| Field | Value | Points to |
|---|---|---|
| `m_Sprite` | TowerMineLayer.png › TowerMineLayer Idle 0 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerMineLayer.png |
| `m_Color` | (1, 1, 1, 1) |  |
| `m_SortingOrder` | 10 |  |
| `m_SortingLayer` | 0 |  |

**Tower_CarpenterAnt** · Tower

| Field | Value | Points to |
|---|---|---|
| `_range` | 2.5 |  |
| `_damage` | 30 |  |
| `_fireInterval` | 3 |  |
| `_projectilePrefab` | (none) |  |
| `_muzzle` | (none) |  |
| `_priority` | 0 |  |
| `_upgrades` | 2 item(s) |  |
| `_upgrades[0].Cost` | 150 |  |
| `_upgrades[0].Range` | 2.8 |  |
| `_upgrades[0].Damage` | 45 |  |
| `_upgrades[0].FireInterval` | 2.5 |  |
| `_upgrades[1].Cost` | 225 |  |
| `_upgrades[1].Range` | 3.1 |  |
| `_upgrades[1].Damage` | 65 |  |
| `_upgrades[1].FireInterval` | 2 |  |
| `_scalePerLevel` | 1.12 |  |
| `_burrowedTint` | (0.5, 0.45, 0.4, 0.6) |  |
| `_stunnedTint` | (0.55, 0.7, 1, 1) |  |
| `_missingTint` | (1, 0.85, 0.45, 1) |  |

**Tower_CarpenterAnt** · MineLayer

| Field | Value | Points to |
|---|---|---|
| `_minePrefab` | Mine_Carpenter › Mine | Component: unity/Assets/_Game/Features/Towers/Prefabs/Mine_Carpenter.prefab |
| `_maxMines` | 3 |  |
| `_spotSpacing` | 0.5 |  |
| `_path` | (none) |  |

**Tower_CarpenterAnt** · SpriteFlipbook

| Field | Value | Points to |
|---|---|---|
| `_renderer` | Tower_CarpenterAnt (SpriteRenderer) | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_CarpenterAnt.prefab |
| `_frames` | 4 item(s) |  |
| `_frames[0]` | TowerMineLayer.png › TowerMineLayer Idle 0 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerMineLayer.png |
| `_frames[1]` | TowerMineLayer.png › TowerMineLayer Idle 1 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerMineLayer.png |
| `_frames[2]` | TowerMineLayer.png › TowerMineLayer Idle 2 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerMineLayer.png |
| `_frames[3]` | TowerMineLayer.png › TowerMineLayer Idle 3 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerMineLayer.png |
| `_frameSeconds` | 0.16 |  |
| `_randomStart` | 1 |  |

**Tower_CarpenterAnt** · TowerAttackAnimation

| Field | Value | Points to |
|---|---|---|
| `_renderer` | Tower_CarpenterAnt (SpriteRenderer) | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_CarpenterAnt.prefab |
| `_idle` | Tower_CarpenterAnt (SpriteFlipbook) | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_CarpenterAnt.prefab |
| `_frames` | 6 item(s) |  |
| `_frames[0]` | TowerMineLayer.png › TowerMineLayer Attack 0 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerMineLayer.png |
| `_frames[1]` | TowerMineLayer.png › TowerMineLayer Attack 1 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerMineLayer.png |
| `_frames[2]` | TowerMineLayer.png › TowerMineLayer Attack 2 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerMineLayer.png |
| `_frames[3]` | TowerMineLayer.png › TowerMineLayer Attack 3 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerMineLayer.png |
| `_frames[4]` | TowerMineLayer.png › TowerMineLayer Attack 4 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerMineLayer.png |
| `_frames[5]` | TowerMineLayer.png › TowerMineLayer Attack 5 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerMineLayer.png |
| `_frameSeconds` | 0.09 |  |
| `_hitFrame` | 2 |  |

### Tower_CrazyAnt

`unity/Assets/_Game/Features/Towers/Prefabs/Tower_CrazyAnt.prefab` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Prefabs/Tower_CrazyAnt.prefab)) · Prefab

Tower: A placed tower. Knows its grid cell, picks a target in range by its TargetingPriority, and attacks it on a cooldown: with a projectile when it has one, o…

Made by: AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01)); AlphaSpritesBuilder (Tools/Hold the Hill/Alpha/Build Sprites (enemy and tower sheets,…

Used in: Hill01_Gameplay, L00_TutorialHill_Gameplay, L01_Meadow_Gameplay

**Tower_CrazyAnt** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (0.8, 0.8, 0.8) |  |

**Tower_CrazyAnt** · SpriteRenderer

| Field | Value | Points to |
|---|---|---|
| `m_Sprite` | TowerKnockback.png › TowerKnockback Idle 0 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerKnockback.png |
| `m_Color` | (1, 1, 1, 1) |  |
| `m_SortingOrder` | 10 |  |
| `m_SortingLayer` | 0 |  |

**Tower_CrazyAnt** · Tower

| Field | Value | Points to |
|---|---|---|
| `_range` | 1.5 |  |
| `_damage` | 6 |  |
| `_fireInterval` | 0.35 |  |
| `_projectilePrefab` | (none) |  |
| `_muzzle` | (none) |  |
| `_priority` | 0 |  |
| `_upgrades` | 2 item(s) |  |
| `_upgrades[0].Cost` | 75 |  |
| `_upgrades[0].Range` | 1.6 |  |
| `_upgrades[0].Damage` | 9 |  |
| `_upgrades[0].FireInterval` | 0.3 |  |
| `_upgrades[1].Cost` | 110 |  |
| `_upgrades[1].Range` | 1.7 |  |
| `_upgrades[1].Damage` | 13 |  |
| `_upgrades[1].FireInterval` | 0.25 |  |
| `_scalePerLevel` | 1.12 |  |
| `_burrowedTint` | (0.5, 0.45, 0.4, 0.6) |  |
| `_stunnedTint` | (0.55, 0.7, 1, 1) |  |
| `_missingTint` | (1, 0.85, 0.45, 1) |  |

**Tower_CrazyAnt** · SpriteFlipbook

| Field | Value | Points to |
|---|---|---|
| `_renderer` | Tower_CrazyAnt (SpriteRenderer) | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_CrazyAnt.prefab |
| `_frames` | 4 item(s) |  |
| `_frames[0]` | TowerKnockback.png › TowerKnockback Idle 0 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerKnockback.png |
| `_frames[1]` | TowerKnockback.png › TowerKnockback Idle 1 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerKnockback.png |
| `_frames[2]` | TowerKnockback.png › TowerKnockback Idle 2 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerKnockback.png |
| `_frames[3]` | TowerKnockback.png › TowerKnockback Idle 3 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerKnockback.png |
| `_frameSeconds` | 0.16 |  |
| `_randomStart` | 1 |  |

**Tower_CrazyAnt** · TowerAttackAnimation

| Field | Value | Points to |
|---|---|---|
| `_renderer` | Tower_CrazyAnt (SpriteRenderer) | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_CrazyAnt.prefab |
| `_idle` | Tower_CrazyAnt (SpriteFlipbook) | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_CrazyAnt.prefab |
| `_frames` | 6 item(s) |  |
| `_frames[0]` | TowerKnockback.png › TowerKnockback Attack 0 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerKnockback.png |
| `_frames[1]` | TowerKnockback.png › TowerKnockback Attack 1 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerKnockback.png |
| `_frames[2]` | TowerKnockback.png › TowerKnockback Attack 2 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerKnockback.png |
| `_frames[3]` | TowerKnockback.png › TowerKnockback Attack 3 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerKnockback.png |
| `_frames[4]` | TowerKnockback.png › TowerKnockback Attack 4 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerKnockback.png |
| `_frames[5]` | TowerKnockback.png › TowerKnockback Attack 5 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerKnockback.png |
| `_frameSeconds` | 0.075 |  |
| `_hitFrame` | 2 |  |

### Tower_FireAnt

`unity/Assets/_Game/Features/Towers/Prefabs/Tower_FireAnt.prefab` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Prefabs/Tower_FireAnt.prefab)) · Prefab

Tower: A placed tower. Knows its grid cell, picks a target in range by its TargetingPriority, and attacks it on a cooldown: with a projectile when it has one, o…

Made by: AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01)); AlphaSpritesBuilder (Tools/Hold the Hill/Alpha/Build Sprites (enemy and tower sheets,…

Used in: Hill01_Gameplay, L00_TutorialHill_Gameplay, L01_Meadow_Gameplay

**Tower_FireAnt** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (0.8, 0.8, 0.8) |  |

**Tower_FireAnt** · SpriteRenderer

| Field | Value | Points to |
|---|---|---|
| `m_Sprite` | TowerMortar.png › TowerMortar Idle 0 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerMortar.png |
| `m_Color` | (1, 1, 1, 1) |  |
| `m_SortingOrder` | 10 |  |
| `m_SortingLayer` | 0 |  |

**Tower_FireAnt** · Tower

| Field | Value | Points to |
|---|---|---|
| `_range` | 2.8 |  |
| `_damage` | 14 |  |
| `_fireInterval` | 1.6 |  |
| `_projectilePrefab` | Projectile_Lob › Projectile | Component: unity/Assets/_Game/Features/Towers/Prefabs/Projectile_Lob.prefab |
| `_muzzle` | (none) |  |
| `_priority` | 0 |  |
| `_upgrades` | 2 item(s) |  |
| `_upgrades[0].Cost` | 120 |  |
| `_upgrades[0].Range` | 3 |  |
| `_upgrades[0].Damage` | 20 |  |
| `_upgrades[0].FireInterval` | 1.5 |  |
| `_upgrades[1].Cost` | 180 |  |
| `_upgrades[1].Range` | 3.3 |  |
| `_upgrades[1].Damage` | 30 |  |
| `_upgrades[1].FireInterval` | 1.4 |  |
| `_scalePerLevel` | 1.12 |  |
| `_burrowedTint` | (0.5, 0.45, 0.4, 0.6) |  |
| `_stunnedTint` | (0.55, 0.7, 1, 1) |  |
| `_missingTint` | (1, 0.85, 0.45, 1) |  |

**Tower_FireAnt** · SpriteFlipbook

| Field | Value | Points to |
|---|---|---|
| `_renderer` | Tower_FireAnt (SpriteRenderer) | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_FireAnt.prefab |
| `_frames` | 4 item(s) |  |
| `_frames[0]` | TowerMortar.png › TowerMortar Idle 0 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerMortar.png |
| `_frames[1]` | TowerMortar.png › TowerMortar Idle 1 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerMortar.png |
| `_frames[2]` | TowerMortar.png › TowerMortar Idle 2 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerMortar.png |
| `_frames[3]` | TowerMortar.png › TowerMortar Idle 3 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerMortar.png |
| `_frameSeconds` | 0.16 |  |
| `_randomStart` | 1 |  |

**Tower_FireAnt** · TowerAttackAnimation

| Field | Value | Points to |
|---|---|---|
| `_renderer` | Tower_FireAnt (SpriteRenderer) | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_FireAnt.prefab |
| `_idle` | Tower_FireAnt (SpriteFlipbook) | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_FireAnt.prefab |
| `_frames` | 6 item(s) |  |
| `_frames[0]` | TowerMortar.png › TowerMortar Attack 0 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerMortar.png |
| `_frames[1]` | TowerMortar.png › TowerMortar Attack 1 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerMortar.png |
| `_frames[2]` | TowerMortar.png › TowerMortar Attack 2 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerMortar.png |
| `_frames[3]` | TowerMortar.png › TowerMortar Attack 3 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerMortar.png |
| `_frames[4]` | TowerMortar.png › TowerMortar Attack 4 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerMortar.png |
| `_frames[5]` | TowerMortar.png › TowerMortar Attack 5 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerMortar.png |
| `_frameSeconds` | 0.08 |  |
| `_hitFrame` | 2 |  |

### Tower_SapAnt

`unity/Assets/_Game/Features/Towers/Prefabs/Tower_SapAnt.prefab` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Prefabs/Tower_SapAnt.prefab)) · Prefab

Tower: A placed tower. Knows its grid cell, picks a target in range by its TargetingPriority, and attacks it on a cooldown: with a projectile when it has one, o…

Made by: AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01)); AlphaSpritesBuilder (Tools/Hold the Hill/Alpha/Build Sprites (enemy and tower sheets,…

Used in: Hill01_Gameplay, L00_TutorialHill_Gameplay, L01_Meadow_Gameplay

**Tower_SapAnt** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (0.8, 0.8, 0.8) |  |

**Tower_SapAnt** · SpriteRenderer

| Field | Value | Points to |
|---|---|---|
| `m_Sprite` | TowerFrostAura.png › TowerFrostAura Idle 0 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerFrostAura.png |
| `m_Color` | (1, 1, 1, 1) |  |
| `m_SortingOrder` | 10 |  |
| `m_SortingLayer` | 0 |  |

**Tower_SapAnt** · Tower

| Field | Value | Points to |
|---|---|---|
| `_range` | 2 |  |
| `_damage` | 0 |  |
| `_fireInterval` | 1 |  |
| `_projectilePrefab` | (none) |  |
| `_muzzle` | (none) |  |
| `_priority` | 0 |  |
| `_upgrades` | 2 item(s) |  |
| `_upgrades[0].Cost` | 135 |  |
| `_upgrades[0].Range` | 2.4 |  |
| `_upgrades[0].Damage` | 0 |  |
| `_upgrades[0].FireInterval` | 1 |  |
| `_upgrades[1].Cost` | 200 |  |
| `_upgrades[1].Range` | 2.8 |  |
| `_upgrades[1].Damage` | 0 |  |
| `_upgrades[1].FireInterval` | 1 |  |
| `_scalePerLevel` | 1.12 |  |
| `_burrowedTint` | (0.5, 0.45, 0.4, 0.6) |  |
| `_stunnedTint` | (0.55, 0.7, 1, 1) |  |
| `_missingTint` | (1, 0.85, 0.45, 1) |  |

**Tower_SapAnt** · SlowAura

| Field | Value | Points to |
|---|---|---|
| `_slowMultiplier` | 0.5 |  |
| `_tickInterval` | 0.25 |  |

**Tower_SapAnt** · SpriteFlipbook

| Field | Value | Points to |
|---|---|---|
| `_renderer` | Tower_SapAnt (SpriteRenderer) | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_SapAnt.prefab |
| `_frames` | 4 item(s) |  |
| `_frames[0]` | TowerFrostAura.png › TowerFrostAura Idle 0 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerFrostAura.png |
| `_frames[1]` | TowerFrostAura.png › TowerFrostAura Idle 1 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerFrostAura.png |
| `_frames[2]` | TowerFrostAura.png › TowerFrostAura Idle 2 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerFrostAura.png |
| `_frames[3]` | TowerFrostAura.png › TowerFrostAura Idle 3 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerFrostAura.png |
| `_frameSeconds` | 0.16 |  |
| `_randomStart` | 1 |  |

**Tower_SapAnt** · TowerAttackAnimation

| Field | Value | Points to |
|---|---|---|
| `_renderer` | Tower_SapAnt (SpriteRenderer) | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_SapAnt.prefab |
| `_idle` | Tower_SapAnt (SpriteFlipbook) | Component: unity/Assets/_Game/Features/Towers/Prefabs/Tower_SapAnt.prefab |
| `_frames` | 6 item(s) |  |
| `_frames[0]` | TowerFrostAura.png › TowerFrostAura Attack 0 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerFrostAura.png |
| `_frames[1]` | TowerFrostAura.png › TowerFrostAura Attack 1 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerFrostAura.png |
| `_frames[2]` | TowerFrostAura.png › TowerFrostAura Attack 2 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerFrostAura.png |
| `_frames[3]` | TowerFrostAura.png › TowerFrostAura Attack 3 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerFrostAura.png |
| `_frames[4]` | TowerFrostAura.png › TowerFrostAura Attack 4 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerFrostAura.png |
| `_frames[5]` | TowerFrostAura.png › TowerFrostAura Attack 5 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerFrostAura.png |
| `_frameSeconds` | 0.06 |  |
| `_hitFrame` | 2 |  |

## Data assets

### Ant

`unity/Assets/_Game/Features/Towers/Data/Ant.asset` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Data/Ant.asset)) · Data asset

Type: `TowerData`: What the shop shows for one tower: its name, price and icon. The placer's offers and the shop cards both point at these assets (in Features/Towers/Data).
Made by: AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01))

Used by: Hill01_Gameplay, L00_TutorialHill_Gameplay, L01_Meadow_Gameplay, TowerList, UI

**TowerData**

| Field | Value | Points to |
|---|---|---|
| `towerName` | Ant |  |
| `honeydewCost` | 75 |  |
| `towerIcon` | TowerSoldier.png › TowerSoldier Idle 0 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerSoldier.png |

### BulletAnt

`unity/Assets/_Game/Features/Towers/Data/BulletAnt.asset` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Data/BulletAnt.asset)) · Data asset

Type: `TowerData`: What the shop shows for one tower: its name, price and icon. The placer's offers and the shop cards both point at these assets (in Features/Towers/Data).
Made by: AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01))

Used by: Hill01_Gameplay, L00_TutorialHill_Gameplay, L01_Meadow_Gameplay, TowerList, UI

**TowerData**

| Field | Value | Points to |
|---|---|---|
| `towerName` | Bullet Ant |  |
| `honeydewCost` | 100 |  |
| `towerIcon` | TowerLinear.png › TowerLinear Idle 0 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerLinear.png |

### CarpenterAnt

`unity/Assets/_Game/Features/Towers/Data/CarpenterAnt.asset` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Data/CarpenterAnt.asset)) · Data asset

Type: `TowerData`: What the shop shows for one tower: its name, price and icon. The placer's offers and the shop cards both point at these assets (in Features/Towers/Data).
Made by: AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01))

Used by: Hill01_Gameplay, L00_TutorialHill_Gameplay, L01_Meadow_Gameplay, TowerList, UI

**TowerData**

| Field | Value | Points to |
|---|---|---|
| `towerName` | Carpenter Ant |  |
| `honeydewCost` | 250 |  |
| `towerIcon` | TowerMineLayer.png › TowerMineLayer Idle 0 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerMineLayer.png |

### CrazyAnt

`unity/Assets/_Game/Features/Towers/Data/CrazyAnt.asset` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Data/CrazyAnt.asset)) · Data asset

Type: `TowerData`: What the shop shows for one tower: its name, price and icon. The placer's offers and the shop cards both point at these assets (in Features/Towers/Data).
Made by: AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01))

Used by: Hill01_Gameplay, L00_TutorialHill_Gameplay, L01_Meadow_Gameplay, TowerList, UI

**TowerData**

| Field | Value | Points to |
|---|---|---|
| `towerName` | Crazy Ant |  |
| `honeydewCost` | 125 |  |
| `towerIcon` | TowerKnockback.png › TowerKnockback Idle 0 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerKnockback.png |

### FireAnt

`unity/Assets/_Game/Features/Towers/Data/FireAnt.asset` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Data/FireAnt.asset)) · Data asset

Type: `TowerData`: What the shop shows for one tower: its name, price and icon. The placer's offers and the shop cards both point at these assets (in Features/Towers/Data).
Made by: AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01))

Used by: Hill01_Gameplay, L00_TutorialHill_Gameplay, L01_Meadow_Gameplay, TowerList, UI

**TowerData**

| Field | Value | Points to |
|---|---|---|
| `towerName` | Fire Ant |  |
| `honeydewCost` | 200 |  |
| `towerIcon` | TowerMortar.png › TowerMortar Idle 0 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerMortar.png |

### SapAnt

`unity/Assets/_Game/Features/Towers/Data/SapAnt.asset` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Towers/Data/SapAnt.asset)) · Data asset

Type: `TowerData`: What the shop shows for one tower: its name, price and icon. The placer's offers and the shop cards both point at these assets (in Features/Towers/Data).
Made by: AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01))

Used by: Hill01_Gameplay, L00_TutorialHill_Gameplay, L01_Meadow_Gameplay, TowerList, UI

**TowerData**

| Field | Value | Points to |
|---|---|---|
| `towerName` | Sap Ant |  |
| `honeydewCost` | 225 |  |
| `towerIcon` | TowerFrostAura.png › TowerFrostAura Idle 0 | Sprite: unity/Assets/_Game/Features/Towers/Art/Sheets/TowerFrostAura.png |
