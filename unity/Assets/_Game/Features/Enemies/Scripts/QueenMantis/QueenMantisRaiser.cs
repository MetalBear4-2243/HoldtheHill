using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// The black shade. She remembers where enemies fall (<see cref="BossDeathLog"/>), in either
    /// shade. In black, every <see cref="QueenMantisData.RaiseInterval"/> seconds she picks up to
    /// <see cref="QueenMantisData.HusksPerRaise"/> of the fresh bodies within reach, they glow for
    /// <see cref="QueenMantisData.RaiseWindUp"/> seconds, and then rise as weaker, tinted Mantis
    /// Spawn husks that walk on from where they fell. A shade change during the wind-up stops the raise.
    /// </summary>
    /// <remarks>
    /// Husks carry the Mantis Spawn prefab's <see cref="RegisteredEnemy"/>, so towers target them and
    /// the win check waits for them. A husk's own death is never remembered, so nothing rises twice.
    /// </remarks>
    [DisallowMultipleComponent]
    public class QueenMantisRaiser : QueenMantisPart
    {
        private readonly List<DeathRecord> _windingUp = new List<DeathRecord>();
        private BossDeathLog _deathLog;
        private float _timer;
        private float _windUpLeft;

        /// <summary>The chosen bodies glow; they rise when the wind-up ends.</summary>
        public event Action<IReadOnlyList<DeathRecord>> WindUpStarted;

        /// <summary>Husks rose; the number is how many.</summary>
        public event Action<int> HusksRaised;

        /// <summary>The deaths she remembers; the body markers read it.</summary>
        public BossDeathLog DeathLog => _deathLog;

        /// <summary>The bodies chosen for the raise in progress, if any.</summary>
        public IReadOnlyList<DeathRecord> WindingUp => _windingUp;

        /// <summary>How far through the wind-up she is, 0 to 1.</summary>
        public float WindUpProgress => Data != null && Data.RaiseWindUp > 0f ? 1f - _windUpLeft / Data.RaiseWindUp : 1f;

        protected override void Awake()
        {
            base.Awake();
            QueenMantisData data = Data;
            _deathLog = new BossDeathLog(IdOf(Boss), data != null ? data.DeathLogCapacity : 12, data != null ? data.DeathMemorySeconds : 12f);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            EnemyRegistry.EnemyDied += RecordDeath;
        }

        protected override void OnDisable()
        {
            EnemyRegistry.EnemyDied -= RecordDeath;
            base.OnDisable();
        }

        protected override void OnShadeChanged(QueenMantisShade shade)
        {
            // Hurting her into the next third in time saves the chosen bodies from rising.
            _windingUp.Clear();
            _windUpLeft = 0f;
            _timer = 0f;
        }

        private void Update()
        {
            if (Data == null || Boss.IsDead)
            {
                return;
            }

            if (_windingUp.Count > 0)
            {
                _windUpLeft -= Time.deltaTime;
                if (_windUpLeft <= 0f)
                {
                    RaiseChosen();
                }
                return;
            }

            if (!Boss.CanAct || Boss.Shade != QueenMantisShade.Black)
            {
                return;
            }

            _timer += Time.deltaTime;
            if (_timer < Data.RaiseInterval)
            {
                return;
            }
            _timer -= Data.RaiseInterval;
            ChooseBodies();
        }

        private void ChooseBodies()
        {
            if (Data.SpawnPrefab == null)
            {
                return;
            }

            Vector3 here = transform.position;
            if (_deathLog.TakeNear(here.x, here.y, Data.RaiseRadius, Data.HusksPerRaise, Time.time, _windingUp) == 0)
            {
                return;
            }

            _windUpLeft = Data.RaiseWindUp;
            WindUpStarted?.Invoke(_windingUp);
            if (_windUpLeft <= 0f)
            {
                RaiseChosen();
            }
        }

        private void RaiseChosen()
        {
            int raised = _windingUp.Count;
            for (int i = 0; i < _windingUp.Count; i++)
            {
                RaiseHusk(_windingUp[i]);
            }
            _windingUp.Clear();
            _windUpLeft = 0f;
            HusksRaised?.Invoke(raised);
        }

        private void RaiseHusk(DeathRecord death)
        {
            var at = new Vector3(death.X, death.Y, transform.position.z);
            BasicEnemy husk = SpawnOnHerRoad(Data.SpawnPrefab, at, death.WaypointIndex);
            husk.SetMaxHealth(husk.MaxHealth * Data.HuskHealthFraction, resetCurrentHealth: true);

            SpriteRenderer sprite = husk.GetComponentInChildren<SpriteRenderer>();
            if (sprite != null)
            {
                sprite.color = Data.HuskTint;
            }

            _deathLog.MarkHusk(IdOf(husk));
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

        // The log is plain C#, so it keys enemies by their EntityId as a number.
        private static ulong IdOf(BasicEnemy enemy)
        {
            return EntityId.ToULong(enemy.GetEntityId());
        }
    }
}
