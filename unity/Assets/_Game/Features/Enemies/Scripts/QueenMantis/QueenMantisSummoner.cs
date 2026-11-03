using System;
using UnityEngine;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// Her escort: <see cref="QueenMantisData.SummonOnEntry"/> Mantis Spawn when she walks in and
    /// <see cref="QueenMantisData.SummonOnShadeChange"/> more at each shade change, on a ring around
    /// her (<see cref="QueenMantisSummonRule"/>). With <see cref="QueenMantisData.EscortsKeepHerPace"/>
    /// they walk at her speed, so her heal and her raise have someone to act on (review EN-4).
    /// </summary>
    [DisallowMultipleComponent]
    public class QueenMantisSummoner : QueenMantisPart
    {
        /// <summary>Mantis Spawn appeared beside her; the number is how many.</summary>
        public event Action<int> Summoned;

        protected override void OnEntered()
        {
            if (Data != null)
            {
                Summon(Data.SummonOnEntry);
            }
        }

        protected override void OnShadeChanged(QueenMantisShade shade)
        {
            if (Data != null)
            {
                Summon(Data.SummonOnShadeChange);
            }
        }

        private void Summon(int count)
        {
            if (count <= 0 || Data.SpawnPrefab == null)
            {
                return;
            }

            Vector3 here = transform.position;
            for (int i = 0; i < count; i++)
            {
                QueenMantisSummonRule.RingOffset(i, count, Data.SummonRingRadius, out float x, out float y);
                BasicEnemy escort = SpawnOnHerRoad(Data.SpawnPrefab, here + new Vector3(x, y, 0f), Boss.CurrentWaypointIndex);
                if (Data.EscortsKeepHerPace)
                {
                    escort.MoveSpeed = Boss.MoveSpeed;
                }
            }
            Summoned?.Invoke(count);
        }
    }
}
