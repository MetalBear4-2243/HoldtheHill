using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// The Queen Mantis, Meadow's boss: a <see cref="BasicEnemy"/> that changes shade each time she
    /// loses a third of her health, white, black, white (<see cref="QueenMantisShadeCycle"/>, GDD
    /// decision 108). Each change is a pray pause: she stands still for
    /// <see cref="QueenMantisData.PraySeconds"/> and does nothing else, the player's opening.
    /// </summary>
    /// <remarks>
    /// This class is only the shade state machine and the one place that talks to Jason's
    /// <see cref="BasicEnemy"/>. Each mechanic is a <see cref="QueenMantisPart"/> beside it: the white
    /// heal (<see cref="QueenMantisHealAura"/>), the black raise (<see cref="QueenMantisRaiser"/>),
    /// the summons (<see cref="QueenMantisSummoner"/>), and the looks (<see cref="QueenMantisLook"/>,
    /// <see cref="QueenMantisBodyMarkers"/>). They and the HUD follow her through the events below.
    /// </remarks>
    public class QueenMantisBoss : BasicEnemy
    {
        private static readonly List<QueenMantisBoss> s_live = new List<QueenMantisBoss>();

        [Header("Queen Mantis")]
        [Tooltip("Her mechanic numbers, art and the Mantis Spawn prefab. Health, speed and damage are the fields above.")]
        [SerializeField] private QueenMantisData _data;

        private readonly QueenMantisShadeCycle _cycle = new QueenMantisShadeCycle();
        private float _prayLeft;
        private float _prayLength;
        private bool _entered;

        /// <summary>A Queen Mantis walked in. Raised once per Queen Mantis, after her own <see cref="Entered"/>.</summary>
        public static event Action<QueenMantisBoss> Appeared;

        /// <summary>A Queen Mantis left: killed, at the Queen, or unloaded with her level.</summary>
        public static event Action<QueenMantisBoss> Gone;

        /// <summary>Every Queen Mantis walking right now, oldest first; the HUD bar shows the first.</summary>
        public static IReadOnlyList<QueenMantisBoss> Live => s_live;

        /// <summary>She has her road and is walking in.</summary>
        public event Action Entered;

        /// <summary>She stepped into a new third and turned this shade; her pray pause starts now.</summary>
        public event Action<QueenMantisShade> ShadeChanged;

        /// <summary>Her pray pause is over.</summary>
        public event Action PrayEnded;

        /// <summary>Her numbers and art.</summary>
        public QueenMantisData Data => _data;

        /// <summary>Her shade now.</summary>
        public QueenMantisShade Shade => _cycle.Shade;

        /// <summary>How many thirds of her health she has changed shade for: 0, 1 or 2.</summary>
        public int ThirdsLost => _cycle.ThirdsLost;

        /// <summary>True during a pray pause, when she stands still and can't heal or raise.</summary>
        public bool IsPraying => _prayLeft > 0f;

        /// <summary>How far through the current pray pause she is, 0 to 1; 1 when not praying.</summary>
        public float PrayProgress => _prayLength > 0f ? 1f - _prayLeft / _prayLength : 1f;

        /// <summary>True while her mechanics may act: alive and not praying.</summary>
        public bool CanAct => !IsDead && !IsPraying;

        /// <summary>Her health over her maximum, 0 to 1.</summary>
        public float HealthFraction => MaxHealth > 0.0 ? (float)(CurrentHealth / MaxHealth) : 0f;

        // Domain reload is on, but clearing keeps Play mode safe if that ever changes.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            s_live.Clear();
            Appeared = null;
            Gone = null;
        }

        protected override void Start()
        {
            // Finds her road if the spawner did not give her one.
            base.Start();

            _entered = true;
            s_live.Add(this);
            Entered?.Invoke();
            Appeared?.Invoke(this);
        }

        protected override void OnDisable()
        {
            if (_entered)
            {
                _entered = false;
                s_live.Remove(this);
                Gone?.Invoke(this);
            }
            base.OnDisable();
        }

        protected override void Update()
        {
            base.Update();
            if (IsDead)
            {
                return;
            }

            if (_prayLeft > 0f)
            {
                _prayLeft -= Time.deltaTime;
                if (_prayLeft <= 0f)
                {
                    _prayLeft = 0f;
                    PrayEnded?.Invoke();
                }
                return;
            }

            // In double: as a float, 200 of 300 rounds above two thirds and the exact third would not count.
            double fraction = MaxHealth > 0.0 ? CurrentHealth / MaxHealth : 0.0;
            if (_cycle.TryAdvance(fraction))
            {
                _prayLength = _data != null ? _data.PraySeconds : 0f;
                _prayLeft = _prayLength;
                ShadeChanged?.Invoke(_cycle.Shade);
                if (_prayLeft <= 0f)
                {
                    PrayEnded?.Invoke();
                }
            }
        }

        // She stands still while she prays.
        protected override void TraversePath()
        {
            if (IsPraying)
            {
                return;
            }
            base.TraversePath();
        }
    }
}
