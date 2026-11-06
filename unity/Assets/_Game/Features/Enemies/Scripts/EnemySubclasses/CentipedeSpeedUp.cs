using System.Collections.Generic;
using UnityEngine;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// The Centipede boss's first mechanic, beside Jason's <see cref="CentipedeEnemy"/>, which does
    /// the second (splitting in two on death). Listens to his <see cref="CentipedeEnemy.OnSplit"/>
    /// and makes every child faster than its parent, so the boss speeds up with each lost part.
    /// Also makes sure each child carries this component and a <see cref="RegisteredEnemy"/>, so
    /// grandchildren speed up too and the win check sees every part. A <see cref="Economy.Bounty"/>
    /// comes along when the children are copies of the parent (the centipede's split prefab left empty).
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CentipedeEnemy))]
    public class CentipedeSpeedUp : MonoBehaviour
    {
        [Tooltip("Each child's speed is its parent's times this. 1.25 means a quarter faster per split.")]
        [SerializeField, Min(1f)] private float _childSpeedFactor = 1.25f;

        private CentipedeEnemy _centipede;

        /// <summary>How much faster each child is than its parent.</summary>
        public float ChildSpeedFactor => _childSpeedFactor;

        private void Awake()
        {
            _centipede = GetComponent<CentipedeEnemy>();
        }

        private void OnEnable()
        {
            _centipede.OnSplit += SpeedUpChildren;
        }

        private void OnDisable()
        {
            _centipede.OnSplit -= SpeedUpChildren;
        }

        private void SpeedUpChildren(CentipedeEnemy parent, List<CentipedeEnemy> children)
        {
            for (int i = 0; i < children.Count; i++)
            {
                CentipedeEnemy child = children[i];
                if (child == null)
                {
                    continue;
                }

                // Jason's Split() has just copied the parent's speed onto the child; build on that.
                child.MoveSpeed = parent.MoveSpeed * _childSpeedFactor;
                EnsureAlphaComponents(child.gameObject);
            }
        }

        // The children are copies of the parent unless a split prefab is set; cover both cases.
        private void EnsureAlphaComponents(GameObject child)
        {
            CentipedeSpeedUp speedUp = child.GetComponent<CentipedeSpeedUp>();
            if (speedUp == null)
            {
                speedUp = child.AddComponent<CentipedeSpeedUp>();
            }
            speedUp._childSpeedFactor = _childSpeedFactor;

            if (child.GetComponent<RegisteredEnemy>() == null)
            {
                child.AddComponent<RegisteredEnemy>();
            }
        }
    }
}
