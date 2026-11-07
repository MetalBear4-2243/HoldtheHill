using System;
using System.Collections.Generic;
using HoldTheHill.Features.Enemies;
using UnityEngine;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// A placed tower. Knows its grid cell, picks a target in range by its
    /// <see cref="TargetingPriority"/>, and attacks it on a cooldown: with a projectile when
    /// it has one, otherwise with a melee swing that hits every ground enemy in reach (or, with a
    /// cone set, every one in the cone toward the target: the Fire Ant's flames). Its numbers
    /// (range, damage, interval, cone, ability strength, prices) are the level table of its
    /// <see cref="TowerData"/>; <see cref="Upgrade"/> moves to the next row. The definition is
    /// shared and never written at run time: what changes during combat (the level, the cooldown,
    /// the condition, the priority) lives on this instance.
    /// </summary>
    /// <remarks>
    /// Towers that do something else in range (slow, lay mines) carry an <see cref="ITowerAbility"/>
    /// component that reads <see cref="Range"/>, <see cref="Damage"/> and <see cref="Ability"/>; such a
    /// tower, or one with no damage and no projectile, does not attack on its own.
    /// </remarks>
    public class Tower : MonoBehaviour
    {
        [Header("Definition")]
        [Tooltip("The data asset with this tower's levels: price, damage, interval, range and ability per level. Its Prefab is this tower.")]
        [SerializeField] private TowerData _data;

        [Header("Attack")]
        [Tooltip("Shot fired at the target. Leave empty for a melee tower that hits directly.")]
        [SerializeField] private Projectile _projectilePrefab;

        [Tooltip("Where shots leave the tower. Defaults to the tower itself.")]
        [SerializeField] private Transform _muzzle;

        [Header("Targeting")]
        [Tooltip("Which enemy to attack when several are in range.")]
        [SerializeField] private TargetingPriority _priority = TargetingPriority.First;

        [Header("Upgrades")]
        [Tooltip("How much bigger the tower is drawn with each level, so an upgrade is visible.")]
        [SerializeField, Min(1f)] private float _scalePerLevel = 1.12f;

        [Header("Condition")]
        [Tooltip("How the sprite is tinted while burrowed: sunk into the ground, out of the fight.")]
        [SerializeField] private Color _burrowedTint = new Color(0.5f, 0.45f, 0.4f, 0.6f);

        [Tooltip("How the sprite is tinted while stunned by a hazard.")]
        [SerializeField] private Color _stunnedTint = new Color(0.55f, 0.7f, 1f, 1f);

        [Tooltip("How the sprite is tinted while a hazard makes the tower miss.")]
        [SerializeField] private Color _missingTint = new Color(1f, 0.85f, 0.45f, 1f);

        private readonly List<TargetPicker.Candidate> _candidates = new List<TargetPicker.Candidate>();
        private readonly List<BasicEnemy> _inRange = new List<BasicEnemy>();
        private readonly TowerCondition _condition = new TowerCondition();
        private SpriteRenderer _sprite;
        private TowerStunRing _stunRing;
        private Color _baseColor = Color.white;
        private float _cooldown;
        private bool _hasAbility;

        // The row of the level table this instance is on, copied once per level so the shared
        // definition is read, never written. Until Awake (a prefab asset), the row is read on demand.
        private TowerLevel _current;
        private bool _hasCurrent;

        /// <summary>Raised after an upgrade, with the new level. The card and visuals listen.</summary>
        public event Action<int> Upgraded;

        /// <summary>Raised when the tower is burrowed or brought back up, with the new state.</summary>
        public event Action<bool> BurrowChanged;

        /// <summary>Raised just before a hazard removes the tower, so the placer can free its cell.</summary>
        public event Action<Tower> Removed;

        /// <summary>
        /// Raised the moment the tower attacks: a melee swing lands or a shot leaves. The attack
        /// animation lines its hit frame up with it.
        /// </summary>
        public event Action Attacked;

        /// <summary>
        /// Raised for each enemy a melee swing hits with real damage, before <see cref="Attacked"/>.
        /// The Crazy Ant's poison counts hits in a row with it.
        /// </summary>
        public event Action<BasicEnemy> HitEnemy;

        /// <summary>
        /// Raised when a tower that works through an ability uses it: the Carpenter Ant lays a mine,
        /// the Sap Ant's aura slows someone. The attack animation plays once for it.
        /// </summary>
        public event Action AbilityUsed;

        /// <summary>The data asset this tower takes its numbers from; null for a tower made without one (tests).</summary>
        public TowerData Definition => _data;

        /// <summary>The numbers at the current level: the definition's row, or <see cref="TowerLevel.Default"/> without one.</summary>
        public TowerLevel Current => _hasCurrent ? _current : LevelAt(Level);

        /// <summary>How far this tower can reach, in world units.</summary>
        public float Range => Current.Range;

        /// <summary>Damage dealt per hit.</summary>
        public float Damage => Current.Damage;

        /// <summary>Seconds between attacks: the level's attack interval, before any tempo.</summary>
        public float FireInterval => Current.AttackInterval;

        /// <summary>Melee only: how wide the swing is, in degrees, pointed at the target. 0 is all round.</summary>
        public float ConeDegrees => Current.Ability.ConeDegrees;

        /// <summary>The ability strength at the current level; the mine layer, the poison and the shots read it.</summary>
        public TowerAbilitySettings Ability => Current.Ability;

        /// <summary>Seconds until the tower may attack again; zero when it is ready.</summary>
        public float CooldownRemaining => Mathf.Max(0f, _cooldown);

        /// <summary>Which grid cell this tower occupies.</summary>
        public Vector2Int Cell { get; set; }

        /// <summary>The shop entry this tower was bought from, for its name on the card. Set by the placer.</summary>
        public TowerData ShopData { get; set; }

        /// <summary>Every Honeydew paid for this tower so far: the build price plus each upgrade. The placer keeps it.</summary>
        public int HoneydewSpent { get; set; }

        /// <summary>Current level, from 1.</summary>
        public int Level { get; private set; } = 1;

        /// <summary>The last level this tower can reach: the definition's row count, or 1 without one.</summary>
        public int MaxLevel => _data != null && _data.LevelCount > 0 ? _data.LevelCount : 1;

        /// <summary>Honeydew for the next level, or 0 at the last level.</summary>
        public int NextUpgradeCost => Level < MaxLevel ? LevelAt(Level + 1).Cost : 0;

        /// <summary>Which enemy this tower prefers to attack. Set from the tower card.</summary>
        public TargetingPriority Priority
        {
            get => _priority;
            set => _priority = value;
        }

        /// <summary>The enemy being attacked this frame, or null.</summary>
        public BasicEnemy CurrentTarget { get; private set; }

        /// <summary>What is being done to this tower and what it may still do.</summary>
        public TowerCondition Condition => _condition;

        /// <summary>
        /// The burrow order. A burrowed tower does not act and is safe from the bird and the
        /// pesticide. Set from the tower card or the B key.
        /// </summary>
        public bool Burrowed
        {
            get => _condition.Burrowed;
            set
            {
                if (_condition.Burrowed == value)
                {
                    return;
                }

                _condition.Burrowed = value;
                CurrentTarget = null;
                RefreshTint();
                BurrowChanged?.Invoke(value);
            }
        }

        /// <summary>True when the tower may aim, fire, slow or lay mines. Companions check this.</summary>
        public bool CanAct => _condition.CanAct;

        public bool IsStunned => _condition.IsStunned;

        public bool IsMissing => _condition.IsMissing;

        /// <summary>
        /// False for a melee or lobbed tower: it cannot reach a flying enemy (the wasp rule).
        /// Follows from how the tower attacks, so no prefab needs a flag.
        /// </summary>
        public bool ReachesFlying => TargetPicker.CanReach(
            enemyFlying: true, hasProjectile: _projectilePrefab != null,
            lobbed: _projectilePrefab != null && _projectilePrefab.IsLobbed);

        /// <summary>
        /// True when the tower picks its own targets, so its standing order (First / Last / Closest)
        /// matters. False for the Sap Ant's aura and the Carpenter Ant's mines. The card reads it.
        /// </summary>
        public bool UsesTargeting => Attacks;

        private bool Attacks => AttacksOnItsOwn(_hasAbility, Damage, _projectilePrefab != null);

        /// <summary>
        /// The attack rule: a tower with an <see cref="ITowerAbility"/> (a mine layer or a slow aura) does
        /// the work through it and only lends it its numbers; one with no damage and no projectile has nothing to hit with.
        /// </summary>
        public static bool AttacksOnItsOwn(bool hasAbility, float damage, bool hasProjectile)
        {
            return !hasAbility && (damage > 0f || hasProjectile);
        }

        // The definition's row for a level (from 1), or the default row for a tower with no definition.
        private TowerLevel LevelAt(int level)
        {
            return _data != null && _data.LevelCount > 0 ? _data.LevelAt(level) : TowerLevel.Default;
        }

        private void ApplyLevel(int level)
        {
            _current = LevelAt(level);
            _hasCurrent = true;
        }

        private void Awake()
        {
            if (_muzzle == null)
            {
                _muzzle = transform;
            }

            if (_data == null)
            {
                Debug.LogWarning($"{name} has no Tower Data, so it uses the default numbers.", this);
            }

            ApplyLevel(Level);
            _hasAbility = GetComponent<ITowerAbility>() != null;

            _sprite = GetComponentInChildren<SpriteRenderer>();
            if (_sprite != null)
            {
                _baseColor = _sprite.color;
            }
        }

        private void OnEnable()
        {
            TowerRegistry.Add(this);
        }

        private void OnDisable()
        {
            TowerRegistry.Remove(this);
        }

        private void Update()
        {
            if (_condition.Tick(Time.deltaTime))
            {
                RefreshTint();
            }

            _cooldown -= Time.deltaTime;

            // The Sap Ant and the Carpenter Ant work through their ability, so they never look for a
            // target of their own; nor does a tower that is stunned or burrowed.
            if (!_condition.CanAct || !Attacks)
            {
                CurrentTarget = null;
                return;
            }

            CurrentTarget = AcquireTarget();
            if (CurrentTarget == null || _cooldown > 0f)
            {
                return;
            }

            // The tower attacks (Swarm Frenzy's tempo applies here; MineLayer applies it to mines).
            _cooldown = TowerTempo.NextCooldown(_cooldown, FireInterval, Time.deltaTime, TowerTempo.FireRateFactor);
            Attack(CurrentTarget);
        }

        /// <summary>A hazard stuns the tower for <paramref name="seconds"/>: it does nothing meanwhile.</summary>
        public void Stun(float seconds)
        {
            _condition.Stun(seconds);
            RefreshTint();

            // Made on the first stun, so no tower prefab has to carry one.
            if (_stunRing == null)
            {
                _stunRing = TowerStunRing.Attach(this, _sprite, _stunnedTint);
            }
        }

        /// <summary>A hazard makes the tower miss for <paramref name="seconds"/>: it fires, to no effect.</summary>
        public void Miss(float seconds)
        {
            _condition.Miss(seconds);
            RefreshTint();
        }

        /// <summary>A hazard takes the tower off the map. Tells the placer first, then destroys it.</summary>
        public void Remove()
        {
            Removed?.Invoke(this);
            Destroy(gameObject);
        }

        // One tint at a time, worst first, so every state is visible and not by colour alone:
        // a burrowed tower is also faded, a stunned one stops moving its target line.
        private void RefreshTint()
        {
            if (_sprite == null)
            {
                return;
            }

            Color tint = _condition.Burrowed ? _burrowedTint
                : _condition.IsStunned ? _stunnedTint
                : _condition.IsMissing ? _missingTint
                : Color.white;
            _sprite.color = _baseColor * tint;
        }

        /// <summary>
        /// Moves to the next level and takes its row of the definition. False at the last level.
        /// The caller (the placer) has already checked the price and paid.
        /// </summary>
        public bool Upgrade()
        {
            if (Level >= MaxLevel)
            {
                return false;
            }

            Level++;
            ApplyLevel(Level);
            transform.localScale *= _scalePerLevel;

            Upgraded?.Invoke(Level);
            return true;
        }

        /// <summary>Picks the best living enemy in range for this tower's priority, or null.</summary>
        public BasicEnemy AcquireTarget()
        {
            _inRange.Clear();
            _candidates.Clear();

            Vector3 here = transform.position;
            float range = Range;
            float rangeSqr = range * range;
            bool reachesFlying = ReachesFlying;
            IReadOnlyList<BasicEnemy> enemies = EnemyRegistry.All;

            for (int i = 0; i < enemies.Count; i++)
            {
                BasicEnemy enemy = enemies[i];
                if (enemy == null || enemy.IsDead)
                {
                    continue;
                }

                // The wasp rule: a melee or lobbed tower does not even aim at a flyer.
                if (!reachesFlying && enemy is IFlyingEnemy flyer && flyer.IsFlying)
                {
                    continue;
                }

                Vector3 position = enemy.transform.position;
                float sqrDistance = (position - here).sqrMagnitude;
                if (sqrDistance > rangeSqr)
                {
                    continue;
                }

                _inRange.Add(enemy);
                _candidates.Add(new TargetPicker.Candidate
                {
                    PathProgress = TargetPicker.PathProgress(enemy.PathCoordinates, enemy.CurrentWaypointIndex, position),
                    SqrDistance = sqrDistance,
                });
            }

            int best = TargetPicker.Pick(_candidates, _priority);
            return best >= 0 ? _inRange[best] : null;
        }

        private void Attack(BasicEnemy target)
        {
            // A missing tower goes through the motions; its hits carry no damage.
            float damage = _condition.HitsLand ? Damage : 0f;

            if (_projectilePrefab == null)
            {
                // A melee swing hits every ground enemy in reach (or in the cone toward the target), not
                // just the target. AcquireTarget filled the list this frame and left flyers out (the wasp
                // rule). A hit can kill and split a centipede; the list is a copy, so that is safe.
                Vector3 here = transform.position;
                Vector3 aim = target.transform.position - here;
                float coneDegrees = ConeDegrees;
                for (int i = 0; i < _inRange.Count; i++)
                {
                    BasicEnemy enemy = _inRange[i];
                    if (enemy == null || enemy.IsDead || !InCone(here, aim, enemy.transform.position, coneDegrees))
                    {
                        continue;
                    }

                    enemy.TakeDamage(damage);
                    if (damage > 0f)
                    {
                        HitEnemy?.Invoke(enemy);
                    }
                }

                Attacked?.Invoke();
                return;
            }

            Projectile shot = Instantiate(_projectilePrefab, _muzzle.position, Quaternion.identity);
            shot.Launch(target, damage, Ability);
            Attacked?.Invoke();
        }

        /// <summary>
        /// The cone rule: true when <paramref name="point"/> lies within <paramref name="coneDegrees"/>
        /// (the whole width) of the direction <paramref name="aim"/> from <paramref name="from"/>, on the
        /// map's plane. A cone of 0 (or 360 and over) takes everything; so does a point on the tower itself.
        /// </summary>
        public static bool InCone(Vector3 from, Vector3 aim, Vector3 point, float coneDegrees)
        {
            if (coneDegrees <= 0f || coneDegrees >= 360f)
            {
                return true;
            }

            var toPoint = new Vector2(point.x - from.x, point.y - from.y);
            var forward = new Vector2(aim.x, aim.y);
            if (toPoint.sqrMagnitude < 0.0001f || forward.sqrMagnitude < 0.0001f)
            {
                return true;
            }

            return Vector2.Angle(forward, toPoint) <= coneDegrees * 0.5f;
        }

        /// <summary>The tower's ability (mine layer, slow aura) tells the tower it acted, for the animation.</summary>
        public void ReportAbilityUsed()
        {
            AbilityUsed?.Invoke();
        }

        // Draws the range circle, and the current target, in the Scene view when the tower is selected.
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, Range);

            if (Application.isPlaying && CurrentTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, CurrentTarget.transform.position);
            }
        }
    }
}
