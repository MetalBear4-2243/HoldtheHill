using System.Collections.Generic;
using UnityEngine;

namespace HoldTheHill.Features.Combat
{
    /// <summary>
    /// Shared queries, splash calculations, and life-check utilities for towers, weapons, and damageables.
    /// Uses static non-allocating buffers to avoid garbage collection spikes during waves.
    /// </summary>
    public static class CombatUtil
    {
        private const int OverlapBufferSize = 128;
        private static readonly Collider2D[] OverlapColliderBuffer = new Collider2D[OverlapBufferSize];
        private static readonly HashSet<IDamageable> SeenDamageables = new HashSet<IDamageable>();
        private static readonly List<IDamageable> SharedSplashBuffer = new List<IDamageable>();
        private static bool _isSplashing;
        private static ContactFilter2D _contactFilter;

        static CombatUtil()
        {
            InitializeContactFilter();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticBuffers()
        {
            for (int i = 0; i < OverlapColliderBuffer.Length; i++)
            {
                OverlapColliderBuffer[i] = null;
            }
            SeenDamageables.Clear();
            SharedSplashBuffer.Clear();
            _isSplashing = false;
            InitializeContactFilter();
        }

        private static void InitializeContactFilter()
        {
            _contactFilter = new ContactFilter2D
            {
                useTriggers = true,
                useLayerMask = true
            };
        }

        /// <summary>
        /// Fills results with live IDamageable targets whose collider overlaps the circle.
        /// Clears results first, returns immediately if radius <= 0, and deduplicates targets with multiple colliders.
        /// </summary>
        public static void OverlapDamageables(Vector2 center, float radius, LayerMask mask, List<IDamageable> results)
        {
            if (results == null)
            {
                return;
            }

            results.Clear();

            if (radius <= 0f)
            {
                return;
            }

            _contactFilter.layerMask = mask;
            int count = Physics2D.OverlapCircle(center, radius, _contactFilter, OverlapColliderBuffer);

            SeenDamageables.Clear();
            try
            {
                for (int i = 0; i < count; i++)
                {
                    Collider2D col = OverlapColliderBuffer[i];
                    if (TryGetDamageable(col, out IDamageable damageable))
                    {
                        if (SeenDamageables.Add(damageable))
                        {
                            results.Add(damageable);
                        }
                    }
                }
            }
            finally
            {
                // Clear references in buffer so destroyed objects are not held in memory
                for (int i = 0; i < count; i++)
                {
                    OverlapColliderBuffer[i] = null;
                }
                SeenDamageables.Clear();
            }
        }

        /// <summary>
        /// Returns the live IDamageable nearest to a point within a radius, or null if none are found.
        /// </summary>
        public static IDamageable NearestDamageable(Vector2 from, float radius, LayerMask mask, List<IDamageable> buffer)
        {
            if (buffer == null)
            {
                return null;
            }

            OverlapDamageables(from, radius, mask, buffer);
            if (buffer.Count == 0)
            {
                return null;
            }

            IDamageable nearest = null;
            float minSqrDist = float.PositiveInfinity;

            for (int i = 0; i < buffer.Count; i++)
            {
                IDamageable candidate = buffer[i];
                if (!IsAlive(candidate))
                {
                    continue;
                }

                Vector2 pos = candidate.Transform.position;
                float sqrDist = (pos - from).sqrMagnitude;
                if (sqrDist < minSqrDist)
                {
                    minSqrDist = sqrDist;
                    nearest = candidate;
                }
            }

            return nearest;
        }

        /// <summary>
        /// Checks whether a target exists, is not destroyed in Unity, is alive (not dead), and has an active Transform.
        /// Casts to UnityEngine.Object to avoid fake null references with interface references.
        /// </summary>
        public static bool IsAlive(IDamageable target)
        {
            if (target == null)
            {
                return false;
            }

            if (target is Object unityObj && unityObj == null)
            {
                return false;
            }

            return !target.IsDead && target.Transform != null;
        }

        /// <summary>
        /// Finds the live IDamageable owning a collider (checking the collider and its parents), if any.
        /// </summary>
        public static bool TryGetDamageable(Collider2D collider, out IDamageable damageable)
        {
            damageable = null;
            if (collider == null)
            {
                return false;
            }

            damageable = collider.GetComponentInParent<IDamageable>();
            return IsAlive(damageable);
        }

        /// <summary>
        /// Damages everything in a circle with DamageInfo and applies status when valid.
        /// Skips the ignore target (usually the direct-hit target).
        /// Handles nested splash calls re-entrantly.
        /// </summary>
        public static void ApplySplash(
            Vector2 center,
            float radius,
            LayerMask mask,
            float damage,
            DamageType type,
            GameObject source,
            StatusEffectData status,
            IDamageable ignore = null)
        {
            if (radius <= 0f || damage <= 0f)
            {
                return;
            }

            List<IDamageable> targets;
            bool wasNested = _isSplashing;

            if (wasNested)
            {
                targets = new List<IDamageable>();
            }
            else
            {
                _isSplashing = true;
                targets = SharedSplashBuffer;
            }

            try
            {
                OverlapDamageables(center, radius, mask, targets);

                DamageInfo splashDamageInfo = new DamageInfo(damage, source, center, type);

                for (int i = 0; i < targets.Count; i++)
                {
                    IDamageable target = targets[i];
                    if (target == ignore || !IsAlive(target))
                    {
                        continue;
                    }

                    target.TakeDamage(splashDamageInfo);

                    if (status.IsValid && IsAlive(target))
                    {
                        target.ApplyStatusEffect(status);
                    }
                }
            }
            finally
            {
                if (!wasNested)
                {
                    SharedSplashBuffer.Clear();
                    _isSplashing = false;
                }
            }
        }

        /// <summary>
        /// Returns who should be credited for damage from source: the IOwned.Owner when present, otherwise the source itself.
        /// </summary>
        public static GameObject OwnerOf(GameObject source)
        {
            if (source == null)
            {
                return null;
            }

            IOwned owned = source.GetComponentInParent<IOwned>();
            if (owned != null && owned.Owner != null)
            {
                return owned.Owner;
            }

            return source;
        }

        /// <summary>
        /// Calculates squared distance in 2D between two transforms. Returns PositiveInfinity if either is null.
        /// </summary>
        public static float SqrDistance(Transform a, Transform b)
        {
            if (a == null || b == null)
            {
                return float.PositiveInfinity;
            }

            Vector2 posA = a.position;
            Vector2 posB = b.position;
            return (posA - posB).sqrMagnitude;
        }
    }
}
