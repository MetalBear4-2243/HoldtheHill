using UnityEngine;

namespace HoldTheHill.Features.Economy
{
    /// <summary>
    /// Holds the run's <see cref="Honeydew"/> in the gameplay scene. One per level.
    /// Towers spend from it, bounties and wave rewards pay into it, the HUD listens to it.
    /// </summary>
    [DisallowMultipleComponent]
    public class HoneydewWallet : MonoBehaviour
    {
        [Tooltip("Honeydew the player starts the level with.")]
        [SerializeField, Min(0)] private int _startingHoneydew = 300;

        private Honeydew _honeydew;

        /// <summary>The counter itself. Created in Awake, so read it from Start or later.</summary>
        public Honeydew Honeydew => _honeydew;

        private void Awake()
        {
            _honeydew = new Honeydew(_startingHoneydew);
        }

        /// <summary>Puts the amount back to the starting value, for a level restart.</summary>
        public void ResetToStart()
        {
            _honeydew.Set(_startingHoneydew);
        }
    }
}
