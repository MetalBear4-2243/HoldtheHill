# Combat

Part of the [code bank](README.md), built 2026-11-05 from commit `d6944f7b`.

## Scripts

| Script | Kind | Owner | What it does |
|---|---|---|---|
| [CombatUtil](#combatutil) | Static class | Jason | Shared queries, splash calculations, and life-check utilities for towers, weapons, and damageables. Uses static non-allocating buffers to avoid garbage collecti… |
| [DamageInfo](#damageinfo) | Struct | Jason | One instance of damage travelling from a source to a target. Struct so hits cost no allocation during heavy waves. |
| [IDamageable](#idamageable) | Interface | Jason | Contract for anything a tower, projectile or hazard can hurt or affect. |
| [IHealthReadable](#ihealthreadable) | Interface | Jason | An IDamageable that exposes its current and maximum health values. Used by towers for Strongest and Weakest priority targeting modes. |
| [IOwned](#iowned) | Interface | Jason | Contract for objects that act on behalf of another entity (e.g., projectiles, hazards left by towers). Damage dealt is credited to Owner rather than the loose o… |
| [StatusEffectData](#statuseffectdata) | Struct | Jason | A damage-over-time and/or slow that a hit or area hazard can leave behind. Serializable so designers can configure it on projectile and weapon prefabs. |

### CombatUtil

`unity/Assets/_Game/Features/Combat/Scripts/CombatUtil.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Combat/Scripts/CombatUtil.cs)) · Static class · `HoldTheHill.Features.Combat` · HoldTheHill.Runtime · owner: Jason

Shared queries, splash calculations, and life-check utilities for towers, weapons, and damageables. Uses static non-allocating buffers to avoid garbage collecti…

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `OverlapBufferSize` | Const (private) | `int` |  |  |  |
| `OverlapDamageables` | Method (static) | `void` | `(Vector2 center, float radius, LayerMask mask, List<IDamageable> results)` | Fills results with live IDamageable targets whose collider overlaps the circle. Clears results first, returns immediately if radius &lt;= 0, and deduplicates targe… |  |
| `NearestDamageable` | Method (static) | `IDamageable` | `(Vector2 from, float radius, LayerMask mask, List<IDamageable> buffer)` | Returns the live IDamageable nearest to a point within a radius, or null if none are found. |  |
| `IsAlive` | Method (static) | `bool` | `(IDamageable target)` | Checks whether a target exists, is not destroyed in Unity, is alive (not dead), and has an active Transform. Casts to UnityEngine.Object to avoid fake null refe… |  |
| `TryGetDamageable` | Method (static) | `bool` | `(Collider2D collider, out IDamageable damageable)` | Finds the live IDamageable owning a collider (checking the collider and its parents), if any. |  |
| `ApplySplash` | Method (static) | `void` | `(Vector2 center, float radius, LayerMask mask, float damage, DamageType type, GameObject source, StatusEffectData status, IDamageable ignore = null)` | Damages everything in a circle with DamageInfo and applies status when valid. Skips the ignore target (usually the direct-hit target). Handles nested splash cal… |  |
| `OwnerOf` | Method (static) | `GameObject` | `(GameObject source)` | Returns who should be credited for damage from source: the IOwned.Owner when present, otherwise the source itself. |  |
| `SqrDistance` | Method (static) | `float` | `(Transform a, Transform b)` | Calculates squared distance in 2D between two transforms. Returns PositiveInfinity if either is null. |  |

Private members not listed: 8.

### DamageInfo

`unity/Assets/_Game/Features/Combat/Scripts/DamageInfo.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Combat/Scripts/DamageInfo.cs)) · Struct · `HoldTheHill.Features.Combat` · HoldTheHill.Runtime · owner: Jason

One instance of damage travelling from a source to a target. Struct so hits cost no allocation during heavy waves.

Named in: BasicEnemy, BasicEnemyTests, CombatUtil, EnemyHurtAndDeath, IDamageable

#### DamageType (enum)

How a hit should be treated. Resistances and defense mechanics key off this.

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Physical` | Enum member (static) | `DamageType` |  |  | BasicEnemyTests |
| `Magic` | Enum member (static) | `DamageType` |  |  |  |
| `True` | Enum member (static) | `DamageType` |  |  |  |
| `Fire` | Enum member (static) | `DamageType` |  |  |  |
| `Poison` | Enum member (static) | `DamageType` |  |  | BasicEnemy |
| `Lightning` | Enum member (static) | `DamageType` |  |  |  |

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `Amount` | `float` |  |  |  |  |
| `Source` | `GameObject` |  |  |  |  |
| `HitPoint` | `Vector2` |  |  |  |  |
| `Type` | `DamageType` |  |  |  |  |
| `IsCritical` | `bool` |  |  |  |  |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `DamageInfo` | Constructor |  | `(float amount, GameObject source, Vector2 hitPoint, DamageType type = DamageType.Physical)` |  | BasicEnemy, BasicEnemyTests, CombatUtil |

### IDamageable

`unity/Assets/_Game/Features/Combat/Scripts/IDamageable.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Combat/Scripts/IDamageable.cs)) · Interface · `HoldTheHill.Features.Combat` · HoldTheHill.Runtime · owner: Jason

Contract for anything a tower, projectile or hazard can hurt or affect.

Named in: BasicEnemy, BasicEnemyTests, CombatUtil

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `TakeDamage` | Method | `void` | `(DamageInfo info)` | Apply one hit to this target. | BasicEnemy, BasicEnemyTests, CombatUtil, RhinoBeetleEnemy, RollyPollyEnemy |
| `ApplyStatusEffect` | Method | `void` | `(StatusEffectData status)` | Apply a damage-over-time or slow status effect. Re-applying refreshes the effect. | BasicEnemy, BasicEnemyTests, CombatUtil |
| `IsDead` | Property | `bool` | `{ get; }` | True once health has run out or death sequence has started. Targeting queries skip dead targets. | BasicEnemyTests, CombatUtil |
| `Transform` | Property | `Transform` | `{ get; }` | Transform of this target, used for aiming, proximity, and distance checks. | BasicEnemyTests, CombatUtil |

### IHealthReadable

`unity/Assets/_Game/Features/Combat/Scripts/IHealthReadable.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Combat/Scripts/IHealthReadable.cs)) · Interface · `HoldTheHill.Features.Combat` · HoldTheHill.Runtime · owner: Jason

An IDamageable that exposes its current and maximum health values. Used by towers for Strongest and Weakest priority targeting modes.

Named in: BasicEnemy, BasicEnemyTests

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `CurrentHealth` | Property | `float` | `{ get; }` | Current health remaining. | BasicEnemy, BasicEnemyTests |
| `MaxHealth` | Property | `float` | `{ get; }` | Maximum health pool. | BasicEnemy, BasicEnemyTests |

### IOwned

`unity/Assets/_Game/Features/Combat/Scripts/IOwned.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Combat/Scripts/IOwned.cs)) · Interface · `HoldTheHill.Features.Combat` · HoldTheHill.Runtime · owner: Jason

