using System.Collections.Generic;
using UnityEngine;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// The targeting rules, as plain C# so EditMode tests can cover them: how far along the path
    /// an enemy is, and which of several candidates a <see cref="TargetingPriority"/> picks.
    /// </summary>
    public static class TargetPicker
    {
        /// <summary>One enemy in range, reduced to the two numbers the priorities compare.</summary>
        public struct Candidate
        {
            /// <summary>Distance walked along the path, in world units. Higher is nearer the Queen.</summary>
            public float PathProgress;

            /// <summary>Squared straight-line distance from the tower.</summary>
            public float SqrDistance;
        }

        /// <summary>
        /// Index of the candidate the priority prefers, or -1 when the list is empty.
        /// Ties go to the earlier candidate.
        /// </summary>
        public static int Pick(IReadOnlyList<Candidate> candidates, TargetingPriority priority)
        {
            int best = -1;
            float bestScore = float.NegativeInfinity;

            for (int i = 0; i < candidates.Count; i++)
            {
                float score = ScoreFor(candidates[i], priority);
                if (score > bestScore)
                {
                    bestScore = score;
                    best = i;
                }
            }

            return best;
        }

        // Every priority is expressed as "higher is better" so one loop covers them all.
        private static float ScoreFor(Candidate candidate, TargetingPriority priority)
        {
            switch (priority)
            {
                case TargetingPriority.First:
                    return candidate.PathProgress;
                case TargetingPriority.Last:
                    return -candidate.PathProgress;
                default:
                    return -candidate.SqrDistance;
            }
        }

        /// <summary>
        /// How far along <paramref name="path"/> something at <paramref name="position"/> has walked,
        /// given that it is heading for waypoint <paramref name="nextWaypointIndex"/>: the path length
        /// up to that waypoint minus the distance still to go. Past the last waypoint it is the whole
        /// path length; with no path it is zero, so First and Last behave like Closest.
        /// </summary>
        public static float PathProgress(IReadOnlyList<Vector3> path, int nextWaypointIndex, Vector3 position)
        {
            if (path == null || path.Count == 0)
            {
                return 0f;
            }

            int last = Mathf.Min(nextWaypointIndex, path.Count - 1);
            float length = 0f;
            for (int i = 0; i < last; i++)
            {
                length += Vector2.Distance(path[i], path[i + 1]);
            }

            if (nextWaypointIndex >= path.Count)
            {
                return length;
            }

            return length - Vector2.Distance(position, path[last]);
        }
    }
}
