using System.Collections.Generic;
using UnityEngine;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// The Centipede boss's first mechanic, beside the <see cref="CentipedeSplitter"/>, which does
    /// the second (splitting in two on death). Listens to its <see cref="CentipedeSplitter.OnSplit"/>
    /// and makes every child faster than its parent, so the boss speeds up with each lost part.
    /// Also makes sure each child carries this component and a <see cref="RegisteredEnemy"/>, so
    /// grandchildren speed up too and the win check sees every part. A <see cref="Economy.Bounty"/>
    /// comes along when the children are copies of the parent (the centipede's split prefab left empty).
    /// The factor is the enemy's <see cref="EnemyAbilitySettings.ChildSpeedFactor"/> (its data asset).
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CentipedeSplitter))]
    public class CentipedeSpeedUp : MonoBehaviour
    {
        private CentipedeSplitter _centipede;

        /// <summary>How much faster each child is than its parent: the data asset's factor, or the default without one.</summary>
        public float ChildSpeedFactor
        {
            get
            {
                BasicEnemy enemy = GetComponent<BasicEnemy>();
                return enemy != null && enemy.Data != null
                    ? Mathf.Max(1f, enemy.Data.Abilities.ChildSpeedFactor)
                    : EnemyAbilitySettings.Default.ChildSpeedFactor;
            }
        }

        private void Awake()
        {
            _centipede = GetComponent<CentipedeSplitter>();
        }

        private void OnEnable()
        {
            _centipede.OnSplit += SpeedUpChildren;
        }

        private void OnDisable()
        {
            _centipede.OnSplit -= SpeedUpChildren;
        }

        private void SpeedUpChildren(BasicEnemy parent, List<BasicEnemy> children)
        {
            float factor = ChildSpeedFactor;
            for (int i = 0; i < children.Count; i++)
            {
                BasicEnemy child = children[i];
                if (child == null)
                {
                    continue;
                }

                // Split() has just copied the parent's speed onto the child; build on that.
                child.MoveSpeed = parent.MoveSpeed * factor;
                EnsureAlphaComponents(child.gameObject);
            }
        }

        // The children are copies of the parent unless a split prefab is set; cover both cases.
        // A child reads the factor from its own data asset, the same one as its parent's.
        private static void EnsureAlphaComponents(GameObject child)
        {
            if (child.GetComponent<CentipedeSpeedUp>() == null)
            {
                child.AddComponent<CentipedeSpeedUp>();
            }

            if (child.GetComponent<RegisteredEnemy>() == null)
            {
                child.AddComponent<RegisteredEnemy>();
            }
        }
    }
}