Contract for objects that act on behalf of another entity (e.g., projectiles, hazards left by towers). Damage dealt is credited to Owner rather than the loose o…

Named in: CombatUtil

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `Owner` | Property | `GameObject` | `{ get; }` | The GameObject to credit for actions performed by this object. | CombatUtil |

### StatusEffectData

`unity/Assets/_Game/Features/Combat/Scripts/StatusEffectData.cs` ([GitHub](https://github.com/MetalBear4-2243/HoldtheHill/blob/Indev/unity/Assets/_Game/Features/Combat/Scripts/StatusEffectData.cs)) · Struct · `HoldTheHill.Features.Combat` · HoldTheHill.Runtime · owner: Jason

A damage-over-time and/or slow that a hit or area hazard can leave behind. Serializable so designers can configure it on projectile and weapon prefabs.

Named in: BasicEnemy, BasicEnemyTests, CombatUtil, IDamageable, SlowAura

**Fields**

| Field | Type | Default | Header | Set by | Tooltip / summary |
|---|---|---|---|---|---|
| `EffectName` | `string` |  |  |  | Identifies the effect. Re-applying the same name refreshes it instead of stacking. |
| `DamagePerTick` | `float` |  |  |  | Damage dealt each tick. Zero for a pure slow. |
| `TickInterval` | `float` |  |  |  | Seconds between ticks. Values at or below zero default to 1 second. |
| `Duration` | `float` |  |  |  | Total seconds the effect lasts. Zero or less means this effect does nothing. |
| `SlowMultiplier` | `float` |  |  |  | Movement speed multiplier while active. 1 = no slow, 0.5 = half speed. |

**API**

| Member | Kind | Type | Signature | Summary | Used by |
|---|---|---|---|---|---|
| `IsValid` | Property | `bool` | `=> Duration > 0f && (DamagePerTick > 0f \|\| SlowMultiplier < 1f)` | True if this effect would actually do something: Duration &gt; 0 and (DamagePerTick &gt; 0 or SlowMultiplier &lt; 1). | BasicEnemy, CombatUtil |
| `SafeTickInterval` | Property | `float` | `=> TickInterval > 0.01f ? TickInterval : 1f` | Tick interval clamped to something safe to divide by: TickInterval when above 0.01, otherwise 1. | BasicEnemy |
| `None` | Property (static) | `StatusEffectData` | `=> new StatusEffectData { SlowMultiplier = 1f` | A no-op effect with SlowMultiplier 1. Use for "this weapon applies nothing". |  |
