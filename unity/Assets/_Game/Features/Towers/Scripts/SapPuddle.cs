using System.Collections.Generic;
using HoldTheHill.Core.Animation;
using HoldTheHill.Features.Combat;
using HoldTheHill.Features.Enemies;
using UnityEngine;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// Sticky sap left on the floor where the Sap Ant's glob lands. It splats out (the sheet's Hit
    /// row), stays as a puddle, and slows every ground enemy standing in it; then it fades and goes.
    /// A wasp flies over it (the wasp rule). Spawned as the sap shot's hit effect.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SapPuddle : MonoBehaviour
    {
        private const string EffectName = "Sap";
        private const string OverlayKind = "Sap";

        [Header("Sap")]
        [Tooltip("Enemies whose centre is this close to the puddle's centre are stuck in it, in world units.")]
        [SerializeField, Min(0.05f)] private float _radius = 0.35f;

        [Tooltip("Movement speed while in the sap. 0.5 is half speed.")]
        [SerializeField, Range(0.05f, 1f)] private float _slowMultiplier = 0.5f;

        [Tooltip("Seconds the puddle lies there, fade included.")]
        [SerializeField, Min(0.1f)] private float _seconds = 4f;

        [Tooltip("Seconds at the end over which it fades out (it still slows while fading).")]
        [SerializeField, Min(0f)] private float _fadeSeconds = 0.6f;

        [Tooltip("Seconds between re-applying the slow to everyone in the puddle.")]
        [SerializeField, Min(0.05f)] private float _tickInterval = 0.2f;

        [Header("Look")]
        [Tooltip("The splat, played once as it lands; its last frame stays as the puddle.")]
        [SerializeField] private Sprite[] _splatFrames = System.Array.Empty<Sprite>();

        [Tooltip("Seconds each splat frame shows.")]
        [SerializeField, Min(0.01f)] private float _splatFrameSeconds = 0.055f;

        [Tooltip("Optional: worn by an enemy while it is stuck (FxStatusOverlay Honey).")]
        [SerializeField] private Sprite[] _overlayFrames = System.Array.Empty<Sprite>();

        [Tooltip("Seconds each overlay frame shows.")]
        [SerializeField, Min(0.01f)] private float _overlayFrameSeconds = 0.2f;

        private SpriteRenderer _renderer;
        private Color _baseColor = Color.white;
        private float _age;
        private float _untilNextTick;

        /// <summary>Movement speed while in the sap.</summary>
        public float SlowMultiplier => _slowMultiplier;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _baseColor = _renderer.color;
        }

        private void OnEnable()
        {
            _age = 0f;
            _untilNextTick = 0f;
            ShowSplat();
        }

        private void Update()
        {
            _age += Time.deltaTime;
            if (_age >= _seconds)
            {
                Destroy(gameObject);
                return;
            }

            ShowSplat();
            Color color = _baseColor;
            color.a *= FadeAlpha(_age, _seconds, _fadeSeconds);
            _renderer.color = color;

            _untilNextTick -= Time.deltaTime;
            if (_untilNextTick <= 0f)
            {
                _untilNextTick = _tickInterval;
                SlowEnemiesInside();
            }
        }

        /// <summary>How opaque the puddle is <paramref name="age"/> seconds in: whole, then down to nothing over the last <paramref name="fadeSeconds"/>.</summary>
        public static float FadeAlpha(float age, float seconds, float fadeSeconds)
        {
            if (fadeSeconds <= 0f)
            {
                return age < seconds ? 1f : 0f;
            }

            return Mathf.Clamp01((seconds - age) / fadeSeconds);
        }

        // The splat plays once; after it the last frame stays.
        private void ShowSplat()
        {
            int count = _splatFrames != null ? _splatFrames.Length : 0;
            if (count == 0)
            {
                return;
            }

            int index = SpriteRow.FrameAt(_age, _splatFrameSeconds, count);
            Sprite frame = _splatFrames[index >= 0 ? index : count - 1];
            if (frame != null && _renderer.sprite != frame)
            {
                _renderer.sprite = frame;
            }
        }

        private void SlowEnemiesInside()
        {
            // Twice the tick, so the slow never flickers off between two applications; it wears off
            // soon after an enemy walks out or the puddle goes.
            var effect = new StatusEffectData
            {
                EffectName = EffectName,
                Duration = _tickInterval * 2f,
                SlowMultiplier = _slowMultiplier,
            };

            Vector3 here = transform.position;
            float radiusSqr = _radius * _radius;
            IReadOnlyList<BasicEnemy> enemies = EnemyRegistry.All;
            for (int i = 0; i < enemies.Count; i++)
            {
                BasicEnemy enemy = enemies[i];
                if (enemy == null || enemy.IsDead)
                {
                    continue;
                }

                Vector3 offset = enemy.transform.position - here;
                offset.z = 0f;
                if (offset.sqrMagnitude > radiusSqr || (enemy is IFlyingEnemy flyer && flyer.IsFlying))
                {
                    continue;
                }

                enemy.ApplyStatusEffect(effect);
                StatusOverlay.Show(enemy, OverlayKind, _overlayFrames, _overlayFrameSeconds, effect.Duration);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.75f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
