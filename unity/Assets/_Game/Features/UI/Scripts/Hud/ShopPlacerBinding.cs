using HoldTheHill.Features.GameFlow;
using HoldTheHill.Features.Towers;
using UnityEngine;

namespace HoldTheHill.Features.UI
{
    /// <summary>
    /// Keeps Adam's shop cards pointed at the running level's <see cref="TowerPlacer"/>. A card
    /// finds the placer once, in its Start, so after Restart or the next level it would still hold
    /// the old, destroyed one. Sits on the TowerList root and hands each run's placer to every
    /// <see cref="TowerCardUI"/> under it; between levels the cards hold none.
    /// </summary>
    /// <remarks>
    /// Cards the shop makes after a Bind (its first Start) find the placer themselves.
    /// </remarks>
    [DisallowMultipleComponent]
    public class ShopPlacerBinding : LevelRunListener
    {
        protected override void Bind(LevelRun run)
        {
            GiveCards(run.Placer);
        }

        protected override void Unbind(LevelRun run)
        {
            GiveCards(null);
        }

        private void GiveCards(TowerPlacer placer)
        {
            // Inactive cards too: the list may be scrolled or hidden when a run starts.
            foreach (TowerCardUI card in GetComponentsInChildren<TowerCardUI>(true))
            {
                card.towerPlacer = placer;
            }
        }
    }
}
