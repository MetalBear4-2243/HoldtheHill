using HoldTheHill.Features.Economy;
using HoldTheHill.Features.GameFlow;
using HoldTheHill.Features.Towers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HoldTheHill.Features.UI
{
    /// <summary>
    /// The card for the selected standing tower: its name and level, an upgrade button with the
    /// price, and the standing order (First / Last / Closest). Shown while a tower is selected.
    /// No sell, no stats page (the Alpha plan).
    /// </summary>
    public class TowerCardPanel : LevelRunListener
    {
        [Tooltip("The card root. Inactive while no tower is selected.")]
        [SerializeField] private GameObject _panel;

        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _level;
        [SerializeField] private Button _upgrade;
        [SerializeField] private TMP_Text _upgradeLabel;

        [Tooltip("One button per standing order, in enum order: First, Last, Closest.")]
        [SerializeField] private Button[] _orderButtons = new Button[3];

        private static readonly string[] OrderNames = { UiStrings.OrderFirst, UiStrings.OrderLast, UiStrings.OrderClosest };

        private TowerPlacer _placer;
        private Honeydew _honeydew;
        private Tower _tower;

        protected override void OnEnable()
        {
            _upgrade.onClick.AddListener(Upgrade);
            for (int i = 0; i < _orderButtons.Length; i++)
            {
                TargetingPriority order = (TargetingPriority)i;
                _orderButtons[i].onClick.AddListener(() => SetOrder(order));
            }

            Show(null);
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _upgrade.onClick.RemoveListener(Upgrade);
            for (int i = 0; i < _orderButtons.Length; i++)
            {
                _orderButtons[i].onClick.RemoveAllListeners();
            }
        }

        protected override void Bind(LevelRun run)
        {
            _placer = run.Placer;
            if (_placer == null)
            {
                return;
            }

            _placer.SelectedTowerChanged += Show;
            if (_placer.Wallet != null)
            {
                _honeydew = _placer.Wallet.Honeydew;
                _honeydew.Changed += HandleHoneydewChanged;
            }

            Show(_placer.SelectedTower);
        }

        protected override void Unbind(LevelRun run)
        {
            if (_placer != null)
            {
                _placer.SelectedTowerChanged -= Show;
                _placer = null;
            }

            if (_honeydew != null)
            {
                _honeydew.Changed -= HandleHoneydewChanged;
                _honeydew = null;
            }

            Show(null);
        }

        private void Show(Tower tower)
        {
            if (_tower != null)
            {
                _tower.Upgraded -= HandleUpgraded;
            }

            _tower = tower;
            _panel.SetActive(tower != null);
            if (tower == null)
            {
                return;
            }

            _tower.Upgraded += HandleUpgraded;
            _name.text = tower.ShopData != null ? tower.ShopData.towerName : UiStrings.UnknownTower;
            Refresh();
        }

        private void Refresh()
        {
            if (_tower == null)
            {
                return;
            }

            _level.text = string.Format(UiStrings.TowerLevelFormat, _tower.Level, _tower.MaxLevel);

            int honeydew = _honeydew != null ? _honeydew.Amount : int.MaxValue;
            TowerUpgradeRule.Verdict verdict = TowerUpgradeRule.Check(
                _tower.Level, _tower.MaxLevel, _tower.NextUpgradeCost, honeydew);
            _upgrade.interactable = verdict == TowerUpgradeRule.Verdict.Ready;
            _upgradeLabel.text = verdict == TowerUpgradeRule.Verdict.AtMaxLevel
                ? UiStrings.MaxLevel
                : string.Format(UiStrings.UpgradeFormat, _tower.NextUpgradeCost);

            for (int i = 0; i < _orderButtons.Length; i++)
            {
                bool current = (int)_tower.Priority == i;
                TMP_Text label = _orderButtons[i].GetComponentInChildren<TMP_Text>();
                if (label != null)
                {
                    // A mark as well as the colour, so the choice never rides on colour alone.
                    label.text = (current ? UiStrings.CurrentOrderMark : string.Empty) + OrderNames[i];
                }
                _orderButtons[i].interactable = !current;
            }
        }

        private void HandleHoneydewChanged(int amount)
        {
            Refresh();
        }

        private void HandleUpgraded(int level)
        {
            Refresh();
        }

        private void Upgrade()
        {
            if (_placer != null)
            {
                _placer.UpgradeSelectedTower();
            }
        }

        private void SetOrder(TargetingPriority order)
        {
            if (_tower != null)
            {
                _tower.Priority = order;
                Refresh();
            }
        }
    }
}
