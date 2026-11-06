# Enemies

Part of the [code bank](README.md), built 2026-11-05 from commit `d6944f7b`.

## Scripts

| Script | Kind | Owner | What it does |
|---|---|---|---|
| [EnemyDeathEffect](#enemydeatheffect) | MonoBehaviour | Nilly | What is left where an enemy died: a copy of its body playing the Death row once, then fading away. Made at run time by EnemyHurtAndDeath, in the enemy's own sce… |
| [EnemyHurtAndDeath](#enemyhurtanddeath) | MonoBehaviour | Nilly | Plays the enemy sheet's Hurt row over the walk each time a hit takes health, and leaves an EnemyDeathEffect playing the Death row where it dies (the enemy itsel… |
| [BasicEnemy](#basicenemy) | MonoBehaviour | Nilly + Jason | Baseline component providing core stats, health management, status effect handling, and waypoint traversal along a coordinate path. Implements IDamageable and I… |
| [EnemyPath](#enemypath) | MonoBehaviour | Jason | The route enemies walk. Each child object is a waypoint, visited in Hierarchy order. Shows the route in the Scene view and lays down a road when the game starts… |
| [SilverfishClickReader](#silverfishclickreader) | MonoBehaviour | Nilly | Kills a SilverfishEnemy with one click, read from the game's actions asset. Jason's #43 took out his OnMouseDown in favour of this reader, so it is the only kil… |
| [EnemySpawner](#enemyspawner) | MonoBehaviour | Nilly + Jason | Controls enemy spawning for a 2D Tower Defense game. Handles map-specific wave configurations, sequential enemy spawning, and pauses at the end of each wave unt… |
| [MapWaveDataSO](#mapwavedataso) | ScriptableObject | Jason | ScriptableObject asset to configure and store map wave configurations in the Project. Create via: Assets &gt; Create &gt; Tower Defense &gt; Map Wave Config |
| [CentipedeEnemy](#centipedeenemy) | MonoBehaviour (enemy) | Nilly + Jason | Specialized subclass of BasicEnemy representing a Centipede enemy. When defeated, it splits into 2 smaller enemies with half of the parent's maximum health and … |
| [CentipedeSpeedUp](#centipedespeedup) | MonoBehaviour | Nilly | The Centipede boss's first mechanic, beside Jason's CentipedeEnemy, which does the second (splitting in two on death). Listens to his OnSplit and makes every ch… |
| [IFlyingEnemy](#iflyingenemy) | Interface | Nilly | An enemy that may be in the air. Melee and lobbed towers skip an enemy whose IsFlying is true (the wasp rule); straight shots, mines and the slow aura still rea… |
| [RhinoBeetleEnemy](#rhinobeetleenemy) | MonoBehaviour (enemy) | Nilly | The Rhino Beetle: a BasicEnemy behind an EnemyShield. Damage hits the shield first; what gets through reaches its health. The shield grows back after a few seco… |
| [RollyPollyEnemy](#rollypollyenemy) | MonoBehaviour (enemy) | Nilly | The Rolly Polly: a BasicEnemy that curls up when hit (see RollyPollyCurl). Curled, it takes only a share of each hit and rolls faster for a few tiles of path, t… |
| [SilverfishEnemy](#silverfishenemy) | MonoBehaviour (enemy) | Jason | A subclass of BasicEnemy that represents a Silverfish. The Silverfish enemy can be instantly defeated when clicked by the player. |
| [WaspEnemy](#waspenemy) | MonoBehaviour (enemy) | Nilly | The Wasp: a BasicEnemy that flies, so melee and lobbed towers cannot reach it (see IFlyingEnemy). Its stingers (hazard 2 in the GDD) come with the hazards step;… |
| [QueenMantisBodyMarkers](#queenmantisbodymarkers) | MonoBehaviour (boss part) | Nilly | Shows which bodies she can raise, so the player can keep the fallen away from her: a ring on each remembered body within her raise reach, fading as it gets olde… |
| [QueenMantisBoss](#queenmantisboss) | MonoBehaviour (enemy) | Nilly | The Queen Mantis, Meadow's boss: a BasicEnemy that changes shade each time she loses a third of her health, white, black, white (QueenMantisShadeCycle, GDD deci… |
| [QueenMantisData](#queenmantisdata) | ScriptableObject | Nilly | The numbers and art behind the Queen Mantis's mechanics: the pray pause, the white heal, the black raise, her summons and her looks. Data, not code (UNITY_RULES… |
| [QueenMantisHealAura](#queenmantishealaura) | MonoBehaviour (boss part) | Nilly | The white shade: every HealInterval seconds she heals the hurt enemies within HealRadius (never herself, QueenMantisHealRule) with Jason's Heal, and a QueenMant… |
| [QueenMantisLook](#queenmantislook) | MonoBehaviour (boss part) | Nilly | How she looks: the White or Black top-down sprite for her shade, the Shift frames played through each pray pause (forwards into white, backwards into black), an… |
| [QueenMantisPart](#queenmantispart) | MonoBehaviour | Nilly | Base for each piece of the Queen Mantis (heal aura, raiser, summoner, look, body markers): a small component beside QueenMantisBoss that reads her QueenMantisDa… |
| [QueenMantisPulse](#queenmantispulse) | MonoBehaviour | Nilly | The ring that shows a Queen Mantis heal: it grows from her out to the heal radius and fades. Sits on a child of the Queen Mantis with its own SpriteRenderer (a … |
| [QueenMantisRaiser](#queenmantisraiser) | MonoBehaviour (boss part) | Nilly | The black shade. She remembers where enemies fall (BossDeathLog), in either shade. In black, every RaiseInterval seconds she picks up to HusksPerRaise of the fr… |
| [QueenMantisSummoner](#queenmantissummoner) | MonoBehaviour (boss part) | Nilly | Her escort: SummonOnEntry Mantis Spawn when she walks in and SummonOnShadeChange more at each shade change, on a ring around her (QueenMantisSummonRule). With E… |
| [EnemyRegistry](#enemyregistry) | Static class | Nilly | Every living enemy that carries a RegisteredEnemy, so towers can look for targets without a scene search or physics queries. Enemies made outside the spawner (c… |
| [RegisteredEnemy](#registeredenemy) | MonoBehaviour | Nilly | Puts this enemy in the EnemyRegistry while it is alive, so towers can target it, and tells the registry when it is killed. Goes on every enemy prefab next to Ba… |
| [BossDeathLog](#bossdeathlog) | Class | Nilly | The Queen Mantis's memory of recent deaths, as plain C# so EditMode tests can cover it. Only the last Capacity deaths are kept. Her own death and the deaths of … |
| [EnemyShield](#enemyshield) | Class | Nilly | The Rhino Beetle's shield, as plain C# so EditMode tests can cover it. Damage hits the shield first and only what is left reaches the enemy's health. Once the s… |
| [QueenMantisHealRule](#queenmantishealrule) | Static class | Nilly | Who her white heal reaches, as plain C# so EditMode tests can cover it. |
| [QueenMantisShade](#queenmantisshade) | Enum | Nilly | The Queen Mantis's two shades (GDD decision 98). |
| [QueenMantisShadeCycle](#queenmantisshadecycle) | Class | Nilly | Her shade over one fight, as plain C# so EditMode tests can cover it. It only moves forward: healing never takes her back a third. It moves one third per TryAdv… |
| [QueenMantisSummonRule](#queenmantissummonrule) | Static class | Nilly | Where her summoned Mantis Spawn appear, as plain C# so EditMode tests can cover it. |
| [RollyPollyCurl](#rollypollycurl) | Class | Nilly | The Rolly Polly's curl, as plain C# so EditMode tests can cover it. A hit on an uncurled Rolly Polly lands in full and makes it curl; while curled it takes only… |

### EnemyDeathEffect

`unity/Assets/_Game/Features/Enemies/Scripts/Animation/EnemyDeathEffect.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/Animation/EnemyDeathEffect.cs)) · MonoBehaviour · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly

What is left where an enemy died: a copy of its body playing the Death row once, then fading away. Made at run time by EnemyHurtAndDeath, in the enemy's own sce…

Derives from / implements: MonoBehaviour

Named in: EnemyHurtAndDeath, SpriteRowTests

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Spawn` | Method (static) | `EnemyDeathEffect` | `(SpriteRenderer body, Sprite[] frames, float frameSeconds, float fadeSeconds, Scene scene)` | Leaves a death at body's place, size, turn and colour, playing frames. Does nothing without a body or frames. | EnemyHurtAndDeath |
| `Opacity` | Method (static) | `float` | `(float clock, float rowSeconds, float fadeSeconds)` | How opaque the body is clock seconds after death: solid while the row plays, then fading to nothing over fadeSeconds. | SpriteRowTests |

Private members not listed: 8.

### EnemyHurtAndDeath

`unity/Assets/_Game/Features/Enemies/Scripts/Animation/EnemyHurtAndDeath.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/Animation/EnemyHurtAndDeath.cs)) · MonoBehaviour · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly

Plays the enemy sheet's Hurt row over the walk each time a hit takes health, and leaves an EnemyDeathEffect playing the Death row where it dies (the enemy itsel…

Derives from / implements: MonoBehaviour

Used on: Enemy_Basic, Enemy_Centipede, Enemy_RhinoBeetle, Enemy_Silverfish, Enemy_Wasp

Named in: AlphaSpritesBuilder, EnemyHurtAndDeathDataTests

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_body` | `SpriteRenderer` |  |  | Enemy_Basic = Enemy_Basic/Body (SpriteRenderer); Enemy_Centipede = Enemy_Centipede/Body (SpriteRenderer); Enemy_RhinoBeetle = Enemy_RhinoBeetle/Body (SpriteRend… | The renderer the enemy is drawn with (its Body). |
| `_walk` | `SpriteFlipbook` |  |  | Enemy_Basic = Enemy_Basic/Body (SpriteFlipbook); Enemy_Centipede = Enemy_Centipede/Body (SpriteFlipbook); Enemy_RhinoBeetle = Enemy_RhinoBeetle/Body (SpriteFlip… | The walk loop, held while the hurt frames show. |
| `_hurtFrames` | `Sprite[]` | `System.Array.Empty<Sprite>()` | Hurt | Enemy_Basic = 2 item(s); Enemy_Centipede = 2 item(s); Enemy_RhinoBeetle = 2 item(s); Enemy_Silverfish = 2 item(s); Enemy_Wasp = 2 item(s) |  |
| `_hurtFrameSeconds` | `float` | `0.08f` | Hurt | Enemy_Basic = 0.06; Enemy_Centipede = 0.06; Enemy_RhinoBeetle = 0.06; Enemy_Silverfish = 0.06; Enemy_Wasp = 0.06 |  |
| `_deathFrames` | `Sprite[]` | `System.Array.Empty<Sprite>()` | Death | Enemy_Basic = 7 item(s); Enemy_Centipede = 7 item(s); Enemy_RhinoBeetle = 7 item(s); Enemy_Silverfish = 7 item(s); Enemy_Wasp = 7 item(s) |  |
| `_deathFrameSeconds` | `float` | `0.08f` | Death | Enemy_Basic = 0.08; Enemy_Centipede = 0.08; Enemy_RhinoBeetle = 0.08; Enemy_Silverfish = 0.08; Enemy_Wasp = 0.08 |  |
| `_deathFadeSeconds` | `float` | `0.35f` | Death | Enemy_Basic = 0.35; Enemy_Centipede = 0.35; Enemy_RhinoBeetle = 0.35; Enemy_Silverfish = 0.35; Enemy_Wasp = 0.35 | Seconds the body takes to fade after the last death frame. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Hurting` | Property | `bool` | `=> _hurting` | True while the hurt frames show. |  |

Private members not listed: 12.

### BasicEnemy

`unity/Assets/_Game/Features/Enemies/Scripts/Basic Enemy/BasicEnemy.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/Basic Enemy/BasicEnemy.cs)) · MonoBehaviour · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly + Jason · [documentation page](../../Documentation/EnemyScripts_Documentation.md)

Baseline component providing core stats, health management, status effect handling, and waypoint traversal along a coordinate path. Implements IDamageable and I…

Derives from / implements: MonoBehaviour, IDamageable, IHealthReadable

Used on: Enemy_Basic, Triangle

Named in: AlphaBossesBuilder, AlphaCoreLoopBuilder, AlphaEnemiesBuilder, AlphaPlaytestToolsBuilder, AlphaSpritesBuilder, BasicEnemyTests, BirdShadow, Bounty, CentipedeEnemy, CentipedeEnemyTests, DevKeys, DiatomaceousEarth, EnemyHurtAndDeath, EnemyRegistry, EnemySpawner, EnemySpawnerTests, LevelPowerUps, Mine, Projectile, Queen, QueenMantisBoss, QueenMantisBuilder, QueenMantisData, QueenMantisHealAura, QueenMantisPart, QueenMantisRaiser, QueenMantisSummoner, QueenTests, RegisteredEnemy, RhinoBeetleEnemy, RollyPollyEnemy, SilverfishClickReader, SilverfishEnemy, SlowAura, Tower, WaspEnemy, WaspStingerDropper

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_maxHealth` | `double` | `100.0` | Health & Combat | Enemy_Basic; Enemy_Centipede; Enemy_MantisSpawn; Enemy_QueenMantis; Enemy_RhinoBeetle; Enemy_RollyPolly; Enemy_Scarab; Enemy_Silverfish (+2 more) | Maximum health of this enemy. |
| `_damage` | `int` | `1` | Health & Combat | Enemy_Basic; Enemy_Centipede; Enemy_MantisSpawn; Enemy_QueenMantis; Enemy_RhinoBeetle; Enemy_RollyPolly; Enemy_Scarab; Enemy_Silverfish (+2 more) | Damage dealt by this enemy (e.g. to the hill or player base upon reaching destination). |
| `_moveSpeed` | `float` | `2f` | Path & Movement | Enemy_Basic; Enemy_Centipede; Enemy_MantisSpawn; Enemy_QueenMantis; Enemy_RhinoBeetle; Enemy_RollyPolly; Enemy_Scarab; Enemy_Silverfish (+2 more) | Movement speed along the path in world units per second. |
| `_waypointTolerance` | `float` | `0.05f` | Path & Movement | Enemy_Basic; Enemy_Centipede; Enemy_QueenMantis; Enemy_RhinoBeetle; Enemy_RollyPolly; Enemy_Silverfish; Enemy_Wasp; Triangle | Distance threshold to consider a waypoint coordinate reached. |
| `_pathCoordinates` | `List<Vector3>` | `new List<Vector3>()` | Path & Movement | Enemy_Basic; Enemy_Centipede; Enemy_QueenMantis; Enemy_RhinoBeetle; Enemy_RollyPolly; Enemy_Silverfish; Enemy_Wasp; Triangle | Initial path coordinates in world space. Can also be assigned dynamically via SetPath(). |
| `_flipSpriteFacing` | `bool` | `true` | Visuals | Enemy_Basic; Enemy_Centipede; Enemy_QueenMantis; Enemy_RhinoBeetle; Enemy_RollyPolly; Enemy_Silverfish; Enemy_Wasp; Triangle | If true and a SpriteRenderer is present, flips sprite on the X axis to match movement direction. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `_currentHealth` | Field (protected) | `double` |  |  |  |
| `_currentWaypointIndex` | Field (protected) | `int` |  |  |  |
| `_speedMultiplier` | Field (protected) | `float` |  |  |  |
| `_spriteRenderer` | Field (protected) | `SpriteRenderer` |  |  |  |
| `_activeStatusEffects` | Field (protected readonly) | `Dictionary<string, ActiveStatusEffect>` |  |  |  |
| `OnDied` | Event | `Action<BasicEnemy>` |  |  | BasicEnemyTests, Bounty, EnemyHurtAndDeath, RegisteredEnemy |
| `OnAnyDied` | Event (static) | `Action<BasicEnemy>` |  |  | EnemySpawner |
| `OnEnemyDied` | Event (static) | `Action<BasicEnemy>` | `custom add/remove` |  |  |
| `OnAnyEnemiesSpawned` | Event (static) | `Action<int>` |  |  | EnemySpawner |
| `OnDestinationReached` | Event | `Action<BasicEnemy>` |  |  | BasicEnemyTests, Queen, RegisteredEnemy |
| `OnAnyDestinationReached` | Event (static) | `Action<BasicEnemy>` |  |  | EnemySpawner, Queen, QueenTests |
| `OnHealthChanged` | Event | `Action<double, double>` |  |  | QueenTests |
| `OnDamaged` | Event | `Action<BasicEnemy, DamageInfo, float>` |  |  | BasicEnemyTests, EnemyHurtAndDeath |
| `OnAnyDamaged` | Event (static) | `Action<BasicEnemy, DamageInfo, float>` |  |  |  |
| `MaxHealth` | Property | `double` | `=> _maxHealth` | Maximum health value as a double. | BasicEnemyTests, CentipedeEnemyTests, DevKeys, QueenMantisHealAura, QueenMantisRaiser, QueenTests |
| `CurrentHealth` | Property | `double` | `=> _currentHealth` | Current health value as a double. | BasicEnemyTests, CentipedeEnemyTests, EnemySpawnerTests, QueenMantisHealAura, QueenTests, SilverfishClickReader |
| `Damage` | Property | `int` | `{ get; set; }` | Damage value as an int. | AlphaCoreLoopBuilder, AlphaEnemiesBuilder, BasicEnemyTests, CentipedeEnemy, CentipedeEnemyTests, Queen, QueenTests, Tower |
| `MoveSpeed` | Property | `float` | `{ get; set; }` | Movement speed in world units per second. | CentipedeEnemy, QueenMantisSummoner |
| `SpeedMultiplier` | Property | `float` | `=> _speedMultiplier` | Current movement speed multiplier applied by active status effects (slows). | BasicEnemyTests, RollyPollyEnemy |
| `PathCoordinates` | Property | `IReadOnlyList<Vector3>` | `=> _pathCoordinates` | List of coordinates that this enemy follows. | BasicEnemyTests, CentipedeEnemyTests, QueenMantisPart, Tower |
| `CurrentWaypointIndex` | Property | `int` | `{ get; set; }` | Index of the next waypoint coordinate being pursued. | CentipedeEnemyTests, QueenMantisRaiser, QueenMantisSummoner, Tower |
| `IsDead` | Property | `bool` | `{ get; protected set; }` | True if health has been depleted and death sequence has started. | BasicEnemyTests, BirdShadow, CentipedeEnemyTests, DiatomaceousEarth, EnemySpawnerTests, Mine, Projectile, QueenMantisHealAura, QueenMantisRaiser, SilverfishClic… |
| `ReachedEnd` | Property | `bool` | `{ get; protected set; }` | True once the enemy has reached the end of its path (the Queen). From then on it is out of the fight: hits, heals, status effects and death do nothing, so a hit… | BasicEnemyTests, EnemySpawnerTests |
| `IsActive` | Property | `bool` | `=> !IsDead && !ReachedEnd` | False once the enemy is dead or has reached the end of its path. | BasicEnemyTests, EnemyHurtAndDeath |
| `Transform` | Property | `Transform` | `=> transform` | Transform of this enemy, satisfying IDamageable for aiming and distance queries. | BasicEnemyTests |
| `IHealthReadable.CurrentHealth` | Property (public (explicit interface)) | `float` | `=> (float)_currentHealth` | Current health exposed as float for tower targeting priorities (IHealthReadable). |  |
| `IHealthReadable.MaxHealth` | Property (public (explicit interface)) | `float` | `=> (float)_maxHealth` | Max health exposed as float for tower targeting priorities (IHealthReadable). |  |
| `Awake` | Method (protected virtual) | `void` | `()` |  | Bounty, DevKeys, EnemyHurtAndDeath, EnemySpawner, LevelPowerUps, Queen, QueenMantisPart, QueenMantisRaiser, RegisteredEnemy, RhinoBeetleEnemy, RollyPollyEnemy, … |
| `Start` | Method (protected virtual) | `void` | `()` |  | BirdShadow, Bounty, DiatomaceousEarth, EnemySpawner, QueenMantisBoss |
| `Update` | Method (protected virtual) | `void` | `()` |  | BirdShadow, DevKeys, DiatomaceousEarth, EnemyHurtAndDeath, LevelPowerUps, Mine, Projectile, QueenMantisBoss, QueenMantisHealAura, QueenMantisRaiser, RhinoBeetle… |
| `OnDisable` | Method (protected virtual) | `void` | `()` |  | Bounty, EnemyHurtAndDeath, EnemySpawner, LevelPowerUps, Queen, QueenMantisBoss, QueenMantisPart, QueenMantisRaiser, RegisteredEnemy, SilverfishClickReader, Towe… |
| `UpdateStatusEffects` | Method (virtual) | `void` | `(float deltaTime)` | Ticks all active status effects, handles periodic damage-over-time, and clears expired effects. | BasicEnemyTests |
| `RecalculateSpeedMultiplier` | Method (protected virtual) | `void` | `()` | Recalculates the speed multiplier based on the strongest active slow effect. |  |
| `TraversePath` | Method (protected virtual) | `void` | `()` | Moves the enemy towards the current target waypoint, scaled by moveSpeed and active slows. | QueenMantisBoss |
| `UpdateFacing` | Method (protected virtual) | `void` | `(Vector3 direction)` | Flips the sprite horizontally according to travel direction. |  |
| `TakeDamage` | Method (virtual) | `void` | `(DamageInfo info)` | Applies damage via DamageInfo (IDamageable). Subtracts health, raises OnDamaged and OnAnyDamaged with the hit details, and triggers Die() at 0 health. | BasicEnemyTests, BirdShadow, CentipedeEnemyTests, DevKeys, DiatomaceousEarth, EnemySpawnerTests, Mine, Projectile, Queen, QueenTests, RhinoBeetleEnemy, RollyPol… |
| `TakeDamage` | Method (virtual) | `void` | `(double damageAmount)` | Applies damage directly as a raw double amount. | BasicEnemyTests, BirdShadow, CentipedeEnemyTests, DevKeys, DiatomaceousEarth, EnemySpawnerTests, Mine, Projectile, Queen, QueenTests, RhinoBeetleEnemy, RollyPol… |
| `ApplyStatusEffect` | Method (virtual) | `void` | `(StatusEffectData status)` | Applies a damage-over-time and/or slow status effect (IDamageable). Re-applying an effect with the same EffectName refreshes it. | BasicEnemyTests, SlowAura |
| `Heal` | Method (virtual) | `void` | `(double healAmount)` | Restores health up to the maximum health. | Queen, QueenMantisHealAura, QueenTests |
| `SetMaxHealth` | Method (virtual) | `void` | `(double newMaxHealth, bool resetCurrentHealth = true)` | Sets the maximum health and optionally resets current health to match. | BasicEnemyTests, CentipedeEnemy, CentipedeEnemyTests, EnemySpawnerTests, Queen, QueenMantisRaiser |
| `SetPath` | Method | `void` | `(IEnumerable<Vector3> coordinates, bool teleportToStart = false)` | Assigns a new ordered sequence of coordinates for the enemy to traverse. | BasicEnemyTests, CentipedeEnemy, CentipedeEnemyTests, DevKeys, QueenMantisPart |
| `SetPath` | Method | `void` | `(IEnumerable<Vector3> coordinates, int startingWaypointIndex, bool teleportToStart = false)` | Assigns a new ordered sequence of coordinates and specifies the starting waypoint index. | BasicEnemyTests, CentipedeEnemy, CentipedeEnemyTests, DevKeys, QueenMantisPart |
| `SetPath` | Method | `void` | `(IEnumerable<Vector2> coordinates, bool teleportToStart = false)` | Overload for setting path coordinates using 2D coordinates (Vector2). | BasicEnemyTests, CentipedeEnemy, CentipedeEnemyTests, DevKeys, QueenMantisPart |
| `SetPath` | Method | `void` | `(EnemyPath enemyPath, bool teleportToStart = false)` | Assigns the path using an existing EnemyPath component. | BasicEnemyTests, CentipedeEnemy, CentipedeEnemyTests, DevKeys, QueenMantisPart |
| `Knockback` | Method (virtual) | `void` | `(float distance)` | Pushes the enemy backward along its route coordinates by the specified distance. | BasicEnemyTests |
| `OnReachedDestination` | Method (protected virtual) | `void` | `()` | Invoked when the enemy reaches the final waypoint in the path. |  |
| `ReachDestination` | Method | `void` | `()` | Triggers the destination reached sequence manually (useful for testing or external events). | BasicEnemyTests, EnemySpawnerTests, QueenTests |
| `Die` | Method (protected virtual) | `void` | `()` | Invoked when health reaches zero. | CentipedeEnemy |
| `DestroySelf` | Method (protected) | `void` | `()` | Removes the enemy. Unity refuses Destroy outside Play mode, so EditMode tests that kill an enemy, or bring one to the Queen, remove it at once instead. |  |
| `NotifyEnemiesSpawned` | Method (static) | `void` | `(int count)` | Fires an event indicating that enemies were spawned by an existing enemy (e.g., splitting or summoning). | EnemySpawnerTests |

#### BasicEnemy.ActiveStatusEffect (class)

Internal tracking container for an active status effect (slow / DoT).

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `Data` | `StatusEffectData` |  |  |  |  |
| `RemainingDuration` | `float` |  |  |  |  |
| `TimeUntilNextTick` | `float` |  |  |  |  |

Private members not listed: 3.

### EnemyPath

`unity/Assets/_Game/Features/Enemies/Scripts/Basic Enemy/EnemyPath.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/Basic Enemy/EnemyPath.cs)) · MonoBehaviour · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Jason

The route enemies walk. Each child object is a waypoint, visited in Hierarchy order. Shows the route in the Scene view and lays down a road when the game starts…

Derives from / implements: MonoBehaviour

Used on: Hill01_Gameplay, L00_TutorialHill_Gameplay, L01_Meadow_Gameplay, Path and Queen Test

Named in: AlphaCoreLoopBuilder, AlphaHomeAndLevelsBuilder, AlphaRunFrameBuilder, BasicEnemy, DevKeys, HoldTheHillMenu, MineLayer, TowerPlacer

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_roadWidth` | `float` | `1f` |  | Hill01_Gameplay = 1; L00_TutorialHill_Gameplay = 1; L01_Meadow_Gameplay = 1; Path and Queen Test = 1 | Width of the road in world units. Towers can't be placed on it. |
| `_roadColor` | `Color` | `new Color(0.55f, 0.42f, 0.3f)` |  | Hill01_Gameplay = (0.55, 0.42, 0.3, 1); L00_TutorialHill_Gameplay = (0.55, 0.42, 0.3, 1); L01_Meadow_Gameplay = (0.55, 0.42, 0.3, 1); Path and Queen Test = (0.5… |  |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `RoadName` | Const (private) | `string` |  |  |  |
| `Waypoints` | Property | `IReadOnlyList<Vector3>` | `=> _waypoints` | Waypoint positions in world space, in walking order. | BasicEnemy, DevKeys, MineLayer |
| `Blocks` | Method | `bool` | `(Vector3 cellCenter, float cellSize)` | True if a grid cell centered at cellCenter overlaps the road. | TowerPlacer |

Private members not listed: 6.

### SilverfishClickReader

`unity/Assets/_Game/Features/Enemies/Scripts/Click/SilverfishClickReader.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/Click/SilverfishClickReader.cs)) · MonoBehaviour · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly

Kills a SilverfishEnemy with one click, read from the game's actions asset. Jason's #43 took out his OnMouseDown in favour of this reader, so it is the only kil…

Derives from / implements: MonoBehaviour

Used on: L00_TutorialHill_Gameplay, L01_Meadow_Gameplay

Named in: AlphaEnemiesBuilder, LevelPowerUps, TowerPlacer

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_actions` | `InputActionAsset` |  |  | L00_TutorialHill_Gameplay = HoldTheHill; L01_Meadow_Gameplay = HoldTheHill | The game's actions asset. Uses its Gameplay map: Point and Place. |
| `_fallbackRadius` | `float` | `0.4f` |  | L00_TutorialHill_Gameplay = 0.4; L01_Meadow_Gameplay = 0.4 | A silverfish with no collider counts as clicked within this distance of its centre, in world units. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `SilverfishClicked` | Event | `System.Action<SilverfishEnemy>` |  | Raised after a click has killed a silverfish. |  |
| `ClickUsedThisFrame` | Property (static) | `bool` | `=> s_usedFrame == Time.frameCount` | True on the frame a click killed a silverfish: that click selects, builds and collects nothing. | LevelPowerUps, TowerPlacer |
| `FindSilverfishAt` | Method | `SilverfishEnemy` | `(Vector3 world)` | The first living silverfish under world, or null. |  |

Private members not listed: 11.

### EnemySpawner

`unity/Assets/_Game/Features/Enemies/Scripts/EnemySpawner/EnemySpawner.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/EnemySpawner/EnemySpawner.cs)) · MonoBehaviour · `global namespace` · HoldTheHill.Runtime · owner: Nilly + Jason · [documentation page](../../Documentation/EnemySpawner_Documentation.md)

Controls enemy spawning for a 2D Tower Defense game. Handles map-specific wave configurations, sequential enemy spawning, and pauses at the end of each wave unt…

Derives from / implements: MonoBehaviour

Used on: Hill01_Gameplay, L00_TutorialHill_Gameplay, L01_Meadow_Gameplay, Path and Queen Test

Named in: AlphaCoreLoopBuilder, AlphaHazardsBuilder, AlphaRunFrameBuilder, EnemySpawnerTests, HazardScheduler, LevelRun, NextWaveRule, NextWaveRuleTests, WaveLabel, WaveReward

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `defaultSpawnPoint` | `Transform` | `null` | Spawn Position | Hill01_Gameplay = Enemy Path/Waypoint 0 (Transform); L00_TutorialHill_Gameplay = Enemy Path/Waypoint 0 (Transform); L01_Meadow_Gameplay = Enemy Path/Waypoint 0 … | Default 2D spawn location. If null, this GameObject's transform is used. |
| `enemyContainer` | `Transform` | `null` | Spawn Position | Hill01_Gameplay = (none); L00_TutorialHill_Gameplay = (none); L01_Meadow_Gameplay = (none); Path and Queen Test = (none) | Optional parent transform to organize spawned enemies in the Hierarchy. |
| `activeMapId` | `string` | `"Map_01"` | Map Configuration | Hill01_Gameplay = Map_01; L00_TutorialHill_Gameplay = Map_01; L01_Meadow_Gameplay = Map_01; Path and Queen Test = Map_01 | ID or name of the map currently active (e.g., 'Level1', 'Grasslands'). |
| `autoDetectMapFromScene` | `bool` | `false` | Map Configuration | Hill01_Gameplay = 0; L00_TutorialHill_Gameplay = 0; L01_Meadow_Gameplay = 0; Path and Queen Test = 0 | If true, automatically sets activeMapId to the active Scene name upon start. |
| `activeMapConfigOverride` | `MapWaveDataSO` | `null` | Map Configuration | Hill01_Gameplay = Hill01_Waves; L00_TutorialHill_Gameplay = L00_TutorialHill_Waves; L01_Meadow_Gameplay = L01_Meadow_Waves; Path and Queen Test = Map_01 | Direct override asset for this map's waves. If assigned, this takes top priority. |
| `scriptableMapConfigs` | `List<MapWaveDataSO>` | `new List<MapWaveDataSO>()` | Map Configuration | Hill01_Gameplay = 0 item(s); L00_TutorialHill_Gameplay = 0 item(s); L01_Meadow_Gameplay = 0 item(s); Path and Queen Test = 0 item(s) | List of ScriptableObject wave configurations for different maps. |
| `inlineMapConfigs` | `List<MapWaveData>` | `new List<MapWaveData>()` | Map Configuration | Hill01_Gameplay = 0 item(s); L00_TutorialHill_Gameplay = 0 item(s); L01_Meadow_Gameplay = 0 item(s); Path and Queen Test = 0 item(s) | Inline wave configurations defined directly in this component's inspector. |
| `autoStartFirstWave` | `bool` | `false` | Wave Progression Settings | Hill01_Gameplay = 0; L00_TutorialHill_Gameplay = 0; L01_Meadow_Gameplay = 0; Path and Queen Test = 1 | If true, Wave 1 starts automatically when the scene loads. If false, waits for StartNextWave() prompt. |
| `requireEnemiesClearedBeforeNextWave` | `bool` | `true` | Wave Progression Settings | Hill01_Gameplay = 0; L00_TutorialHill_Gameplay = 0; L01_Meadow_Gameplay = 0; Path and Queen Test = 0 | If true, requires all living enemies of the current wave to be defeated before StartNextWave() can be triggered. |
| `logProgress` | `bool` | `false` | Debug |  | If true, logs each wave starting and finishing and map changes to the Console. Warnings are always logged. |
| `onEnemySpawned` | `UnityEvent<GameObject>` |  | Events | Hill01_Gameplay = 0 listener(s); L00_TutorialHill_Gameplay = 0 listener(s); L01_Meadow_Gameplay = 0 listener(s); Path and Queen Test = 0 listener(s) | Invoked whenever an enemy is instantiated. |
| `onWaveStarted` | `UnityEvent<int, int>` |  | Events | Hill01_Gameplay = 0 listener(s); L00_TutorialHill_Gameplay = 0 listener(s); L01_Meadow_Gameplay = 0 listener(s); Path and Queen Test = 0 listener(s) | Invoked when a wave starts spawning. Passes (currentWaveNumber, totalWaves). |
| `onWaveCompleted` | `UnityEvent<int>` |  | Events | Hill01_Gameplay = 0 listener(s); L00_TutorialHill_Gameplay = 0 listener(s); L01_Meadow_Gameplay = 0 listener(s); Path and Queen Test = 0 listener(s) | Invoked when all enemies in the current wave have finished spawning. Passes (completedWaveNumber). |
| `onAllWavesCompleted` | `UnityEvent` |  | Events | Hill01_Gameplay = 0 listener(s); L00_TutorialHill_Gameplay = 0 listener(s); L01_Meadow_Gameplay = 0 listener(s); Path and Queen Test = 0 listener(s) | Invoked when all waves for the active map have been finished. |
| `onMapChanged` | `UnityEvent<string>` |  | Events | Hill01_Gameplay = 0 listener(s); L00_TutorialHill_Gameplay = 0 listener(s); L01_Meadow_Gameplay = 0 listener(s); Path and Queen Test = 0 listener(s) | Invoked when the active map changes. Passes (mapId). |
| `livingEnemyCount` | `int` | `0` | Events |  | Number of living enemies currently tracked on the map. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `CurrentState` | Property | `SpawnerState` | `{ get; }` |  | LevelRun, WaveLabel |
| `ActiveMapId` | Property | `string` | `{ get; }` |  |  |
| `CurrentWaveIndex` | Property | `int` | `{ get; }` |  |  |
| `CurrentWaveNumber` | Property | `int` | `{ get; }` |  | WaveLabel |
| `TotalWaves` | Property | `int` | `{ get; }` |  | WaveLabel |
| `LivingEnemyCount` | Property | `int` | `{ get; }` | Current count of alive enemies on the map. Increments when this spawner spawns an enemy or when enemies spawn other enemies (e.g. Centipede splitting), and decr… | EnemySpawnerTests |
| `IsSpawning` | Property | `bool` | `{ get; }` |  |  |
| `SubscribeToEvents` | Method | `void` | `()` | Subscribes to enemy lifecycle events to track live enemy count. Safe to call multiple times. | EnemySpawnerTests |
| `UnsubscribeFromEvents` | Method | `void` | `()` | Unsubscribes from enemy lifecycle events. | EnemySpawnerTests |
| `HandleEnemyDied` | Method | `void` | `(BasicEnemy enemy)` | Event handler invoked when any enemy dies. Decrements the live enemy counter. |  |
| `HandleEnemyDestinationReached` | Method | `void` | `(BasicEnemy enemy)` | Event handler invoked when an enemy completes its path to the destination. Decrements the live enemy counter. |  |
| `HandleCentipedeSplit` | Method | `void` | `(CentipedeEnemy parent, List<CentipedeEnemy> children)` | Event handler invoked when a centipede enemy splits into child segments. Increments the live enemy counter by the number of spawned children. |  |
| `HandleEnemiesSpawned` | Method | `void` | `(int count)` | Event handler invoked when an enemy spawns additional enemies into the scene. Increments the live enemy counter. |  |
| `SetActiveMap` | Method | `void` | `(string mapId)` | Loads the wave configuration for the given map ID and resets wave progress. | EnemySpawnerTests |
| `StartNextWave` | Method | `void` | `()` | Prompts the spawner to begin the next wave. Can be wired to a UI Button OnClick() or invoked by game managers. | AlphaCoreLoopBuilder, LevelRun |
| `SpawnEnemy` | Method | `GameObject` | `(EnemySpawnEntry entry)` | Instantiates an enemy GameObject, tracks it, and increments the living enemy count. | EnemySpawnerTests |
| `SpawnEnemy` | Method | `GameObject` | `(GameObject enemyPrefab, Transform spawnPointOverride = null)` | Instantiates an enemy prefab directly at an optional spawn transform. Increments the living enemy counter. | EnemySpawnerTests |
| `TrackEnemy` | Method | `void` | `(GameObject enemy)` | Manually tracks an externally instantiated enemy, incrementing the living enemy count. |  |
| `NotifyEnemyDefeated` | Method | `void` | `(GameObject enemy)` | Allows external enemy logic (or health components) to notify the spawner when an enemy dies. Decrements the living enemy counter. | EnemySpawnerTests |
| `StopSpawning` | Method | `void` | `()` | Immediately stops any active wave spawning. |  |

#### EnemySpawner.SpawnerState (enum)

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Idle` | Enum member (static) | `SpawnerState` |  |  | NextWaveRule, WaveLabel |
| `Spawning` | Enum member (static) | `SpawnerState` |  |  |  |
| `WaitingForNextWave` | Enum member (static) | `SpawnerState` |  |  | NextWaveRule |
| `Completed` | Enum member (static) | `SpawnerState` |  |  | LevelRun |

Private members not listed: 14.

### MapWaveDataSO

`unity/Assets/_Game/Features/Enemies/Scripts/EnemySpawner/MapWaveDataSO.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/EnemySpawner/MapWaveDataSO.cs)) · ScriptableObject · `global namespace` · HoldTheHill.Runtime · owner: Jason · [documentation page](../../Documentation/MapWaveData_Documentation.md)

ScriptableObject asset to configure and store map wave configurations in the Project. Create via: Assets &gt; Create &gt; Tower Defense &gt; Map Wave Config

Derives from / implements: ScriptableObject

Used on: Hill01_Waves, L00_TutorialHill_Waves, L01_Meadow_Waves, Map_01

Named in: AlphaBossesBuilder, AlphaCoreLoopBuilder, AlphaEnemiesBuilder, AlphaHazardsBuilder, EnemySpawner, HazardScheduler, LevelHazardSchedule, QueenMantisBuilder

#### EnemySpawnEntry (class)

Represents a single enemy spawn event in a wave.

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `enemyPrefab` | `GameObject` |  |  |  | The 2D enemy prefab to instantiate. |
| `delayBeforeNext` | `float` | `1f` |  |  | Delay in seconds after this enemy is spawned before spawning the next enemy. |
| `spawnPointOverride` | `Transform` |  |  |  | Optional custom spawn point. If null, the spawner's default spawn point will be used. |

#### Wave (class)

Represents a wave containing a sequential list of enemies.

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `waveName` | `string` | `"Wave 1"` |  |  | Name or identifier for this wave (e.g. 'Wave 1', 'Boss Wave'). |
| `enemies` | `List<EnemySpawnEntry>` | `new List<EnemySpawnEntry>()` |  |  | Sequential list of enemies to spawn one at a time in this wave. |

#### MapWaveData (class)

Serializable data structure representing a map and its associated waves. Can be configured directly inside EnemySpawner inspector.

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `mapId` | `string` | `"Map_01"` |  |  | Unique ID or name for this map (e.g., 'Grasslands', 'Level1', or scene name). |
| `waves` | `List<Wave>` | `new List<Wave>()` |  |  | List of delineated waves for this map. |

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `mapId` | `string` | `"Map_01"` |  | Hill01_Waves = Hill01; L00_TutorialHill_Waves = L00_TutorialHill; L01_Meadow_Waves = L01_Meadow; Map_01 = Map_01 | Unique identifier for this map (e.g., 'Grasslands', 'Desert', or matching Scene name). |
| `waves` | `List<Wave>` | `new List<Wave>()` |  | Hill01_Waves = 3 item(s); L00_TutorialHill_Waves = 5 item(s); L01_Meadow_Waves = 10 item(s); Map_01 = 1 item(s) | Delineated waves for this map. |

### CentipedeEnemy

`unity/Assets/_Game/Features/Enemies/Scripts/EnemySubclasses/CentipedeEnemy.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/EnemySubclasses/CentipedeEnemy.cs)) · MonoBehaviour (enemy) · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly + Jason · [documentation page](../../Documentation/CentipedeEnemy_Documentation.md)

Specialized subclass of BasicEnemy representing a Centipede enemy. When defeated, it splits into 2 smaller enemies with half of the parent's maximum health and …

Derives from / implements: BasicEnemy

Used on: Enemy_Centipede

Named in: AlphaBossesBuilder, BirdShadow, CentipedeEnemyTests, CentipedeSpeedUp, EnemySpawner, EnemySpawnerTests

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_splitsRemaining` | `int` | `-1` | Centipede Splitting | Enemy_Centipede = 2 | Number of remaining splits allowed. Decrements with each split. Set to -1 for unlimited splits until 1 HP. |
| `_splitPrefab` | `GameObject` | `null` | Centipede Splitting | Enemy_Centipede = (none) | Optional prefab to instantiate for child segments; it must carry a CentipedeEnemy. If null, this GameObject is used as the template. |
| `_sizeMultiplier` | `float` | `0.8f` | Centipede Splitting | Enemy_Centipede = 0.8 | Visual scale multiplier applied to child enemies when splitting. |
| `_splitForwardDistance` | `float` | `1.0f` | Centipede Splitting | Enemy_Centipede = 0.6 | Distance forward along the path where the second child enemy is spawned. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `OnSplit` | Event | `Action<CentipedeEnemy, List<CentipedeEnemy>>` |  |  | CentipedeEnemyTests, CentipedeSpeedUp |
| `OnAnyCentipedeSplit` | Event (static) | `Action<CentipedeEnemy, List<CentipedeEnemy>>` |  |  | EnemySpawner |
| `SplitsRemaining` | Property | `int` | `{ get; set; }` | Gets or sets the number of splits remaining. -1 enables splitting until the 1 HP condition is reached. | BirdShadow, CentipedeEnemyTests, EnemySpawnerTests |
| `SizeMultiplier` | Property | `float` | `{ get; set; }` | Gets or sets the visual scale factor applied to child segments upon splitting. | CentipedeEnemyTests |
| `SplitForwardDistance` | Property | `float` | `{ get; set; }` | Gets or sets the distance forward along the path where the second child enemy is spawned. |  |
| `SplitOffsetDistance` | Property | `float` | `{ get; set; }` | Backward-compatible alias for SplitForwardDistance. |  |
| `SplitPrefab` | Property | `GameObject` | `{ get; set; }` | Optional prefab template for child enemies. If null, copies of this instance are instantiated. | CentipedeEnemyTests |
| `CanSplit` | Method | `bool` | `()` | Determines whether this centipede can split upon death based on remaining splits and health constraints. | CentipedeEnemyTests |
| `Die` | Method (protected override) | `void` | `()` | Overrides Die() to spawn 2 smaller child enemies when eligible before completing the death sequence. |  |
| `CalculatePositionAlongPath` | Method | `Vector3` | `(float forwardDistance, out int targetWaypointIndex)` | Calculates the world position and next target waypoint index after advancing a given distance forward along the path. | CentipedeEnemyTests |
| `RemainingPathDistance` | Method | `float` | `()` | Distance left to walk along the path, from where the centipede stands to the last waypoint. Zero with no path or once the last waypoint is passed. |  |
| `Split` | Method | `List<CentipedeEnemy>` | `()` | Spawns 2 smaller centipede enemies with half maximum health, half damage to the Queen and decremented splits remaining. The first child is spawned at the origin… | CentipedeEnemyTests, EnemySpawnerTests |

Private members not listed: 3.

### CentipedeSpeedUp

`unity/Assets/_Game/Features/Enemies/Scripts/EnemySubclasses/CentipedeSpeedUp.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/EnemySubclasses/CentipedeSpeedUp.cs)) · MonoBehaviour · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly

The Centipede boss's first mechanic, beside Jason's CentipedeEnemy, which does the second (splitting in two on death). Listens to his OnSplit and makes every ch…

Derives from / implements: MonoBehaviour

Used on: Enemy_Centipede

Named in: AlphaBossesBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_childSpeedFactor` | `float` | `1.25f` |  | Enemy_Centipede = 1.25 | Each child's speed is its parent's times this. 1.25 means a quarter faster per split. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `ChildSpeedFactor` | Property | `float` | `=> _childSpeedFactor` | How much faster each child is than its parent. |  |

Private members not listed: 6.

### IFlyingEnemy

`unity/Assets/_Game/Features/Enemies/Scripts/EnemySubclasses/IFlyingEnemy.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/EnemySubclasses/IFlyingEnemy.cs)) · Interface · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly

An enemy that may be in the air. Melee and lobbed towers skip an enemy whose IsFlying is true (the wasp rule); straight shots, mines and the slow aura still rea…

Named in: DiatomaceousEarth, Projectile, Tower, WaspEnemy, WaspStingerDropper

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `IsFlying` | Property | `bool` | `{ get; }` | True while this enemy is off the ground. | DiatomaceousEarth, Projectile, Tower, WaspStingerDropper |

### RhinoBeetleEnemy

`unity/Assets/_Game/Features/Enemies/Scripts/EnemySubclasses/RhinoBeetleEnemy.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/EnemySubclasses/RhinoBeetleEnemy.cs)) · MonoBehaviour (enemy) · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly

The Rhino Beetle: a BasicEnemy behind an EnemyShield. Damage hits the shield first; what gets through reaches its health. The shield grows back after a few seco…

Derives from / implements: BasicEnemy

Used on: Enemy_RhinoBeetle

Named in: AlphaEnemiesBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_shieldCapacity` | `float` | `40f` | Shield | Enemy_RhinoBeetle = 40 | How much damage a full shield soaks before anything reaches health. |
| `_shieldRegrowDelay` | `float` | `3f` | Shield | Enemy_RhinoBeetle = 3 | Seconds without a hit before the shield starts to grow back. |
| `_shieldRegrowPerSecond` | `float` | `25f` | Shield | Enemy_RhinoBeetle = 25 | Shield points regained per second once it is growing back. |
| `_shieldVisual` | `SpriteRenderer` |  | Shield | Enemy_RhinoBeetle = Enemy_RhinoBeetle/Shield (SpriteRenderer) | Optional: a sprite drawn over the beetle whose alpha follows the shield. Leave empty for none. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Shield` | Property | `EnemyShield` | `=> _shield` | The shield rule, for tests and the visual. |  |
| `Awake` | Method (protected override) | `void` | `()` |  |  |
| `Update` | Method (protected override) | `void` | `()` |  |  |
| `TakeDamage` | Method (override) | `void` | `(double damageAmount)` |  |  |

Private members not listed: 3.

### RollyPollyEnemy

`unity/Assets/_Game/Features/Enemies/Scripts/EnemySubclasses/RollyPollyEnemy.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/EnemySubclasses/RollyPollyEnemy.cs)) · MonoBehaviour (enemy) · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly

The Rolly Polly: a BasicEnemy that curls up when hit (see RollyPollyCurl). Curled, it takes only a share of each hit and rolls faster for a few tiles of path, t…

Derives from / implements: BasicEnemy

Used on: Enemy_RollyPolly

Named in: AlphaEnemiesBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_maxCurls` | `int` | `3` | Curl | Enemy_RollyPolly = 3 | How many times it can curl in one life. |
| `_rollDistance` | `float` | `3f` | Curl | Enemy_RollyPolly = 3 | Path distance rolled while curled before it uncurls, in tiles (one tile is one world unit). |
| `_curledDamageShare` | `float` | `0.4f` | Curl | Enemy_RollyPolly = 0.4 | The share of each hit that lands while curled. 0.4 means it takes 40 percent. |
| `_curledSpeedMultiplier` | `float` | `2f` | Curl | Enemy_RollyPolly = 2 | Movement speed multiplier while rolling. |
| `_curledScale` | `float` | `0.75f` | Curl | Enemy_RollyPolly = 0.75 | How much smaller the sprite is drawn while curled. |
| `_curledTint` | `Color` | `new Color(0.55f, 0.55f, 0.65f)` | Curl | Enemy_RollyPolly = (0.55, 0.55, 0.65, 1) | Sprite tint while curled; the uncurled tint is whatever the prefab has. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Curl` | Property | `RollyPollyCurl` | `=> _curl` | The curl rule, for tests and the visual. |  |
| `Awake` | Method (protected override) | `void` | `()` |  |  |
| `Update` | Method (protected override) | `void` | `()` |  |  |
| `TakeDamage` | Method (override) | `void` | `(double damageAmount)` |  |  |

Private members not listed: 5.

### SilverfishEnemy

`unity/Assets/_Game/Features/Enemies/Scripts/EnemySubclasses/SilverfishEnemy.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/EnemySubclasses/SilverfishEnemy.cs)) · MonoBehaviour (enemy) · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Jason · [documentation page](../../Documentation/SilverfishEnemy_Documentation.md)

A subclass of BasicEnemy that represents a Silverfish. The Silverfish enemy can be instantly defeated when clicked by the player.

Derives from / implements: BasicEnemy

Used on: Enemy_Silverfish

Named in: AlphaEnemiesBuilder, SilverfishClickReader

### WaspEnemy

`unity/Assets/_Game/Features/Enemies/Scripts/EnemySubclasses/WaspEnemy.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/EnemySubclasses/WaspEnemy.cs)) · MonoBehaviour (enemy) · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly

The Wasp: a BasicEnemy that flies, so melee and lobbed towers cannot reach it (see IFlyingEnemy). Its stingers (hazard 2 in the GDD) come with the hazards step;…

Derives from / implements: BasicEnemy, IFlyingEnemy

Used on: Enemy_Wasp

Named in: AlphaEnemiesBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_isFlying` | `bool` | `true` | Wasp | Enemy_Wasp = 1 | While on, melee and lobbed towers skip this enemy. Off makes it an ordinary walker. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `IsFlying` | Property | `bool` | `{ get; set; }` | True while this wasp is in the air. |  |

### QueenMantisBodyMarkers

`unity/Assets/_Game/Features/Enemies/Scripts/QueenMantis/QueenMantisBodyMarkers.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/QueenMantis/QueenMantisBodyMarkers.cs)) · MonoBehaviour (boss part) · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly

Shows which bodies she can raise, so the player can keep the fallen away from her: a ring on each remembered body within her raise reach, fading as it gets olde…

Derives from / implements: QueenMantisPart

Used on: Enemy_QueenMantis

Named in: QueenMantisBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_markerSize` | `float` | `0.5f` |  | Enemy_QueenMantis = 0.5 | World units across the ring of a body she can raise. |
| `_windUpSize` | `float` | `1f` |  | Enemy_QueenMantis = 1 | World units across a chosen body's ring at the end of the wind-up. |
| `_sortingOrder` | `int` | `1` |  | Enemy_QueenMantis = 1 | Sorting order of the rings: over the level's terrain (0), under the hill (4) and the enemies (5). |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Awake` | Method (protected override) | `void` | `()` |  |  |

Private members not listed: 6.

### QueenMantisBoss

`unity/Assets/_Game/Features/Enemies/Scripts/QueenMantis/QueenMantisBoss.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/QueenMantis/QueenMantisBoss.cs)) · MonoBehaviour (enemy) · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly

The Queen Mantis, Meadow's boss: a BasicEnemy that changes shade each time she loses a third of her health, white, black, white (QueenMantisShadeCycle, GDD deci…

Derives from / implements: BasicEnemy

Used on: Enemy_QueenMantis

Named in: AlphaBossesBuilder, DevKeys, QueenMantisBossBar, QueenMantisBuilder, QueenMantisPart

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_data` | `QueenMantisData` |  | Queen Mantis | Enemy_QueenMantis = QueenMantisData | Her mechanic numbers, art and the Mantis Spawn prefab. Health, speed and damage are the fields above. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Appeared` | Event (static) | `Action<QueenMantisBoss>` |  | A Queen Mantis walked in. Raised once per Queen Mantis, after her own Entered. | QueenMantisBossBar |
| `Gone` | Event (static) | `Action<QueenMantisBoss>` |  | A Queen Mantis left: killed, at the Queen, or unloaded with her level. | QueenMantisBossBar |
| `Live` | Property (static) | `IReadOnlyList<QueenMantisBoss>` | `=> s_live` | Every Queen Mantis walking right now, oldest first; the HUD bar shows the first. | DevKeys, QueenMantisBossBar |
| `Entered` | Event | `Action` |  | She has her road and is walking in. | QueenMantisPart |
| `ShadeChanged` | Event | `Action<QueenMantisShade>` |  | She stepped into a new third and turned this shade; her pray pause starts now. | QueenMantisBossBar, QueenMantisPart |
| `PrayEnded` | Event | `Action` |  | Her pray pause is over. | QueenMantisBossBar, QueenMantisPart |
| `Data` | Property | `QueenMantisData` | `=> _data` | Her numbers and art. | AlphaBossesBuilder, QueenMantisPart |
| `Shade` | Property | `QueenMantisShade` | `=> _cycle.Shade` | Her shade now. | QueenMantisBossBar |
| `ThirdsLost` | Property | `int` | `=> _cycle.ThirdsLost` | How many thirds of her health she has changed shade for: 0, 1 or 2. |  |
| `IsPraying` | Property | `bool` | `=> _prayLeft > 0f` | True during a pray pause, when she stands still and can't heal or raise. | QueenMantisBossBar |
| `PrayProgress` | Property | `float` | `=> _prayLength > 0f ? 1f - _prayLeft / _prayLength : 1f` | How far through the current pray pause she is, 0 to 1; 1 when not praying. |  |
| `CanAct` | Property | `bool` | `=> !IsDead && !IsPraying` | True while her mechanics may act: alive and not praying. |  |
| `HealthFraction` | Property | `float` | `=> MaxHealth > 0.0 ? (float)(CurrentHealth / MaxHealth) : 0f` | Her health over her maximum, 0 to 1. | QueenMantisBossBar |
| `Start` | Method (protected override) | `void` | `()` |  |  |
| `OnDisable` | Method (protected override) | `void` | `()` |  | QueenMantisBossBar, QueenMantisPart |
| `Update` | Method (protected override) | `void` | `()` |  | DevKeys |
| `TraversePath` | Method (protected override) | `void` | `()` |  |  |

Private members not listed: 6.

### QueenMantisData

`unity/Assets/_Game/Features/Enemies/Scripts/QueenMantis/QueenMantisData.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/QueenMantis/QueenMantisData.cs)) · ScriptableObject · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly

The numbers and art behind the Queen Mantis's mechanics: the pray pause, the white heal, the black raise, her summons and her looks. Data, not code (UNITY_RULES…

Derives from / implements: ScriptableObject

Used on: QueenMantisData

Named in: QueenMantisBoss, QueenMantisBuilder, QueenMantisPart, QueenMantisRaiser

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `PraySeconds` | `float` | `1f` | Shade changes | QueenMantisData = 1 | Seconds she stands still at each shade change. She can't heal or raise meanwhile: the opening to hit back. |
| `HealRadius` | `float` | `3f` | White: healing | QueenMantisData = 3 | Hurt enemies within this many world units of her are healed. Never herself. |
| `HealAmount` | `float` | `6f` | White: healing | QueenMantisData = 6 | Health given to each of them per heal. |
| `HealInterval` | `float` | `1f` | White: healing | QueenMantisData = 1 | Seconds between heals. A pulse ring shows each one. |
| `RaiseRadius` | `float` | `4f` | Black: raising husks | QueenMantisData = 4 | Bodies within this many world units of her can be raised. Their markers show inside this reach. |
| `HusksPerRaise` | `int` | `3` | Black: raising husks | QueenMantisData = 3 | At most this many husks per raise. |
| `RaiseInterval` | `float` | `4f` | Black: raising husks | QueenMantisData = 4 | Seconds between raises. |
| `RaiseWindUp` | `float` | `0.6f` | Black: raising husks | QueenMantisData = 0.6 | Seconds the chosen bodies glow before they rise. A shade change in this time stops the raise. |
| `HuskHealthFraction` | `float` | `0.5f` | Black: raising husks | QueenMantisData = 0.5 | A husk's health as a share of the Mantis Spawn prefab's; 0.5 is half. |
| `DeathMemorySeconds` | `float` | `12f` | Black: raising husks | QueenMantisData = 12 | A body older than this many seconds can no longer be raised; its marker fades out over this time. |
| `DeathLogCapacity` | `int` | `12` | Black: raising husks | QueenMantisData = 12 | How many recent deaths she remembers at most. |
| `SpawnPrefab` | `BasicEnemy` |  | Summons | QueenMantisData = Enemy_MantisSpawn | The Mantis Spawn prefab she summons, and raises husks as. |
| `SummonOnEntry` | `int` | `3` | Summons | QueenMantisData = 3 | Mantis Spawn she brings with her when she walks in. |
| `SummonOnShadeChange` | `int` | `2` | Summons | QueenMantisData = 2 | Mantis Spawn she calls at each shade change, at the start of the pray pause. |
| `SummonRingRadius` | `float` | `0.7f` | Summons | QueenMantisData = 0.7 | World units from her where summoned Mantis Spawn appear. |
| `EscortsKeepHerPace` | `bool` | `true` | Summons | QueenMantisData = 1 | If on, summoned Mantis Spawn walk at her speed, so they stay inside her heal and raise reach. |
| `WhiteSprite` | `Sprite` |  | Looks | QueenMantisData = QueenMantisTopDown_White | Her sprite in the white shade. |
| `BlackSprite` | `Sprite` |  | Looks | QueenMantisData = QueenMantisTopDown_Black | Her sprite in the black shade. |
| `ShiftFrames` | `Sprite[]` | `new Sprite[0]` | Looks | QueenMantisData = 3 item(s) | In-between frames from black to white, played forwards into white and backwards into black during the pray pause. |
| `TurnDegreesPerSecond` | `float` | `360f` | Looks | QueenMantisData = 360 | How fast her body turns to face the road, in degrees per second. |
| `RingSprite` | `Sprite` |  | Looks | QueenMantisData = Ring | A ring sprite for the heal pulse and the body markers. |
| `HealPulseColor` | `Color` | `new Color(0.95f, 1f, 0.8f, 0.8f)` | Looks | QueenMantisData = (0.95, 1, 0.8, 0.8) | The heal pulse at its start; it grows out to the heal radius and fades. |
| `MarkerColor` | `Color` | `new Color(0.35f, 0.15f, 0.5f, 0.85f)` | Looks | QueenMantisData = (0.35, 0.15, 0.5, 0.85) | A body she can raise; fades as the body gets older. |
| `WindUpColor` | `Color` | `new Color(0.75f, 0.45f, 1f, 1f)` | Looks | QueenMantisData = (0.75, 0.45, 1, 1) | A chosen body during the wind-up, just before it rises. |
| `HuskTint` | `Color` | `new Color(0.45f, 0.38f, 0.55f)` | Looks | QueenMantisData = (0.45, 0.38, 0.55, 1) | Sprite colour of a raised husk, so it reads as one. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `SpriteFor` | Method | `Sprite` | `(QueenMantisShade shade)` | Her sprite in shade. |  |

### QueenMantisHealAura

`unity/Assets/_Game/Features/Enemies/Scripts/QueenMantis/QueenMantisHealAura.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/QueenMantis/QueenMantisHealAura.cs)) · MonoBehaviour (boss part) · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly

The white shade: every HealInterval seconds she heals the hurt enemies within HealRadius (never herself, QueenMantisHealRule) with Jason's Heal, and a QueenMant…

Derives from / implements: QueenMantisPart

Used on: Enemy_QueenMantis

Named in: QueenMantisBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_pulse` | `QueenMantisPulse` |  |  | Enemy_QueenMantis = Enemy_QueenMantis/Heal Pulse (QueenMantisPulse) | The ring on her child object that grows out to the heal radius at each heal. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Healed` | Event | `Action<int>` |  | She healed; the number is how many enemies it reached (0 when none were near and hurt). |  |
| `OnShadeChanged` | Method (protected override) | `void` | `(QueenMantisShade shade)` |  |  |

Private members not listed: 3.

### QueenMantisLook

`unity/Assets/_Game/Features/Enemies/Scripts/QueenMantis/QueenMantisLook.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/QueenMantis/QueenMantisLook.cs)) · MonoBehaviour (boss part) · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly

How she looks: the White or Black top-down sprite for her shade, the Shift frames played through each pray pause (forwards into white, backwards into black), an…

Derives from / implements: QueenMantisPart

Used on: Enemy_QueenMantis

Named in: QueenMantisBuilder

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_body` | `SpriteRenderer` |  |  | Enemy_QueenMantis = Enemy_QueenMantis/Body (SpriteRenderer) | Her body sprite, on a child object. The sprite faces up (+y). |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `OnEnable` | Method (protected override) | `void` | `()` |  |  |
| `OnShadeChanged` | Method (protected override) | `void` | `(QueenMantisShade shade)` |  |  |

Private members not listed: 6.

### QueenMantisPart

`unity/Assets/_Game/Features/Enemies/Scripts/QueenMantis/QueenMantisPart.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/QueenMantis/QueenMantisPart.cs)) · MonoBehaviour · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly

Base for each piece of the Queen Mantis (heal aura, raiser, summoner, look, body markers): a small component beside QueenMantisBoss that reads her QueenMantisDa…

Derives from / implements: MonoBehaviour

Named in: QueenMantisBodyMarkers, QueenMantisHealAura, QueenMantisLook, QueenMantisRaiser, QueenMantisSummoner

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Boss` | Property (protected) | `QueenMantisBoss` | `{ get; private set; }` | The Queen Mantis this piece belongs to. |  |
| `Data` | Property (protected) | `QueenMantisData` | `=> Boss.Data` | Her numbers and art; null on a prefab the builder has not filled. |  |
| `Awake` | Method (protected virtual) | `void` | `()` |  | QueenMantisBodyMarkers, QueenMantisRaiser |
| `OnEnable` | Method (protected virtual) | `void` | `()` |  | QueenMantisLook, QueenMantisRaiser |
| `OnDisable` | Method (protected virtual) | `void` | `()` |  | QueenMantisRaiser |
| `OnEntered` | Method (protected virtual) | `void` | `()` | She has her road and is walking in. | QueenMantisSummoner |
| `OnShadeChanged` | Method (protected virtual) | `void` | `(QueenMantisShade shade)` | She stepped into a new third and turned shade; her pray pause starts now. | QueenMantisHealAura, QueenMantisLook, QueenMantisRaiser, QueenMantisSummoner |
| `OnPrayEnded` | Method (protected virtual) | `void` | `()` | Her pray pause is over and she walks and acts again. |  |
| `SpawnOnHerRoad` | Method (protected) | `BasicEnemy` | `(BasicEnemy prefab, Vector3 at, int waypointIndex)` | Puts a copy of prefab on her road at at, walking on towards waypoint waypointIndex, in her scene and under her parent. | QueenMantisRaiser, QueenMantisSummoner |

### QueenMantisPulse

`unity/Assets/_Game/Features/Enemies/Scripts/QueenMantis/QueenMantisPulse.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/QueenMantis/QueenMantisPulse.cs)) · MonoBehaviour · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly

The ring that shows a Queen Mantis heal: it grows from her out to the heal radius and fades. Sits on a child of the Queen Mantis with its own SpriteRenderer (a …

Derives from / implements: MonoBehaviour

Used on: Enemy_QueenMantis

Named in: QueenMantisBuilder, QueenMantisHealAura

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `_duration` | `float` | `0.5f` |  | Enemy_QueenMantis = 0.5 | Seconds one pulse takes to reach the heal radius and fade out. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Play` | Method | `void` | `(float radius, Color color)` | Starts a pulse out to radius world units, starting at color. | QueenMantisHealAura |

Private members not listed: 7.

### QueenMantisRaiser

`unity/Assets/_Game/Features/Enemies/Scripts/QueenMantis/QueenMantisRaiser.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/QueenMantis/QueenMantisRaiser.cs)) · MonoBehaviour (boss part) · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly

The black shade. She remembers where enemies fall (BossDeathLog), in either shade. In black, every RaiseInterval seconds she picks up to HusksPerRaise of the fr…

Derives from / implements: QueenMantisPart

Used on: Enemy_QueenMantis

Named in: QueenMantisBodyMarkers, QueenMantisBuilder

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `WindUpStarted` | Event | `Action<IReadOnlyList<DeathRecord>>` |  | The chosen bodies glow; they rise when the wind-up ends. |  |
| `HusksRaised` | Event | `Action<int>` |  | Husks rose; the number is how many. |  |
| `DeathLog` | Property | `BossDeathLog` | `=> _deathLog` | The deaths she remembers; the body markers read it. | QueenMantisBodyMarkers |
| `WindingUp` | Property | `IReadOnlyList<DeathRecord>` | `=> _windingUp` | The bodies chosen for the raise in progress, if any. | QueenMantisBodyMarkers |
| `WindUpProgress` | Property | `float` | `=> Data != null && Data.RaiseWindUp > 0f ? 1f - _windUpLeft / Data.RaiseWindUp : 1f` | How far through the wind-up she is, 0 to 1. | QueenMantisBodyMarkers |
| `Awake` | Method (protected override) | `void` | `()` |  | QueenMantisBodyMarkers |
| `OnEnable` | Method (protected override) | `void` | `()` |  |  |
| `OnDisable` | Method (protected override) | `void` | `()` |  |  |
| `OnShadeChanged` | Method (protected override) | `void` | `(QueenMantisShade shade)` |  |  |

Private members not listed: 10.

### QueenMantisSummoner

`unity/Assets/_Game/Features/Enemies/Scripts/QueenMantis/QueenMantisSummoner.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/QueenMantis/QueenMantisSummoner.cs)) · MonoBehaviour (boss part) · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly

Her escort: SummonOnEntry Mantis Spawn when she walks in and SummonOnShadeChange more at each shade change, on a ring around her (QueenMantisSummonRule). With E…

Derives from / implements: QueenMantisPart

Used on: Enemy_QueenMantis

Named in: QueenMantisBuilder

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Summoned` | Event | `Action<int>` |  | Mantis Spawn appeared beside her; the number is how many. |  |
| `OnEntered` | Method (protected override) | `void` | `()` |  |  |
| `OnShadeChanged` | Method (protected override) | `void` | `(QueenMantisShade shade)` |  |  |

Private members not listed: 1.

### EnemyRegistry

`unity/Assets/_Game/Features/Enemies/Scripts/Registry/EnemyRegistry.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/Registry/EnemyRegistry.cs)) · Static class · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly

Every living enemy that carries a RegisteredEnemy, so towers can look for targets without a scene search or physics queries. Enemies made outside the spawner (c…

Named in: BirdShadow, DiatomaceousEarth, LevelPowerUps, LevelRun, Mine, Projectile, QueenMantisHealAura, QueenMantisRaiser, RegisteredEnemy, SilverfishClickReader, SlowAura, Tower

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `All` | Property (static) | `IReadOnlyList<BasicEnemy>` | `=> s_all` | Enemies currently alive and enabled and still on the path, in registration order. One that has reached the Queen leaves at once, before its end-of-frame Destroy… | BirdShadow, DiatomaceousEarth, LevelRun, Mine, Projectile, QueenMantisHealAura, SilverfishClickReader, SlowAura, Tower |
| `Contains` | Method (static) | `bool` | `(BasicEnemy enemy)` | True while enemy is registered. False once it has died, reached the Queen or been disabled, so anything holding a reference can let it go. | Projectile |
| `EnemyDied` | Event (static) | `Action<BasicEnemy>` |  | A registered enemy was killed (not: reached the Queen). Raised by RegisteredEnemy while the enemy's transform is still valid, so the Queen Mantis can note where… | LevelPowerUps, QueenMantisRaiser |
| `Add` | Method (internal static) | `void` | `(BasicEnemy enemy)` |  | RegisteredEnemy |
| `Remove` | Method (internal static) | `void` | `(BasicEnemy enemy)` |  | RegisteredEnemy |
| `NotifyDied` | Method (internal static) | `void` | `(BasicEnemy enemy)` |  | RegisteredEnemy |

Private members not listed: 3.

### RegisteredEnemy

`unity/Assets/_Game/Features/Enemies/Scripts/Registry/RegisteredEnemy.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/Registry/RegisteredEnemy.cs)) · MonoBehaviour · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly

Puts this enemy in the EnemyRegistry while it is alive, so towers can target it, and tells the registry when it is killed. Goes on every enemy prefab next to Ba…

Derives from / implements: MonoBehaviour

Used on: Enemy_Basic, Enemy_Centipede, Enemy_QueenMantis, Enemy_RhinoBeetle, Enemy_RollyPolly, Enemy_Silverfish, Enemy_Wasp

Named in: AlphaBossesBuilder, AlphaCoreLoopBuilder, AlphaEnemiesBuilder, CentipedeSpeedUp, QueenMantisBuilder

Private members not listed: 6.

### BossDeathLog

`unity/Assets/_Game/Features/Enemies/Scripts/Rules/BossDeathLog.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/Rules/BossDeathLog.cs)) · Class · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly

The Queen Mantis's memory of recent deaths, as plain C# so EditMode tests can cover it. Only the last Capacity deaths are kept. Her own death and the deaths of …

Named in: BossDeathLogTests, QueenMantisBodyMarkers, QueenMantisRaiser

#### DeathRecord (struct)

Where and when an enemy fell, and which waypoint it was walking to.

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `DeathRecord` | Constructor |  | `(float x, float y, int waypointIndex, float time)` |  |  |
| `X` | Property | `float` | `{ get; }` |  |  |
| `Y` | Property | `float` | `{ get; }` |  |  |
| `WaypointIndex` | Property | `int` | `{ get; }` |  | BossDeathLogTests, QueenMantisRaiser |
| `Time` | Property | `float` | `{ get; }` |  | BossDeathLogTests |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `BossDeathLog` | Constructor |  | `(ulong ownerId, int capacity, float memorySeconds)` |  | BossDeathLogTests, QueenMantisRaiser |
| `OwnerId` | Property | `ulong` | `{ get; }` |  |  |
| `Capacity` | Property | `int` | `{ get; }` |  |  |
| `MemorySeconds` | Property | `float` | `{ get; }` |  |  |
| `Count` | Property | `int` | `=> _deaths.Count` | Deaths remembered right now, expired ones included until the next TakeNear. | BossDeathLogTests, QueenMantisBodyMarkers, QueenMantisRaiser |
| `this[]` | Indexer | `DeathRecord` | `=> _deaths[index]` | A remembered death, oldest first; for markers that show which bodies she can raise. |  |
| `Freshness` | Method | `float` | `(DeathRecord death, float now)` | How fresh a death is at now: 1 as it falls, 0 once it is older than the memory and can no longer be raised. A marker fades with it. | BossDeathLogTests, QueenMantisBodyMarkers |
| `MarkHusk` | Method | `void` | `(ulong enemyId)` | Marks an enemy as a husk this boss raised, so its death will not count. | BossDeathLogTests, QueenMantisRaiser |
| `Record` | Method | `bool` | `(ulong enemyId, float x, float y, int waypointIndex, float time)` | Notes a death. Returns false, and remembers nothing, for the boss itself and for a husk it raised (the husk is then forgotten too). | BossDeathLogTests, QueenMantisRaiser |
| `TakeNear` | Method | `int` | `(float x, float y, float radius, int maxCount, float now, List<DeathRecord> into)` | Moves up to maxCount deaths within radius of a point into into, newest first, and forgets them. Deaths older than the memory are dropped on the way. Returns how… | BossDeathLogTests, QueenMantisRaiser |

Private members not listed: 2.

### EnemyShield

`unity/Assets/_Game/Features/Enemies/Scripts/Rules/EnemyShield.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/Rules/EnemyShield.cs)) · Class · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly

The Rhino Beetle's shield, as plain C# so EditMode tests can cover it. Damage hits the shield first and only what is left reaches the enemy's health. Once the s…

Named in: EnemyShieldTests, RhinoBeetleEnemy

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `EnemyShield` | Constructor |  | `(double capacity, float regrowDelay, double regrowPerSecond)` |  | EnemyShieldTests, RhinoBeetleEnemy |
| `Capacity` | Property | `double` | `{ get; }` | How much damage a full shield soaks. |  |
| `RegrowDelay` | Property | `float` | `{ get; }` | Seconds without a hit before the shield starts to grow back. |  |
| `RegrowPerSecond` | Property | `double` | `{ get; }` | Shield points regained per second once regrowing. |  |
| `Current` | Property | `double` | `{ get; private set; }` | Shield points left right now. | EnemyShieldTests |
| `UntouchedFor` | Property | `float` | `{ get; private set; }` | Seconds since the last hit. | EnemyShieldTests |
| `IsUp` | Property | `bool` | `=> Current > 0.0` | True while any shield is left. | EnemyShieldTests |
| `Fraction` | Property | `float` | `=> Capacity > 0.0 ? (float)(Current / Capacity) : 0f` | Shield left as a share of the capacity, 0 to 1, for a visual. | EnemyShieldTests, RhinoBeetleEnemy |
| `Soak` | Method | `double` | `(double damage)` | Takes damage on the shield and returns the part that gets through to health. Zero or negative damage changes nothing. | EnemyShieldTests, RhinoBeetleEnemy |
| `Tick` | Method | `void` | `(float deltaTime)` | Advances time: counts the untouched seconds and regrows the shield once the delay has passed. Only the part of this tick that lies beyond the delay regrows anyt… | EnemyShieldTests, RhinoBeetleEnemy |

### QueenMantisHealRule

`unity/Assets/_Game/Features/Enemies/Scripts/Rules/QueenMantisHealRule.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/Rules/QueenMantisHealRule.cs)) · Static class · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly

Who her white heal reaches, as plain C# so EditMode tests can cover it.

Named in: QueenMantisBodyMarkers, QueenMantisHealAura, QueenMantisHealRuleTests

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `InReach` | Method (static) | `bool` | `(float dx, float dy, float radius)` | True when a point dx, dy away from her is within radius; the edge counts. | QueenMantisBodyMarkers, QueenMantisHealRuleTests |
| `ShouldHeal` | Method (static) | `bool` | `(bool isHer, bool isDead, double currentHealth, double maxHealth, float dx, float dy, float radius)` | True for a living, hurt enemy within reach. Never herself: she heals her escort, so focusing her down stays the answer. | QueenMantisHealAura, QueenMantisHealRuleTests |

### QueenMantisShade

`unity/Assets/_Game/Features/Enemies/Scripts/Rules/QueenMantisShade.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/Rules/QueenMantisShade.cs)) · Enum · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly

The Queen Mantis's two shades (GDD decision 98).

Named in: QueenMantisBoss, QueenMantisBossBar, QueenMantisData, QueenMantisHealAura, QueenMantisLook, QueenMantisPart, QueenMantisRaiser, QueenMantisShadeCycle, QueenMantisShadeTests, QueenMantisSummoner

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `White` | Enum member (static) | `QueenMantisShade` |  | Praying: she heals the enemies around her. | QueenMantisHealAura, QueenMantisLook, QueenMantisShadeTests |
| `Black` | Enum member (static) | `QueenMantisShade` |  | Necromancer: she raises the enemies killed near her as husks. | QueenMantisBossBar, QueenMantisData, QueenMantisLook, QueenMantisRaiser, QueenMantisShadeTests |

#### QueenMantisShadeRule (class)

Which shade each third of her health gives, as plain C# so EditMode tests can cover it. Decision 108: she changes shade each time she loses a third of her healt…

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `ThirdsLost` | Method (static) | `int` | `(double healthFraction)` | How many thirds of her health are gone: 0, 1 or 2. An exact third counts as gone. | QueenMantisShadeCycle, QueenMantisShadeTests |
| `ForThirdsLost` | Method (static) | `QueenMantisShade` | `(int thirdsLost)` | Her shade once thirdsLost thirds are gone: white, then black, then white again. | QueenMantisShadeCycle, QueenMantisShadeTests |

### QueenMantisShadeCycle

`unity/Assets/_Game/Features/Enemies/Scripts/Rules/QueenMantisShadeCycle.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/Rules/QueenMantisShadeCycle.cs)) · Class · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly

Her shade over one fight, as plain C# so EditMode tests can cover it. It only moves forward: healing never takes her back a third. It moves one third per TryAdv…

Named in: QueenMantisBoss, QueenMantisShadeTests

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `ThirdsLost` | Property | `int` | `{ get; private set; }` | Thirds she has changed shade for so far: 0, 1 or 2. | QueenMantisBoss, QueenMantisShadeTests |
| `Shade` | Property | `QueenMantisShade` | `=> QueenMantisShadeRule.ForThirdsLost(ThirdsLost)` | Her shade now. | QueenMantisBoss, QueenMantisShadeTests |
| `TryAdvance` | Method | `bool` | `(double healthFraction)` | Steps into the next third if her health says she has lost more than she has shown. Returns true when she stepped, which always changes her shade. | QueenMantisBoss, QueenMantisShadeTests |

### QueenMantisSummonRule

`unity/Assets/_Game/Features/Enemies/Scripts/Rules/QueenMantisSummonRule.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/Rules/QueenMantisSummonRule.cs)) · Static class · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly

Where her summoned Mantis Spawn appear, as plain C# so EditMode tests can cover it.

Named in: QueenMantisSummonRuleTests, QueenMantisSummoner

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `RingOffset` | Method (static) | `void` | `(int index, int count, float radius, out float x, out float y)` | The offset from her of escort index of count, spread evenly on a ring of radius. The first one stands straight above her. | QueenMantisSummonRuleTests, QueenMantisSummoner |

### RollyPollyCurl

`unity/Assets/_Game/Features/Enemies/Scripts/Rules/RollyPollyCurl.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/Rules/RollyPollyCurl.cs)) · Class · `HoldTheHill.Features.Enemies` · HoldTheHill.Runtime · owner: Nilly

The Rolly Polly's curl, as plain C# so EditMode tests can cover it. A hit on an uncurled Rolly Polly lands in full and makes it curl; while curled it takes only…

Named in: RollyPollyCurlTests, RollyPollyEnemy

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `RollyPollyCurl` | Constructor |  | `(int maxCurls, float rollDistance, double curledDamageShare, float curledSpeedMultiplier)` |  | RollyPollyCurlTests, RollyPollyEnemy |
| `MaxCurls` | Property | `int` | `{ get; }` | How many times it can curl in one life. | RollyPollyCurlTests |
| `RollDistance` | Property | `float` | `{ get; }` | Path distance rolled while curled before it uncurls, in world units (one tile is one unit). | RollyPollyCurlTests |
| `CurledDamageShare` | Property | `double` | `{ get; }` | The share of each hit that lands while curled; 0.4 means 40 percent. | RollyPollyCurlTests |
| `CurledSpeedMultiplier` | Property | `float` | `{ get; }` | Movement speed multiplier while curled. | RollyPollyCurlTests |
| `CurlsLeft` | Property | `int` | `{ get; private set; }` | Curls still available. | RollyPollyCurlTests |
| `IsCurled` | Property | `bool` | `{ get; private set; }` | True while curled up and rolling. | RollyPollyCurlTests, RollyPollyEnemy |
| `DistanceLeft` | Property | `float` | `{ get; private set; }` | Path distance still to roll before uncurling; zero when not curled. | RollyPollyCurlTests |
| `SpeedMultiplier` | Property | `float` | `=> IsCurled ? CurledSpeedMultiplier : 1f` | The speed multiplier to apply right now: the curled one while rolling, otherwise 1. | RollyPollyCurlTests, RollyPollyEnemy |
| `Hit` | Method | `double` | `(double damage)` | Takes a hit of damage and returns how much of it lands. The hit that makes it curl lands in full; hits while curled land at CurledDamageShare. | RollyPollyCurlTests, RollyPollyEnemy |
| `Advance` | Method | `bool` | `(float distance)` | Records distance of path rolled. Returns true on the call that finishes the roll and uncurls; false otherwise, including while not curled. | RollyPollyCurlTests, RollyPollyEnemy |

## Prefabs

### Enemy_Basic

`unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_Basic.prefab` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_Basic.prefab)) · Prefab

BasicEnemy: Baseline component providing core stats, health management, status effect handling, and waypoint traversal along a coordinate path. Implements IDama…

Made by: AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01)); AlphaEnemiesBuilder (Tools/Hold the Hill/Alpha/Build Enemies (six kinds, waves, Silve…

Used in: Enemy_MantisSpawn, Enemy_Scarab, Hill01_Waves, L00_TutorialHill_Waves, L01_Meadow_Waves

**Enemy_Basic/Body** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (1.3333334, 1.3333334, 1.3333334) |  |

**Enemy_Basic/Body** · SpriteRenderer

| Field | Value | Points to |
|---|---|---|
| `m_Sprite` | EnemyGrunt.png › EnemyGrunt Walk 0 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyGrunt.png |
| `m_Color` | (1, 1, 1, 1) |  |
| `m_SortingOrder` | 5 |  |
| `m_SortingLayer` | 0 |  |

**Enemy_Basic/Body** · SpriteFlipbook

| Field | Value | Points to |
|---|---|---|
| `_renderer` | Enemy_Basic/Body (SpriteRenderer) | Component: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_Basic.prefab |
| `_frames` | 6 item(s) |  |
| `_frames[0]` | EnemyGrunt.png › EnemyGrunt Walk 0 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyGrunt.png |
| `_frames[1]` | EnemyGrunt.png › EnemyGrunt Walk 1 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyGrunt.png |
| `_frames[2]` | EnemyGrunt.png › EnemyGrunt Walk 2 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyGrunt.png |
| `_frames[3]` | EnemyGrunt.png › EnemyGrunt Walk 3 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyGrunt.png |
| `_frames[4]` | EnemyGrunt.png › EnemyGrunt Walk 4 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyGrunt.png |
| `_frames[5]` | EnemyGrunt.png › EnemyGrunt Walk 5 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyGrunt.png |
| `_frameSeconds` | 0.09 |  |
| `_randomStart` | 1 |  |

**Enemy_Basic** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (0.6, 0.6, 0.6) |  |

**Enemy_Basic** · BasicEnemy

| Field | Value | Points to |
|---|---|---|
| `_maxHealth` | 30 |  |
| `_damage` | 10 |  |
| `_moveSpeed` | 1.5 |  |
| `_waypointTolerance` | 0.05 |  |
| `_pathCoordinates` | 0 item(s) |  |
| `_flipSpriteFacing` | 0 |  |

**Enemy_Basic** · Bounty

| Field | Value | Points to |
|---|---|---|
| `_honeydew` | 5 |  |

**Enemy_Basic** · FaceTravel

| Field | Value | Points to |
|---|---|---|
| `_body` | Enemy_Basic/Body (Transform) | Component: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_Basic.prefab |
| `_artFacingDegrees` | 0 |  |
| `_turnDegreesPerSecond` | 720 |  |
| `_teleportDistance` | 1 |  |

**Enemy_Basic** · EnemyHurtAndDeath

| Field | Value | Points to |
|---|---|---|
| `_body` | Enemy_Basic/Body (SpriteRenderer) | Component: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_Basic.prefab |
| `_walk` | Enemy_Basic/Body (SpriteFlipbook) | Component: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_Basic.prefab |
| `_hurtFrames` | 2 item(s) |  |
| `_hurtFrames[0]` | EnemyGrunt.png › EnemyGrunt Hurt 0 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyGrunt.png |
| `_hurtFrames[1]` | EnemyGrunt.png › EnemyGrunt Hurt 1 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyGrunt.png |
| `_hurtFrameSeconds` | 0.06 |  |
| `_deathFrames` | 7 item(s) |  |
| `_deathFrames[0]` | EnemyGrunt.png › EnemyGrunt Death 0 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyGrunt.png |
| `_deathFrames[1]` | EnemyGrunt.png › EnemyGrunt Death 1 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyGrunt.png |
| `_deathFrames[2]` | EnemyGrunt.png › EnemyGrunt Death 2 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyGrunt.png |
| `_deathFrames[3]` | EnemyGrunt.png › EnemyGrunt Death 3 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyGrunt.png |
| `_deathFrames[4]` | EnemyGrunt.png › EnemyGrunt Death 4 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyGrunt.png |
| `_deathFrames[5]` | EnemyGrunt.png › EnemyGrunt Death 5 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyGrunt.png |
| `_deathFrames[6]` | EnemyGrunt.png › EnemyGrunt Death 6 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyGrunt.png |
| `_deathFrameSeconds` | 0.08 |  |
| `_deathFadeSeconds` | 0.35 |  |

### Enemy_Centipede

`unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_Centipede.prefab` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_Centipede.prefab)) · Prefab

CentipedeEnemy: Specialized subclass of BasicEnemy representing a Centipede enemy. When defeated, it splits into 2 smaller enemies with half of the parent's max…

Made by: AlphaBossesBuilder (Tools/Hold the Hill/Alpha/Build Bosses (Centipede, Queen Mantis, last waves)); AlphaSpritesBuilder (Tools/Hold the Hill/Alpha/Build Sprites …

Used in: L00_TutorialHill_Waves, UI

**Enemy_Centipede** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (0.9, 0.9, 0.9) |  |

**Enemy_Centipede** · CentipedeEnemy

| Field | Value | Points to |
|---|---|---|
| `_maxHealth` | 60 |  |
| `_damage` | 20 |  |
| `_moveSpeed` | 1.1 |  |
| `_waypointTolerance` | 0.05 |  |
| `_pathCoordinates` | 0 item(s) |  |
| `_flipSpriteFacing` | 0 |  |
| `_splitsRemaining` | 2 |  |
| `_splitPrefab` | (none) |  |
| `_sizeMultiplier` | 0.8 |  |
| `_splitForwardDistance` | 0.6 |  |

**Enemy_Centipede** · Bounty

| Field | Value | Points to |
|---|---|---|
| `_honeydew` | 10 |  |

**Enemy_Centipede** · CentipedeSpeedUp

| Field | Value | Points to |
|---|---|---|
| `_childSpeedFactor` | 1.25 |  |

**Enemy_Centipede** · FaceTravel

| Field | Value | Points to |
|---|---|---|
| `_body` | Enemy_Centipede/Body (Transform) | Component: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_Centipede.prefab |
| `_artFacingDegrees` | 0 |  |
| `_turnDegreesPerSecond` | 720 |  |
| `_teleportDistance` | 1 |  |

**Enemy_Centipede** · EnemyHurtAndDeath

| Field | Value | Points to |
|---|---|---|
| `_body` | Enemy_Centipede/Body (SpriteRenderer) | Component: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_Centipede.prefab |
| `_walk` | Enemy_Centipede/Body (SpriteFlipbook) | Component: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_Centipede.prefab |
| `_hurtFrames` | 2 item(s) |  |
| `_hurtFrames[0]` | EnemySplitter.png › EnemySplitter Hurt 0 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySplitter.png |
| `_hurtFrames[1]` | EnemySplitter.png › EnemySplitter Hurt 1 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySplitter.png |
| `_hurtFrameSeconds` | 0.06 |  |
| `_deathFrames` | 7 item(s) |  |
| `_deathFrames[0]` | EnemySplitter.png › EnemySplitter Death 0 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySplitter.png |
| `_deathFrames[1]` | EnemySplitter.png › EnemySplitter Death 1 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySplitter.png |
| `_deathFrames[2]` | EnemySplitter.png › EnemySplitter Death 2 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySplitter.png |
| `_deathFrames[3]` | EnemySplitter.png › EnemySplitter Death 3 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySplitter.png |
| `_deathFrames[4]` | EnemySplitter.png › EnemySplitter Death 4 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySplitter.png |
| `_deathFrames[5]` | EnemySplitter.png › EnemySplitter Death 5 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySplitter.png |
| `_deathFrames[6]` | EnemySplitter.png › EnemySplitter Death 6 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySplitter.png |
| `_deathFrameSeconds` | 0.08 |  |
| `_deathFadeSeconds` | 0.35 |  |

**Enemy_Centipede/Body** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (1, 1, 1) |  |

**Enemy_Centipede/Body** · SpriteRenderer

| Field | Value | Points to |
|---|---|---|
| `m_Sprite` | EnemySplitter.png › EnemySplitter Walk 0 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySplitter.png |
| `m_Color` | (1, 1, 1, 1) |  |
| `m_SortingOrder` | 6 |  |
| `m_SortingLayer` | 0 |  |

**Enemy_Centipede/Body** · SpriteFlipbook

| Field | Value | Points to |
|---|---|---|
| `_renderer` | Enemy_Centipede/Body (SpriteRenderer) | Component: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_Centipede.prefab |
| `_frames` | 6 item(s) |  |
| `_frames[0]` | EnemySplitter.png › EnemySplitter Walk 0 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySplitter.png |
| `_frames[1]` | EnemySplitter.png › EnemySplitter Walk 1 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySplitter.png |
| `_frames[2]` | EnemySplitter.png › EnemySplitter Walk 2 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySplitter.png |
| `_frames[3]` | EnemySplitter.png › EnemySplitter Walk 3 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySplitter.png |
| `_frames[4]` | EnemySplitter.png › EnemySplitter Walk 4 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySplitter.png |
| `_frames[5]` | EnemySplitter.png › EnemySplitter Walk 5 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySplitter.png |
| `_frameSeconds` | 0.08 |  |
| `_randomStart` | 1 |  |

### Enemy_MantisSpawn

`unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_MantisSpawn.prefab` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_MantisSpawn.prefab)) · Prefab variant

Variant of Enemy_Basic. BasicEnemy: Baseline component providing core stats, health management, status effect handling, and waypoint traversal along a coordinat…

Made by: AlphaEnemiesBuilder (Tools/Hold the Hill/Alpha/Build Enemies (six kinds, waves, Silverfish click)); AlphaSpritesBuilder (Tools/Hold the Hill/Alpha/Build Sprites…

Variant of [Enemy_Basic](#enemy_basic): only the overrides are listed.

Used in: L01_Meadow_Waves, QueenMantisData

| Object | Component | Field | Override | Base value |
|---|---|---|---|---|
| Enemy_MantisSpawn | BasicEnemy | `_damage` | 5 | 10 |
| Enemy_MantisSpawn | BasicEnemy | `_maxHealth` | 12 | 30 |
| Enemy_MantisSpawn | BasicEnemy | `_moveSpeed` | 1.7 | 1.5 |
| Enemy_MantisSpawn | Transform | `m_LocalScale.x` | 0.45 | (0.6, 0.6, 0.6) |
| Enemy_MantisSpawn | Transform | `m_LocalScale.y` | 0.45 | (0.6, 0.6, 0.6) |
| Enemy_MantisSpawn | Transform | `m_LocalScale.z` | 0.45 | (0.6, 0.6, 0.6) |
| Enemy_MantisSpawn/Body | SpriteRenderer | `m_Sprite` | EnemyHealer.png › EnemyHealer Walk 0 | EnemyGrunt.png › EnemyGrunt Walk 0 |
| Enemy_MantisSpawn | EnemyHurtAndDeath | `_hurtFrames[0]` | EnemyHealer.png › EnemyHealer Hurt 0 | EnemyGrunt.png › EnemyGrunt Hurt 0 |
| Enemy_MantisSpawn | EnemyHurtAndDeath | `_hurtFrames[1]` | EnemyHealer.png › EnemyHealer Hurt 1 | EnemyGrunt.png › EnemyGrunt Hurt 1 |
| Enemy_MantisSpawn | EnemyHurtAndDeath | `_deathFrames[0]` | EnemyHealer.png › EnemyHealer Death 0 | EnemyGrunt.png › EnemyGrunt Death 0 |
| Enemy_MantisSpawn | EnemyHurtAndDeath | `_deathFrames[1]` | EnemyHealer.png › EnemyHealer Death 1 | EnemyGrunt.png › EnemyGrunt Death 1 |
| Enemy_MantisSpawn | EnemyHurtAndDeath | `_deathFrames[2]` | EnemyHealer.png › EnemyHealer Death 2 | EnemyGrunt.png › EnemyGrunt Death 2 |
| Enemy_MantisSpawn | EnemyHurtAndDeath | `_deathFrames[3]` | EnemyHealer.png › EnemyHealer Death 3 | EnemyGrunt.png › EnemyGrunt Death 3 |
| Enemy_MantisSpawn | EnemyHurtAndDeath | `_deathFrames[4]` | EnemyHealer.png › EnemyHealer Death 4 | EnemyGrunt.png › EnemyGrunt Death 4 |
| Enemy_MantisSpawn | EnemyHurtAndDeath | `_deathFrames[5]` | EnemyHealer.png › EnemyHealer Death 5 | EnemyGrunt.png › EnemyGrunt Death 5 |
| Enemy_MantisSpawn | EnemyHurtAndDeath | `_deathFrames[6]` | EnemyHealer.png › EnemyHealer Death 6 | EnemyGrunt.png › EnemyGrunt Death 6 |
| Enemy_MantisSpawn/Body | Transform | `m_LocalScale.x` | 1 | (1.3333334, 1.3333334, 1.3333334) |
| Enemy_MantisSpawn/Body | Transform | `m_LocalScale.y` | 1 | (1.3333334, 1.3333334, 1.3333334) |
| Enemy_MantisSpawn/Body | Transform | `m_LocalScale.z` | 1 | (1.3333334, 1.3333334, 1.3333334) |
| Enemy_MantisSpawn | GameObject | `m_Name` | Enemy_MantisSpawn |  |
| Enemy_MantisSpawn | Bounty | `_honeydew` | 2 | 5 |
| Enemy_MantisSpawn/Body | SpriteFlipbook | `_frameSeconds` | 0.1 | 0.09 |
| Enemy_MantisSpawn/Body | SpriteFlipbook | `_frames[0]` | EnemyHealer.png › EnemyHealer Walk 0 | EnemyGrunt.png › EnemyGrunt Walk 0 |
| Enemy_MantisSpawn/Body | SpriteFlipbook | `_frames[1]` | EnemyHealer.png › EnemyHealer Walk 1 | EnemyGrunt.png › EnemyGrunt Walk 1 |
| Enemy_MantisSpawn/Body | SpriteFlipbook | `_frames[2]` | EnemyHealer.png › EnemyHealer Walk 2 | EnemyGrunt.png › EnemyGrunt Walk 2 |
| Enemy_MantisSpawn/Body | SpriteFlipbook | `_frames[3]` | EnemyHealer.png › EnemyHealer Walk 3 | EnemyGrunt.png › EnemyGrunt Walk 3 |
| Enemy_MantisSpawn/Body | SpriteFlipbook | `_frames[4]` | EnemyHealer.png › EnemyHealer Walk 4 | EnemyGrunt.png › EnemyGrunt Walk 4 |
| Enemy_MantisSpawn/Body | SpriteFlipbook | `_frames[5]` | EnemyHealer.png › EnemyHealer Walk 5 | EnemyGrunt.png › EnemyGrunt Walk 5 |


### Enemy_QueenMantis

`unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_QueenMantis.prefab` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_QueenMantis.prefab)) · Prefab

QueenMantisPulse: The ring that shows a Queen Mantis heal: it grows from her out to the heal radius and fades. Sits on a child of the Queen Mantis with its own …

Made by: AlphaBossesBuilder (Tools/Hold the Hill/Alpha/Build Bosses (Centipede, Queen Mantis, last waves)); QueenMantisBuilder (Tools/Hold the Hill/Alpha/Build Queen Man…

Used in: L01_Meadow_Waves, UI

**Enemy_QueenMantis/Body** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (1, 1, 1) |  |

**Enemy_QueenMantis/Body** · SpriteRenderer

| Field | Value | Points to |
|---|---|---|
| `m_Sprite` | QueenMantisTopDown_White | Sprite: unity/Assets/_Game/Features/Enemies/Art/QueenMantis/QueenMantisTopDown_White.png |
| `m_Color` | (1, 1, 1, 1) |  |
| `m_SortingOrder` | 6 |  |
| `m_SortingLayer` | 0 |  |

**Enemy_QueenMantis/Heal Pulse** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (1, 1, 1) |  |

**Enemy_QueenMantis/Heal Pulse** · SpriteRenderer

| Field | Value | Points to |
|---|---|---|
| `m_Sprite` | Ring | Sprite: unity/Assets/_Game/Shared/Art/Placeholders/Ring.png |
| `m_Color` | (1, 1, 1, 1) |  |
| `m_SortingOrder` | 3 |  |
| `m_SortingLayer` | 0 |  |

**Enemy_QueenMantis/Heal Pulse** · QueenMantisPulse

| Field | Value | Points to |
|---|---|---|
| `_duration` | 0.5 |  |

**Enemy_QueenMantis** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (1, 1, 1) |  |

**Enemy_QueenMantis** · QueenMantisBoss

| Field | Value | Points to |
|---|---|---|
| `_maxHealth` | 300 |  |
| `_damage` | 40 |  |
| `_moveSpeed` | 0.8 |  |
| `_waypointTolerance` | 0.05 |  |
| `_pathCoordinates` | 0 item(s) |  |
| `_flipSpriteFacing` | 0 |  |
| `_data` | QueenMantisData | Data asset: unity/Assets/_Game/Features/Enemies/Data/QueenMantisData.asset |

**Enemy_QueenMantis** · Bounty

| Field | Value | Points to |
|---|---|---|
| `_honeydew` | 80 |  |

**Enemy_QueenMantis** · QueenMantisHealAura

| Field | Value | Points to |
|---|---|---|
| `_pulse` | Enemy_QueenMantis/Heal Pulse (QueenMantisPulse) | Component: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_QueenMantis.prefab |

**Enemy_QueenMantis** · QueenMantisBodyMarkers

| Field | Value | Points to |
|---|---|---|
| `_markerSize` | 0.5 |  |
| `_windUpSize` | 1 |  |
| `_sortingOrder` | 1 |  |

**Enemy_QueenMantis** · QueenMantisLook

| Field | Value | Points to |
|---|---|---|
| `_body` | Enemy_QueenMantis/Body (SpriteRenderer) | Component: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_QueenMantis.prefab |

### Enemy_RhinoBeetle

`unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_RhinoBeetle.prefab` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_RhinoBeetle.prefab)) · Prefab

RhinoBeetleEnemy: The Rhino Beetle: a BasicEnemy behind an EnemyShield. Damage hits the shield first; what gets through reaches its health. The shield grows bac…

Made by: AlphaEnemiesBuilder (Tools/Hold the Hill/Alpha/Build Enemies (six kinds, waves, Silverfish click)); AlphaSpritesBuilder (Tools/Hold the Hill/Alpha/Build Sprites…

Used in: L00_TutorialHill_Waves, L01_Meadow_Waves

**Enemy_RhinoBeetle/Shield** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (1.35, 1.35, 1.35) |  |

**Enemy_RhinoBeetle/Shield** · SpriteRenderer

| Field | Value | Points to |
|---|---|---|
| `m_Sprite` | Circle | Sprite: unity/Assets/_Game/Shared/Art/Placeholders/Circle.png |
| `m_Color` | (0.55, 0.8, 1, 0.55) |  |
| `m_SortingOrder` | 6 |  |
| `m_SortingLayer` | 0 |  |

**Enemy_RhinoBeetle** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (0.8, 0.8, 0.8) |  |

**Enemy_RhinoBeetle** · RhinoBeetleEnemy

| Field | Value | Points to |
|---|---|---|
| `_maxHealth` | 60 |  |
| `_damage` | 15 |  |
| `_moveSpeed` | 1.1 |  |
| `_waypointTolerance` | 0.05 |  |
| `_pathCoordinates` | 0 item(s) |  |
| `_flipSpriteFacing` | 0 |  |
| `_shieldCapacity` | 40 |  |
| `_shieldRegrowDelay` | 3 |  |
| `_shieldRegrowPerSecond` | 25 |  |
| `_shieldVisual` | Enemy_RhinoBeetle/Shield (SpriteRenderer) | Component: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_RhinoBeetle.prefab |

**Enemy_RhinoBeetle** · Bounty

| Field | Value | Points to |
|---|---|---|
| `_honeydew` | 12 |  |

**Enemy_RhinoBeetle** · FaceTravel

| Field | Value | Points to |
|---|---|---|
| `_body` | Enemy_RhinoBeetle/Body (Transform) | Component: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_RhinoBeetle.prefab |
| `_artFacingDegrees` | 0 |  |
| `_turnDegreesPerSecond` | 720 |  |
| `_teleportDistance` | 1 |  |

**Enemy_RhinoBeetle** · EnemyHurtAndDeath

| Field | Value | Points to |
|---|---|---|
| `_body` | Enemy_RhinoBeetle/Body (SpriteRenderer) | Component: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_RhinoBeetle.prefab |
| `_walk` | Enemy_RhinoBeetle/Body (SpriteFlipbook) | Component: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_RhinoBeetle.prefab |
| `_hurtFrames` | 2 item(s) |  |
| `_hurtFrames[0]` | EnemyShielded.png › EnemyShielded Hurt 0 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyShielded.png |
| `_hurtFrames[1]` | EnemyShielded.png › EnemyShielded Hurt 1 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyShielded.png |
| `_hurtFrameSeconds` | 0.06 |  |
| `_deathFrames` | 7 item(s) |  |
| `_deathFrames[0]` | EnemyShielded.png › EnemyShielded Death 0 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyShielded.png |
| `_deathFrames[1]` | EnemyShielded.png › EnemyShielded Death 1 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyShielded.png |
| `_deathFrames[2]` | EnemyShielded.png › EnemyShielded Death 2 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyShielded.png |
| `_deathFrames[3]` | EnemyShielded.png › EnemyShielded Death 3 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyShielded.png |
| `_deathFrames[4]` | EnemyShielded.png › EnemyShielded Death 4 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyShielded.png |
| `_deathFrames[5]` | EnemyShielded.png › EnemyShielded Death 5 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyShielded.png |
| `_deathFrames[6]` | EnemyShielded.png › EnemyShielded Death 6 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyShielded.png |
| `_deathFrameSeconds` | 0.08 |  |
| `_deathFadeSeconds` | 0.35 |  |

**Enemy_RhinoBeetle/Body** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (1, 1, 1) |  |

**Enemy_RhinoBeetle/Body** · SpriteRenderer

| Field | Value | Points to |
|---|---|---|
| `m_Sprite` | EnemyShielded.png › EnemyShielded WalkBare 0 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyShielded.png |
| `m_Color` | (1, 1, 1, 1) |  |
| `m_SortingOrder` | 5 |  |
| `m_SortingLayer` | 0 |  |

**Enemy_RhinoBeetle/Body** · SpriteFlipbook

| Field | Value | Points to |
|---|---|---|
| `_renderer` | Enemy_RhinoBeetle/Body (SpriteRenderer) | Component: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_RhinoBeetle.prefab |
| `_frames` | 6 item(s) |  |
| `_frames[0]` | EnemyShielded.png › EnemyShielded WalkBare 0 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyShielded.png |
| `_frames[1]` | EnemyShielded.png › EnemyShielded WalkBare 1 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyShielded.png |
| `_frames[2]` | EnemyShielded.png › EnemyShielded WalkBare 2 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyShielded.png |
| `_frames[3]` | EnemyShielded.png › EnemyShielded WalkBare 3 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyShielded.png |
| `_frames[4]` | EnemyShielded.png › EnemyShielded WalkBare 4 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyShielded.png |
| `_frames[5]` | EnemyShielded.png › EnemyShielded WalkBare 5 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyShielded.png |
| `_frameSeconds` | 0.1 |  |
| `_randomStart` | 1 |  |

### Enemy_RollyPolly

`unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_RollyPolly.prefab` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_RollyPolly.prefab)) · Prefab

RollyPollyEnemy: The Rolly Polly: a BasicEnemy that curls up when hit (see RollyPollyCurl). Curled, it takes only a share of each hit and rolls faster for a few…

Made by: AlphaEnemiesBuilder (Tools/Hold the Hill/Alpha/Build Enemies (six kinds, waves, Silverfish click)); AlphaSpritesBuilder (Tools/Hold the Hill/Alpha/Build Sprites…

Used in: L01_Meadow_Waves

**Enemy_RollyPolly/Body** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (2, 2, 2) |  |

**Enemy_RollyPolly/Body** · SpriteRenderer

| Field | Value | Points to |
|---|---|---|
| `m_Sprite` | CritterPillbug.png › CritterPillbug Walk 0 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/CritterPillbug.png |
| `m_Color` | (1, 1, 1, 1) |  |
| `m_SortingOrder` | 5 |  |
| `m_SortingLayer` | 0 |  |

**Enemy_RollyPolly/Body** · SpriteFlipbook

| Field | Value | Points to |
|---|---|---|
| `_renderer` | Enemy_RollyPolly/Body (SpriteRenderer) | Component: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_RollyPolly.prefab |
| `_frames` | 4 item(s) |  |
| `_frames[0]` | CritterPillbug.png › CritterPillbug Walk 0 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/CritterPillbug.png |
| `_frames[1]` | CritterPillbug.png › CritterPillbug Walk 1 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/CritterPillbug.png |
| `_frames[2]` | CritterPillbug.png › CritterPillbug Walk 2 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/CritterPillbug.png |
| `_frames[3]` | CritterPillbug.png › CritterPillbug Walk 3 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/CritterPillbug.png |
| `_frameSeconds` | 0.11 |  |
| `_randomStart` | 1 |  |

**Enemy_RollyPolly** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (0.6, 0.6, 0.6) |  |

**Enemy_RollyPolly** · RollyPollyEnemy

| Field | Value | Points to |
|---|---|---|
| `_maxHealth` | 45 |  |
| `_damage` | 10 |  |
| `_moveSpeed` | 1.3 |  |
| `_waypointTolerance` | 0.05 |  |
| `_pathCoordinates` | 0 item(s) |  |
| `_flipSpriteFacing` | 0 |  |
| `_maxCurls` | 3 |  |
| `_rollDistance` | 3 |  |
| `_curledDamageShare` | 0.4 |  |
| `_curledSpeedMultiplier` | 2 |  |
| `_curledScale` | 0.75 |  |
| `_curledTint` | (0.55, 0.55, 0.65, 1) |  |

**Enemy_RollyPolly** · Bounty

| Field | Value | Points to |
|---|---|---|
| `_honeydew` | 8 |  |

**Enemy_RollyPolly** · FaceTravel

| Field | Value | Points to |
|---|---|---|
| `_body` | Enemy_RollyPolly/Body (Transform) | Component: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_RollyPolly.prefab |
| `_artFacingDegrees` | 0 |  |
| `_turnDegreesPerSecond` | 720 |  |
| `_teleportDistance` | 1 |  |

### Enemy_Scarab

`unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_Scarab.prefab` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_Scarab.prefab)) · Prefab variant

Variant of Enemy_Basic. BasicEnemy: Baseline component providing core stats, health management, status effect handling, and waypoint traversal along a coordinat…

Made by: AlphaEnemiesBuilder (Tools/Hold the Hill/Alpha/Build Enemies (six kinds, waves, Silverfish click)); AlphaSpritesBuilder (Tools/Hold the Hill/Alpha/Build Sprites…

Variant of [Enemy_Basic](#enemy_basic): only the overrides are listed.

Used in: L00_TutorialHill_Waves, L01_Meadow_Waves

| Object | Component | Field | Override | Base value |
|---|---|---|---|---|
| Enemy_Scarab | BasicEnemy | `_damage` | 5 | 10 |
| Enemy_Scarab | BasicEnemy | `_maxHealth` | 10 | 30 |
| Enemy_Scarab | BasicEnemy | `_moveSpeed` | 2.8 | 1.5 |
| Enemy_Scarab | Transform | `m_LocalScale.x` | 0.35 | (0.6, 0.6, 0.6) |
| Enemy_Scarab | Transform | `m_LocalScale.y` | 0.35 | (0.6, 0.6, 0.6) |
| Enemy_Scarab | Transform | `m_LocalScale.z` | 0.35 | (0.6, 0.6, 0.6) |
| Enemy_Scarab/Body | SpriteRenderer | `m_Sprite` | EnemyRunner.png › EnemyRunner Walk 0 | EnemyGrunt.png › EnemyGrunt Walk 0 |
| Enemy_Scarab | EnemyHurtAndDeath | `_hurtFrames[0]` | EnemyRunner.png › EnemyRunner Hurt 0 | EnemyGrunt.png › EnemyGrunt Hurt 0 |
| Enemy_Scarab | EnemyHurtAndDeath | `_hurtFrames[1]` | EnemyRunner.png › EnemyRunner Hurt 1 | EnemyGrunt.png › EnemyGrunt Hurt 1 |
| Enemy_Scarab | EnemyHurtAndDeath | `_deathFrames[0]` | EnemyRunner.png › EnemyRunner Death 0 | EnemyGrunt.png › EnemyGrunt Death 0 |
| Enemy_Scarab | EnemyHurtAndDeath | `_deathFrames[1]` | EnemyRunner.png › EnemyRunner Death 1 | EnemyGrunt.png › EnemyGrunt Death 1 |
| Enemy_Scarab | EnemyHurtAndDeath | `_deathFrames[2]` | EnemyRunner.png › EnemyRunner Death 2 | EnemyGrunt.png › EnemyGrunt Death 2 |
| Enemy_Scarab | EnemyHurtAndDeath | `_deathFrames[3]` | EnemyRunner.png › EnemyRunner Death 3 | EnemyGrunt.png › EnemyGrunt Death 3 |
| Enemy_Scarab | EnemyHurtAndDeath | `_deathFrames[4]` | EnemyRunner.png › EnemyRunner Death 4 | EnemyGrunt.png › EnemyGrunt Death 4 |
| Enemy_Scarab | EnemyHurtAndDeath | `_deathFrames[5]` | EnemyRunner.png › EnemyRunner Death 5 | EnemyGrunt.png › EnemyGrunt Death 5 |
| Enemy_Scarab | EnemyHurtAndDeath | `_deathFrames[6]` | EnemyRunner.png › EnemyRunner Death 6 | EnemyGrunt.png › EnemyGrunt Death 6 |
| Enemy_Scarab | GameObject | `m_Name` | Enemy_Scarab |  |
| Enemy_Scarab | Bounty | `_honeydew` | 2 | 5 |
| Enemy_Scarab/Body | SpriteFlipbook | `_frameSeconds` | 0.06 | 0.09 |
| Enemy_Scarab/Body | SpriteFlipbook | `_frames[0]` | EnemyRunner.png › EnemyRunner Walk 0 | EnemyGrunt.png › EnemyGrunt Walk 0 |
| Enemy_Scarab/Body | SpriteFlipbook | `_frames[1]` | EnemyRunner.png › EnemyRunner Walk 1 | EnemyGrunt.png › EnemyGrunt Walk 1 |
| Enemy_Scarab/Body | SpriteFlipbook | `_frames[2]` | EnemyRunner.png › EnemyRunner Walk 2 | EnemyGrunt.png › EnemyGrunt Walk 2 |
| Enemy_Scarab/Body | SpriteFlipbook | `_frames[3]` | EnemyRunner.png › EnemyRunner Walk 3 | EnemyGrunt.png › EnemyGrunt Walk 3 |
| Enemy_Scarab/Body | SpriteFlipbook | `_frames[4]` | EnemyRunner.png › EnemyRunner Walk 4 | EnemyGrunt.png › EnemyGrunt Walk 4 |
| Enemy_Scarab/Body | SpriteFlipbook | `_frames[5]` | EnemyRunner.png › EnemyRunner Walk 5 | EnemyGrunt.png › EnemyGrunt Walk 5 |


### Enemy_Silverfish

`unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_Silverfish.prefab` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_Silverfish.prefab)) · Prefab

SilverfishEnemy: A subclass of BasicEnemy that represents a Silverfish. The Silverfish enemy can be instantly defeated when clicked by the player.

Made by: AlphaEnemiesBuilder (Tools/Hold the Hill/Alpha/Build Enemies (six kinds, waves, Silverfish click)); AlphaSpritesBuilder (Tools/Hold the Hill/Alpha/Build Sprites…

Used in: L00_TutorialHill_Waves, L01_Meadow_Waves

**Enemy_Silverfish/Body** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (1.3333334, 1.3333334, 1.3333334) |  |

**Enemy_Silverfish/Body** · SpriteRenderer

| Field | Value | Points to |
|---|---|---|
| `m_Sprite` | EnemySilverfish.png › EnemySilverfish Walk 0 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySilverfish.png |
| `m_Color` | (1, 1, 1, 1) |  |
| `m_SortingOrder` | 5 |  |
| `m_SortingLayer` | 0 |  |

**Enemy_Silverfish/Body** · SpriteFlipbook

| Field | Value | Points to |
|---|---|---|
| `_renderer` | Enemy_Silverfish/Body (SpriteRenderer) | Component: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_Silverfish.prefab |
| `_frames` | 6 item(s) |  |
| `_frames[0]` | EnemySilverfish.png › EnemySilverfish Walk 0 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySilverfish.png |
| `_frames[1]` | EnemySilverfish.png › EnemySilverfish Walk 1 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySilverfish.png |
| `_frames[2]` | EnemySilverfish.png › EnemySilverfish Walk 2 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySilverfish.png |
| `_frames[3]` | EnemySilverfish.png › EnemySilverfish Walk 3 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySilverfish.png |
| `_frames[4]` | EnemySilverfish.png › EnemySilverfish Walk 4 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySilverfish.png |
| `_frames[5]` | EnemySilverfish.png › EnemySilverfish Walk 5 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySilverfish.png |
| `_frameSeconds` | 0.04 |  |
| `_randomStart` | 1 |  |

**Enemy_Silverfish** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (0.5, 0.5, 0.5) |  |

**Enemy_Silverfish** · SilverfishEnemy

| Field | Value | Points to |
|---|---|---|
| `_maxHealth` | 25 |  |
| `_damage` | 10 |  |
| `_moveSpeed` | 1.8 |  |
| `_waypointTolerance` | 0.05 |  |
| `_pathCoordinates` | 0 item(s) |  |
| `_flipSpriteFacing` | 0 |  |

**Enemy_Silverfish** · Bounty

| Field | Value | Points to |
|---|---|---|
| `_honeydew` | 6 |  |

**Enemy_Silverfish** · FaceTravel

| Field | Value | Points to |
|---|---|---|
| `_body` | Enemy_Silverfish/Body (Transform) | Component: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_Silverfish.prefab |
| `_artFacingDegrees` | 0 |  |
| `_turnDegreesPerSecond` | 720 |  |
| `_teleportDistance` | 1 |  |

**Enemy_Silverfish** · EnemyHurtAndDeath

| Field | Value | Points to |
|---|---|---|
| `_body` | Enemy_Silverfish/Body (SpriteRenderer) | Component: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_Silverfish.prefab |
| `_walk` | Enemy_Silverfish/Body (SpriteFlipbook) | Component: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_Silverfish.prefab |
| `_hurtFrames` | 2 item(s) |  |
| `_hurtFrames[0]` | EnemySilverfish.png › EnemySilverfish Hurt 0 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySilverfish.png |
| `_hurtFrames[1]` | EnemySilverfish.png › EnemySilverfish Hurt 1 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySilverfish.png |
| `_hurtFrameSeconds` | 0.06 |  |
| `_deathFrames` | 7 item(s) |  |
| `_deathFrames[0]` | EnemySilverfish.png › EnemySilverfish Death 0 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySilverfish.png |
| `_deathFrames[1]` | EnemySilverfish.png › EnemySilverfish Death 1 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySilverfish.png |
| `_deathFrames[2]` | EnemySilverfish.png › EnemySilverfish Death 2 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySilverfish.png |
| `_deathFrames[3]` | EnemySilverfish.png › EnemySilverfish Death 3 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySilverfish.png |
| `_deathFrames[4]` | EnemySilverfish.png › EnemySilverfish Death 4 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySilverfish.png |
| `_deathFrames[5]` | EnemySilverfish.png › EnemySilverfish Death 5 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySilverfish.png |
| `_deathFrames[6]` | EnemySilverfish.png › EnemySilverfish Death 6 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemySilverfish.png |
| `_deathFrameSeconds` | 0.08 |  |
| `_deathFadeSeconds` | 0.35 |  |

### Enemy_Wasp

`unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_Wasp.prefab` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_Wasp.prefab)) · Prefab

WaspEnemy: The Wasp: a BasicEnemy that flies, so melee and lobbed towers cannot reach it (see IFlyingEnemy). Its stingers (hazard 2 in the GDD) come with the ha…

Made by: AlphaEnemiesBuilder (Tools/Hold the Hill/Alpha/Build Enemies (six kinds, waves, Silverfish click)); AlphaHazardsBuilder (Tools/Hold the Hill/Alpha/Build Hazards…

Used in: L00_TutorialHill_Waves, L01_Meadow_Waves

**Enemy_Wasp/Body** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (1, 1, 1) |  |

**Enemy_Wasp/Body** · SpriteRenderer

| Field | Value | Points to |
|---|---|---|
| `m_Sprite` | EnemyWasp.png › EnemyWasp Walk 0 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyWasp.png |
| `m_Color` | (1, 1, 1, 1) |  |
| `m_SortingOrder` | 7 |  |
| `m_SortingLayer` | 0 |  |

**Enemy_Wasp/Body** · SpriteFlipbook

| Field | Value | Points to |
|---|---|---|
| `_renderer` | Enemy_Wasp/Body (SpriteRenderer) | Component: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_Wasp.prefab |
| `_frames` | 6 item(s) |  |
| `_frames[0]` | EnemyWasp.png › EnemyWasp Walk 0 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyWasp.png |
| `_frames[1]` | EnemyWasp.png › EnemyWasp Walk 1 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyWasp.png |
| `_frames[2]` | EnemyWasp.png › EnemyWasp Walk 2 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyWasp.png |
| `_frames[3]` | EnemyWasp.png › EnemyWasp Walk 3 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyWasp.png |
| `_frames[4]` | EnemyWasp.png › EnemyWasp Walk 4 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyWasp.png |
| `_frames[5]` | EnemyWasp.png › EnemyWasp Walk 5 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyWasp.png |
| `_frameSeconds` | 0.05 |  |
| `_randomStart` | 1 |  |

**Enemy_Wasp** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (0, 0, 0) |  |
| `m_LocalScale` | (0.5, 0.5, 0.5) |  |

**Enemy_Wasp** · WaspEnemy

| Field | Value | Points to |
|---|---|---|
| `_maxHealth` | 24 |  |
| `_damage` | 10 |  |
| `_moveSpeed` | 2.2 |  |
| `_waypointTolerance` | 0.05 |  |
| `_pathCoordinates` | 0 item(s) |  |
| `_flipSpriteFacing` | 0 |  |
| `_isFlying` | 1 |  |

**Enemy_Wasp** · Bounty

| Field | Value | Points to |
|---|---|---|
| `_honeydew` | 8 |  |

**Enemy_Wasp** · WaspStingerDropper

| Field | Value | Points to |
|---|---|---|
| `_stingerPrefab` | Hazard_WaspStinger › WaspStinger | Component: unity/Assets/_Game/Features/Hazards/Prefabs/Hazard_WaspStinger.prefab |
| `_firstDropDelay` | 2 |  |
| `_interval` | 3.5 |  |

**Enemy_Wasp** · FaceTravel

| Field | Value | Points to |
|---|---|---|
| `_body` | Enemy_Wasp/Body (Transform) | Component: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_Wasp.prefab |
| `_artFacingDegrees` | 0 |  |
| `_turnDegreesPerSecond` | 720 |  |
| `_teleportDistance` | 1 |  |

**Enemy_Wasp** · EnemyHurtAndDeath

| Field | Value | Points to |
|---|---|---|
| `_body` | Enemy_Wasp/Body (SpriteRenderer) | Component: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_Wasp.prefab |
| `_walk` | Enemy_Wasp/Body (SpriteFlipbook) | Component: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_Wasp.prefab |
| `_hurtFrames` | 2 item(s) |  |
| `_hurtFrames[0]` | EnemyWasp.png › EnemyWasp Hurt 0 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyWasp.png |
| `_hurtFrames[1]` | EnemyWasp.png › EnemyWasp Hurt 1 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyWasp.png |
| `_hurtFrameSeconds` | 0.06 |  |
| `_deathFrames` | 7 item(s) |  |
| `_deathFrames[0]` | EnemyWasp.png › EnemyWasp Death 0 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyWasp.png |
| `_deathFrames[1]` | EnemyWasp.png › EnemyWasp Death 1 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyWasp.png |
| `_deathFrames[2]` | EnemyWasp.png › EnemyWasp Death 2 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyWasp.png |
| `_deathFrames[3]` | EnemyWasp.png › EnemyWasp Death 3 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyWasp.png |
| `_deathFrames[4]` | EnemyWasp.png › EnemyWasp Death 4 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyWasp.png |
| `_deathFrames[5]` | EnemyWasp.png › EnemyWasp Death 5 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyWasp.png |
| `_deathFrames[6]` | EnemyWasp.png › EnemyWasp Death 6 | Sprite: unity/Assets/_Game/Features/Enemies/Art/Sheets/EnemyWasp.png |
| `_deathFrameSeconds` | 0.08 |  |
| `_deathFadeSeconds` | 0.35 |  |

### Triangle

`unity/Assets/_Game/Features/Enemies/Scripts/EnemySpawner/Triangle.prefab` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/EnemySpawner/Triangle.prefab)) · Prefab

BasicEnemy: Baseline component providing core stats, health management, status effect handling, and waypoint traversal along a coordinate path. Implements IDama…

Made by: Hand-made (no builder names it)

Used in: Map_01

**Triangle** · Transform

| Field | Value | Points to |
|---|---|---|
| `m_LocalPosition` | (-8.61, -0.23, 0) |  |
| `m_LocalScale` | (1, 1, 1) |  |

**Triangle** · SpriteRenderer

| Field | Value | Points to |
|---|---|---|
| `m_Sprite` | (outside the repo: 75f5f34d) | Unknown |
| `m_Color` | (1, 1, 1, 1) |  |
| `m_SortingOrder` | 0 |  |
| `m_SortingLayer` | 0 |  |

**Triangle** · BasicEnemy

| Field | Value | Points to |
|---|---|---|
| `_maxHealth` | 1 |  |
| `_damage` | 1 |  |
| `_moveSpeed` | 2 |  |
| `_waypointTolerance` | 0.05 |  |
| `_pathCoordinates` | 0 item(s) |  |
| `_flipSpriteFacing` | 1 |  |

## Data assets

### Hill01_Waves

`unity/Assets/_Game/Features/Enemies/Data/Hill01_Waves.asset` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Data/Hill01_Waves.asset)) · Data asset

Type: `MapWaveDataSO`: ScriptableObject asset to configure and store map wave configurations in the Project. Create via: Assets &gt; Create &gt; Tower Defense &gt; Map Wave Config
Made by: AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01))

Used by: Hill01_Gameplay

Map id `Hill01`, 3 waves.

| # | Wave | Enemies | Spawns (prefab × count @ delay before next) |
|---|---|---|---|
| 1 | Wave 1 | 5 | Enemy_Basic ×5 @1.2s |
| 2 | Wave 2 | 8 | Enemy_Basic ×8 @0.9s |
| 3 | Wave 3 | 12 | Enemy_Basic ×12 @0.6s |

### L00_TutorialHill_Waves

`unity/Assets/_Game/Features/Enemies/Data/L00_TutorialHill_Waves.asset` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Data/L00_TutorialHill_Waves.asset)) · Data asset

Type: `MapWaveDataSO`: ScriptableObject asset to configure and store map wave configurations in the Project. Create via: Assets &gt; Create &gt; Tower Defense &gt; Map Wave Config
Made by: AlphaCoreLoopBuilder (Tools/Hold the Hill/Alpha/Build Core Loop (Hill01))

Used by: L00_TutorialHill_Gameplay

Map id `L00_TutorialHill`, 5 waves.

| # | Wave | Enemies | Spawns (prefab × count @ delay before next) |
|---|---|---|---|
| 1 | Wave 1 | 4 | Enemy_Basic ×4 @1.4s |
| 2 | Wave 2 | 11 | Enemy_Basic ×3 @1.2s, Enemy_Scarab ×8 @0.35s |
| 3 | Wave 3 | 6 | Enemy_RhinoBeetle ×2 @2s, Enemy_Basic ×4 @1s |
| 4 | Wave 4 | 13 | Enemy_Wasp ×3 @1.5s, Enemy_Basic ×4 @1s, Enemy_Scarab ×6 @0.35s |
| 5 | Wave 5 | 13 | Enemy_Silverfish ×3 @1.2s, Enemy_RhinoBeetle ×2 @1.5s, Enemy_Wasp ×2 @1.2s, Enemy_Basic ×4 @0.8s, Enemy_Basic ×1 @3s, Enemy_Centipede ×1 @0s |

### L01_Meadow_Waves

`unity/Assets/_Game/Features/Enemies/Data/L01_Meadow_Waves.asset` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Data/L01_Meadow_Waves.asset)) · Data asset

Type: `MapWaveDataSO`: ScriptableObject asset to configure and store map wave configurations in the Project. Create via: Assets &gt; Create &gt; Tower Defense &gt; Map Wave Config
Made by: QueenMantisBuilder (Tools/Hold the Hill/Alpha/Build Queen Mantis (art, data, prefab, wave, boss bar))

Used by: L01_Meadow_Gameplay

Map id `L01_Meadow`, 10 waves.

| # | Wave | Enemies | Spawns (prefab × count @ delay before next) |
|---|---|---|---|
| 1 | Wave 1 | 5 | Enemy_Basic ×5 @1.2s |
| 2 | Wave 2 | 13 | Enemy_Scarab ×10 @0.35s, Enemy_Basic ×3 @1s |
| 3 | Wave 3 | 7 | Enemy_RhinoBeetle ×3 @1.6s, Enemy_Basic ×4 @1s |
| 4 | Wave 4 | 8 | Enemy_Wasp ×4 @1.2s, Enemy_Basic ×4 @1s |
| 5 | Wave 5 | 7 | Enemy_RollyPolly ×3 @1.8s, Enemy_Basic ×4 @1s |
| 6 | Wave 6 | 12 | Enemy_Silverfish ×4 @1s, Enemy_Scarab ×8 @0.35s |
| 7 | Wave 7 | 12 | Enemy_MantisSpawn ×8 @0.6s, Enemy_Basic ×4 @1s |
| 8 | Wave 8 | 14 | Enemy_RhinoBeetle ×3 @1.4s, Enemy_Wasp ×4 @1s, Enemy_RollyPolly ×2 @1.5s, Enemy_Basic ×5 @0.8s |
| 9 | Wave 9 | 23 | Enemy_Scarab ×12 @0.3s, Enemy_Silverfish ×4 @1s, Enemy_Wasp ×4 @1s, Enemy_RhinoBeetle ×3 @1.2s |
| 10 | Wave 10 | 36 | Enemy_RollyPolly ×4 @1.2s, Enemy_RhinoBeetle ×4 @1.2s, Enemy_QueenMantis ×1 @2s, Enemy_Wasp ×5 @0.9s, Enemy_MantisSpawn ×6 @0.5s, Enemy_Basic ×6 @0.7s, Enemy_Sc… |

### QueenMantisData

`unity/Assets/_Game/Features/Enemies/Data/QueenMantisData.asset` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Data/QueenMantisData.asset)) · Data asset

Type: `QueenMantisData`: The numbers and art behind the Queen Mantis's mechanics: the pray pause, the white heal, the black raise, her summons and her looks. Data, not code (UNITY_RULES…
Made by: QueenMantisBuilder (Tools/Hold the Hill/Alpha/Build Queen Mantis (art, data, prefab, wave, boss bar))

Used by: Enemy_QueenMantis

**QueenMantisData**

| Field | Value | Points to |
|---|---|---|
| `PraySeconds` | 1 |  |
| `HealRadius` | 3 |  |
| `HealAmount` | 6 |  |
| `HealInterval` | 1 |  |
| `RaiseRadius` | 4 |  |
| `HusksPerRaise` | 3 |  |
| `RaiseInterval` | 4 |  |
| `RaiseWindUp` | 0.6 |  |
| `HuskHealthFraction` | 0.5 |  |
| `DeathMemorySeconds` | 12 |  |
| `DeathLogCapacity` | 12 |  |
| `SpawnPrefab` | Enemy_MantisSpawn | Prefab: unity/Assets/_Game/Features/Enemies/Prefabs/Enemy_MantisSpawn.prefab |
| `SummonOnEntry` | 3 |  |
| `SummonOnShadeChange` | 2 |  |
| `SummonRingRadius` | 0.7 |  |
| `EscortsKeepHerPace` | 1 |  |
| `WhiteSprite` | QueenMantisTopDown_White | Sprite: unity/Assets/_Game/Features/Enemies/Art/QueenMantis/QueenMantisTopDown_White.png |
| `BlackSprite` | QueenMantisTopDown_Black | Sprite: unity/Assets/_Game/Features/Enemies/Art/QueenMantis/QueenMantisTopDown_Black.png |
| `ShiftFrames` | 3 item(s) |  |
| `ShiftFrames[0]` | QueenMantisTopDown_Shift1 | Sprite: unity/Assets/_Game/Features/Enemies/Art/QueenMantis/QueenMantisTopDown_Shift1.png |
| `ShiftFrames[1]` | QueenMantisTopDown_Shift2 | Sprite: unity/Assets/_Game/Features/Enemies/Art/QueenMantis/QueenMantisTopDown_Shift2.png |
| `ShiftFrames[2]` | QueenMantisTopDown_Shift3 | Sprite: unity/Assets/_Game/Features/Enemies/Art/QueenMantis/QueenMantisTopDown_Shift3.png |
| `TurnDegreesPerSecond` | 360 |  |
| `RingSprite` | Ring | Sprite: unity/Assets/_Game/Shared/Art/Placeholders/Ring.png |
| `HealPulseColor` | (0.95, 1, 0.8, 0.8) |  |
| `MarkerColor` | (0.35, 0.15, 0.5, 0.85) |  |
| `WindUpColor` | (0.75, 0.45, 1, 1) |  |
| `HuskTint` | (0.45, 0.38, 0.55, 1) |  |

### Map_01

`unity/Assets/_Game/Features/Enemies/Scripts/EnemySpawner/Map_01.asset` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Enemies/Scripts/EnemySpawner/Map_01.asset)) · Data asset

Type: `MapWaveDataSO`: ScriptableObject asset to configure and store map wave configurations in the Project. Create via: Assets &gt; Create &gt; Tower Defense &gt; Map Wave Config
Made by: Hand-made (no builder names it)

Used by: Path and Queen Test

Map id `Map_01`, 1 waves.

| # | Wave | Enemies | Spawns (prefab × count @ delay before next) |
|---|---|---|---|
| 1 | Wave 1 | 1 | Triangle ×1 @0s |
