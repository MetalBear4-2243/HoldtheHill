using UnityEngine;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// The numbers behind one kind of enemy: what a designer tunes without touching a prefab or
    /// code. Every enemy prefab (a variant of Enemy_Base) points at one of these, and
    /// <see cref="BasicEnemy"/> reads it when the enemy spawns. An enemy's abilities (the shield,
    /// the curl, the split) are components on its variant; their numbers are the
    /// <see cref="Abilities"/> here, so the Balance Table window shows every enemy in one table.
    /// Data, not code (UNITY_RULES 32).
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Hold the Hill/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Health and combat")]
        [Tooltip("Health it spawns with. Hits take it down; at 0 it dies.")]
        [Min(0f)] public double MaxHealth = 100.0;

        [Tooltip("Damage it does to the Queen when it reaches her.")]
        [Min(0)] public int Damage = 1;

        [Tooltip("Honeydew paid when it is killed. Nothing is paid for reaching the Queen.")]
        [Min(0)] public int Honeydew = 5;

        [Header("Movement")]
        [Tooltip("Walking speed along the road, in world units (tiles) per second.")]
        [Min(0f)] public float MoveSpeed = 2f;

        [Tooltip("How close to a waypoint counts as reaching it, in world units. Negative values would never arrive.")]
        [Min(0f)] public float WaypointTolerance = 0.05f;

        [Tooltip("If on, the sprite flips left or right with its direction. Off for the sheet art, whose Body turns to face the road instead.")]
        public bool FlipSpriteFacing;

        [Header("Rules")]
        [Tooltip("If on, it is in the air: melee and lobbed towers can't reach it (the Wasp rule). Straight shots, mines and the slow aura still can.")]
        public bool IsFlying;

        [Tooltip("If on, one click on it kills it outright (the Silverfish rule).")]
        public bool DiesToClick;

        [Header("Abilities")]
        [Tooltip("The strength of the abilities this enemy's variant carries (shield, curl, split). Fields for a component it lacks do nothing.")]
        public EnemyAbilitySettings Abilities;
    }
}
