using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// Base for each piece of the Queen Mantis (heal aura, raiser, summoner, look, body markers): a
    /// small component beside <see cref="QueenMantisBoss"/> that reads her <see cref="QueenMantisData"/>
    /// and hears her shade changes through the hooks below.
    /// </summary>
    [RequireComponent(typeof(QueenMantisBoss))]
    public abstract class QueenMantisPart : MonoBehaviour
    {
        /// <summary>The Queen Mantis this piece belongs to.</summary>
        protected QueenMantisBoss Boss { get; private set; }

        /// <summary>Her numbers and art; null on a prefab the builder has not filled.</summary>
        protected QueenMantisData Data => Boss.Data;

        protected virtual void Awake()
        {
            Boss = GetComponent<QueenMantisBoss>();
        }

        protected virtual void OnEnable()
        {
            Boss.Entered += OnEntered;
            Boss.ShadeChanged += OnShadeChanged;
            Boss.PrayEnded += OnPrayEnded;
        }

        protected virtual void OnDisable()
        {
            Boss.Entered -= OnEntered;
            Boss.ShadeChanged -= OnShadeChanged;
            Boss.PrayEnded -= OnPrayEnded;
        }

        /// <summary>She has her road and is walking in.</summary>
        protected virtual void OnEntered()
        {
        }

        /// <summary>She stepped into a new third and turned <paramref name="shade"/>; her pray pause starts now.</summary>
        protected virtual void OnShadeChanged(QueenMantisShade shade)
        {
        }

        /// <summary>Her pray pause is over and she walks and acts again.</summary>
        protected virtual void OnPrayEnded()
        {
        }

        /// <summary>
        /// Puts a copy of <paramref name="prefab"/> on her road at <paramref name="at"/>, walking on
        /// towards waypoint <paramref name="waypointIndex"/>, in her scene and under her parent.
        /// </summary>
        protected BasicEnemy SpawnOnHerRoad(BasicEnemy prefab, Vector3 at, int waypointIndex)
        {
            Transform parent = transform.parent;
            BasicEnemy spawned = Instantiate(prefab, at, Quaternion.identity, parent);
            if (parent == null)
            {
                SceneManager.MoveGameObjectToScene(spawned.gameObject, gameObject.scene);
            }

            // Set before its Start looks for a path of its own.
            if (Boss.PathCoordinates.Count > 0)
            {
                spawned.SetPath(Boss.PathCoordinates, waypointIndex, teleportToStart: false);
            }
            return spawned;
        }
    }
}
