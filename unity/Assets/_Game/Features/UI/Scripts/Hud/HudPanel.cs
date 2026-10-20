using HoldTheHill.Features.GameFlow;
using UnityEngine;

namespace HoldTheHill.Features.UI
{
    /// <summary>
    /// Shows the HUD only while a level runs, so the home screen is not drawn over by a stale
    /// Honeydew count and wave label. Sits on the canvas, above the HUD object it toggles: a piece
    /// that disabled itself would never hear the next run start.
    /// </summary>
    public class HudPanel : LevelRunListener
    {
        [Tooltip("The HUD root (Honeydew, wave, Queen bar, Next Wave). Inactive between levels.")]
        [SerializeField] private GameObject _hud;

        protected override void OnEnable()
        {
            _hud.SetActive(false);
            base.OnEnable();
        }

        protected override void Bind(LevelRun run)
        {
            _hud.SetActive(true);
        }

        protected override void Unbind(LevelRun run)
        {
            _hud.SetActive(false);
        }
    }
}
