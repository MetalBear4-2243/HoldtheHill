using UnityEngine;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// A subclass of BasicEnemy that represents a Silverfish.
    /// The Silverfish enemy can be instantly defeated when clicked by the player.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class SilverfishEnemy : BasicEnemy
    {
        /// <summary>
        /// Instantly defeats the Silverfish when clicked.
        /// Requires a Collider2D on the GameObject to detect mouse clicks.
        /// </summary>
        /// 
        /// commenting out this function since we are no longer using it
        /// not deleting it just so we still have it around
        // private void OnMouseDown()
        // {
        //     if (IsDead)
        //     {
        //         return;
        //     }

        //     // Instantly deplete all remaining health
        //     TakeDamage(CurrentHealth);
        // }
    }
}
