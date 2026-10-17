using System.Collections.Generic;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>Where and when an enemy fell, and which waypoint it was walking to.</summary>
    public readonly struct DeathRecord
    {
        public DeathRecord(float x, float y, int waypointIndex, float time)
        {
            X = x;
            Y = y;
            WaypointIndex = waypointIndex;
            Time = time;
        }

        public float X { get; }
        public float Y { get; }
        public int WaypointIndex { get; }
        public float Time { get; }
    }

    /// <summary>
    /// The Queen Mantis's memory of recent deaths, as plain C# so EditMode tests can cover it. Only
    /// the last <see cref="Capacity"/> deaths are kept. Her own death and the deaths of husks she
    /// raised do not count, so a husk never comes back twice. <see cref="TakeNear"/> hands out the
    /// newest deaths within reach that are younger than <see cref="MemorySeconds"/>, and forgets them.
    /// </summary>
    public sealed class BossDeathLog
    {
        private readonly List<DeathRecord> _deaths;
        private readonly HashSet<ulong> _husks = new HashSet<ulong>();

        /// <param name="ownerId">The boss's own id (its <see cref="UnityEngine.EntityId"/> as a number); its death is never recorded.</param>
        /// <param name="capacity">How many deaths are remembered at most; the oldest are dropped first.</param>
        /// <param name="memorySeconds">A death older than this is never raised.</param>
        public BossDeathLog(ulong ownerId, int capacity, float memorySeconds)
        {
            OwnerId = ownerId;
            Capacity = capacity < 1 ? 1 : capacity;
            MemorySeconds = memorySeconds < 0f ? 0f : memorySeconds;
            _deaths = new List<DeathRecord>(Capacity);
        }

        public ulong OwnerId { get; }
        public int Capacity { get; }
        public float MemorySeconds { get; }

        /// <summary>Deaths remembered right now, expired ones included until the next <see cref="TakeNear"/>.</summary>
        public int Count => _deaths.Count;

        /// <summary>Marks an enemy as a husk this boss raised, so its death will not count.</summary>
        public void MarkHusk(ulong enemyId)
        {
            _husks.Add(enemyId);
        }

        /// <summary>
        /// Notes a death. Returns false, and remembers nothing, for the boss itself and for a husk
        /// it raised (the husk is then forgotten too).
        /// </summary>
        public bool Record(ulong enemyId, float x, float y, int waypointIndex, float time)
        {
            if (enemyId == OwnerId || _husks.Remove(enemyId))
            {
                return false;
            }

            _deaths.Add(new DeathRecord(x, y, waypointIndex, time));
            if (_deaths.Count > Capacity)
            {
                _deaths.RemoveAt(0);
            }
            return true;
        }

        /// <summary>
        /// Moves up to <paramref name="maxCount"/> deaths within <paramref name="radius"/> of a point into
        /// <paramref name="into"/>, newest first, and forgets them. Deaths older than the memory are
        /// dropped on the way. Returns how many were taken.
        /// </summary>
        public int TakeNear(float x, float y, float radius, int maxCount, float now, List<DeathRecord> into)
        {
            int taken = 0;
            float radiusSquared = radius * radius;

            for (int i = _deaths.Count - 1; i >= 0 && taken < maxCount; i--)
            {
                DeathRecord death = _deaths[i];
                if (now - death.Time > MemorySeconds)
                {
                    _deaths.RemoveAt(i);
                    continue;
                }

                float dx = death.X - x;
                float dy = death.Y - y;
                if (dx * dx + dy * dy > radiusSquared)
                {
                    continue;
                }

                into.Add(death);
                _deaths.RemoveAt(i);
                taken++;
            }

            return taken;
        }
    }
}
