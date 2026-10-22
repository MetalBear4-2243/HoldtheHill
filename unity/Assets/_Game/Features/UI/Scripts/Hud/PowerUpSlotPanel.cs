using HoldTheHill.Features.GameFlow;
using HoldTheHill.Features.PowerUps;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HoldTheHill.Features.UI
{
    /// <summary>
    /// The HUD's one power-up slot: a button with the held power-up's icon and name. Clicking it uses
    /// the power-up through the run. While a Royal Gift waits for a tower it says so; while a Swarm
    /// Frenzy runs it counts the seconds down.
    /// </summary>
    public class PowerUpSlotPanel : LevelRunListener
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _label;

        [Tooltip("How long the 'already max level' notice stays, in unscaled seconds.")]
        [SerializeField, Min(0.5f)] private float _noticeSeconds = 2f;

        private LevelPowerUps _powerUps;
        private float _noticeUntil;

        protected override void OnEnable()
        {
            _button.onClick.AddListener(Use);
            Refresh();
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _button.onClick.RemoveListener(Use);
        }

        protected override void Bind(LevelRun run)
        {
            _powerUps = run.PowerUps;
            if (_powerUps != null)
            {
                _powerUps.Changed += Refresh;
                _powerUps.GiftRefused += ShowGiftRefused;
            }

            Refresh();
        }

        protected override void Unbind(LevelRun run)
        {
            if (_powerUps != null)
            {
                _powerUps.Changed -= Refresh;
                _powerUps.GiftRefused -= ShowGiftRefused;
                _powerUps = null;
            }

            Refresh();
        }

        private void Update()
        {
            // Only the countdown and the notice change without an event.
            if (_powerUps != null && (_powerUps.FrenzyActive || Time.unscaledTime < _noticeUntil))
            {
                Refresh();
            }
        }

        private void Refresh()
        {
            PowerUpKind? held = _powerUps != null ? _powerUps.Slot.Held : null;
            bool hasIcon = held != null && _powerUps.Settings != null;

            _icon.enabled = hasIcon;
            if (hasIcon)
            {
                _icon.sprite = _powerUps.Settings.IconFor(held.Value);
                _icon.color = _powerUps.Settings.ColorFor(held.Value);
            }

            _button.interactable = held != null;
            _label.text = LabelFor(held);
        }

        private string LabelFor(PowerUpKind? held)
        {
            if (_powerUps != null && Time.unscaledTime < _noticeUntil)
            {
                return UiStrings.GiftAtMaxLevel;
            }

            if (held == PowerUpKind.RoyalGift)
            {
                return _powerUps.PickingGiftTarget ? UiStrings.PickTowerForGift : UiStrings.RoyalGift;
            }

            if (held == PowerUpKind.SwarmFrenzy)
            {
                return UiStrings.SwarmFrenzy;
            }

            if (_powerUps != null && _powerUps.FrenzyActive)
            {
                return string.Format(UiStrings.FrenzyActiveFormat, Mathf.CeilToInt(_powerUps.FrenzySecondsLeft));
            }

            return UiStrings.PowerUpEmpty;
        }

        private void ShowGiftRefused()
        {
            _noticeUntil = Time.unscaledTime + _noticeSeconds;
            Refresh();
        }

        private void Use()
        {
            if (Run != null)
            {
                Run.UsePowerUp();
            }
        }
    }
}
