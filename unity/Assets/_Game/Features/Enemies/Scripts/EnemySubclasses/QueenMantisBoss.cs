using System.Collections.Generic;
using UnityEngine;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// The Queen Mantis, Meadow's boss: a <see cref="BasicEnemy"/> that changes shade at each third
    /// of her health (<see cref="QueenMantisShadeRule"/>). In her white third she heals the enemies
    /// around her on a timer with Jason's <see cref="BasicEnemy.Heal"/>. In her black third she
    /// raises the enemies killed near her (<see cref="BossDeathLog"/>) as weaker Mantis Spawn husks
    /// at the spots where they fell, walking on from there. Husks carry the Mantis Spawn prefab's
    /// <see cref="RegisteredEnemy"/>, so the win check waits for them.
    /// </summary>
    public class QueenMantisBoss : BasicEnemy
    {
        [Header("Shades")]
        [Tooltip("Sprite colour above two thirds of her health.")]
        [SerializeField] private Color _greenShade = new Color(0.25f, 0.6f, 0.2f);

        [Tooltip("Sprite colour between one and two thirds: the healing third.")]
        [SerializeField] private Color _whiteShade = new Color(0.95f, 0.95f, 0.88f);

        [Tooltip("Sprite colour below one third: the raising third.")]
        [SerializeField] private Color _blackShade = new Color(0.12f, 0.1f, 0.14f);

        [Header("White third: healing")]
        [Tooltip("Enemies within this many world units of her are healed.")]
        [SerializeField, Min(0f)] private float _healRadius = 3f;

        [Tooltip("Health restored to each of them per heal.")]
        [SerializeField, Min(0f)] private float _healAmount = 6f;

        [Tooltip("Seconds between heals.")]
        [SerializeField, Min(0.05f)] private float _healInterval = 1f;

        [Header("Black third: raising husks")]
        [Tooltip("The Mantis Spawn prefab raised at each death spot.")]
        [SerializeField] private BasicEnemy _huskPrefab;

        [Tooltip("Deaths within this many world units of her can be raised.")]
        [SerializeField, Min(0f)] private float _raiseRadius = 4f;

        [Tooltip("At most this many husks per raise.")]
        [SerializeField, Min(1)] private int _husksPerRaise = 3;

        [Tooltip("Seconds between raises.")]
        [SerializeField, Min(0.05f)] private float _raiseInterval = 4f;

        [Tooltip("A husk's health as a share of the Mantis Spawn prefab's; 0.5 is half.")]
        [SerializeField, Range(0.05f, 1f)] private float _huskHealthFraction = 0.5f;

        [Tooltip("A death older than this many seconds is forgotten.")]
        [SerializeField, Min(0f)] private float _deathMemorySeconds = 12f;

        [Tooltip("How many recent deaths she remembers at most.")]
        [SerializeField, Min(1)] private int _deathLogCapacity = 12;

        [Tooltip("Sprite colour of a raised husk, so it reads as one.")]
        [SerializeField] private Color _huskTint = new Color(0.45f, 0.5f, 0.35f);

        private readonly List<DeathRecord> _toRaise = new List<DeathRecord>();
        private BossDeathLog _deathLog;
        private QueenMantisShade _shade;
        private float _healTimer;
        private float _raiseTimer;

        /// <summary>Her current shade.</summary>
        public QueenMantisShade Shade => _shade;

        /// <summary>The deaths she remembers.</summary>
        public BossDeathLog DeathLog => _deathLog;

        protected override void Awake()
        {
            base.Awake();
            _deathLog = new BossDeathLog(IdOf(this), _deathLogCapacity, _deathMemorySeconds);
            _shade = QueenMantisShadeRule.ForHealthFraction(1.0);
            ApplyShade();
        }

        private void OnEnable()
        {
            EnemyRegistry.EnemyDied += RecordDeath;
        }

        protected override void OnDisable()
        {
            EnemyRegistry.EnemyDied -= RecordDeath;
            base.OnDisable();
        }

        protected override void Update()
        {
            base.Update();
            if (IsDead)
            {
                return;
            }

            RefreshShade();
            switch (_shade)
            {
                case QueenMantisShade.White:
                    TickHeal(Time.deltaTime);
                    break;
                case QueenMantisShade.Black:
                    TickRaise(Time.deltaTime);
                    break;
            }
        }

        private void RefreshShade()
        {
            double fraction = MaxHealth > 0.0 ? CurrentHealth / MaxHealth : 0.0;
            QueenMantisShade shade = QueenMantisShadeRule.ForHealthFraction(fraction);
            if (shade == _shade)
            {
                return;
            }

            _shade = shade;
            _healTimer = 0f;
            _raiseTimer = 0f;
            ApplyShade();
        }

        private void ApplyShade()
        {
            if (_spriteRenderer == null)
            {
                return;
            }

            switch (_shade)
            {
                case QueenMantisShade.White:
                    _spriteRenderer.color = _whiteShade;
                    break;
                case QueenMantisShade.Black:
                    _spriteRenderer.color = _blackShade;
                    break;
                default:
                    _spriteRenderer.color = _greenShade;
                    break;
            }
        }

        private void TickHeal(float deltaTime)
        {
            _healTimer += deltaTime;
            if (_healTimer < _healInterval)
            {
                return;
            }
            _healTimer -= _healInterval;

            float radiusSquared = _healRadius * _healRadius;
            Vector3 here = transform.position;
            IReadOnlyList<BasicEnemy> all = EnemyRegistry.All;
            for (int i = 0; i < all.Count; i++)
            {
                BasicEnemy other = all[i];
                if (other == null || other == this || other.IsDead)
                {
                    continue;
                }

                Vector3 offset = other.transform.position - here;
                offset.z = 0f;
                if (offset.sqrMagnitude <= radiusSquared)
                {
                    other.Heal(_healAmount);
                }
            }
        }

        private void TickRaise(float deltaTime)
        {
            _raiseTimer += deltaTime;
            if (_raiseTimer < _raiseInterval)
            {
                return;
            }
            _raiseTimer -= _raiseInterval;

            if (_huskPrefab == null)
            {
                return;
            }

            _toRaise.Clear();
            Vector3 here = transform.position;
            _deathLog.TakeNear(here.x, here.y, _raiseRadius, _husksPerRaise, Time.time, _toRaise);
            for (int i = 0; i < _toRaise.Count; i++)
            {
                RaiseHusk(_toRaise[i]);
            }
        }

        private void RaiseHusk(DeathRecord death)
        {
            var at = new Vector3(death.X, death.Y, transform.position.z);
            BasicEnemy husk = Instantiate(_huskPrefab, at, Quaternion.identity, transform.parent);
            husk.SetMaxHealth(husk.MaxHealth * _huskHealthFraction, resetCurrentHealth: true);

            // Walk on towards the waypoint the dead enemy was heading for; set before its Start looks for a path.
            if (PathCoordinates.Count > 0)
            {
                husk.SetPath(PathCoordinates, death.WaypointIndex, teleportToStart: false);
            }

            SpriteRenderer sprite = husk.GetComponentInChildren<SpriteRenderer>();
            if (sprite != null)
            {
                sprite.color = _huskTint;
            }

            _deathLog.MarkHusk(IdOf(husk));
        }

        // The log is plain C#, so it keys enemies by their EntityId as a number.
        private static ulong IdOf(BasicEnemy enemy)
        {
            return EntityId.ToULong(enemy.GetEntityId());
        }

        private void RecordDeath(BasicEnemy enemy)
        {
            if (enemy == null)
            {
                return;
            }

            Vector3 at = enemy.transform.position;
            _deathLog.Record(IdOf(enemy), at.x, at.y, enemy.CurrentWaypointIndex, Time.time);
        }
    }
}
