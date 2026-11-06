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
    /// Frenzy runs it counts the seconds down. A gift aimed at a tower at its last level, or a drop
    /// clicked while the slot is full, shows a short notice instead.
    /// </summary>
    public class PowerUpSlotPanel : LevelRunListener
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _label;

        [Tooltip("How long the 'already max level' and 'slot full' notices stay, in unscaled seconds.")]
        [SerializeField, Min(0.5f)] private float _noticeSeconds = 2f;

        private readonly TimedNotice _notice = new TimedNotice();
        private LevelPowerUps _powerUps;
        private bool _noticeShown;

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
            // A notice from the last run must not greet the new one.
            _notice.Clear();
            _powerUps = run.PowerUps;
            if (_powerUps != null)
            {
                _powerUps.Changed += Refresh;
                _powerUps.GiftRefused += ShowGiftRefused;
                _powerUps.SlotFull += ShowSlotFull;
            }

            Refresh();
        }

        protected override void Unbind(LevelRun run)
        {
            if (_powerUps != null)
            {
                _powerUps.Changed -= Refresh;
                _powerUps.GiftRefused -= ShowGiftRefused;
                _powerUps.SlotFull -= ShowSlotFull;
                _powerUps = null;
            }

            _notice.Clear();
            Refresh();
        }

        private void Update()
        {
            // Only the countdown and the notice change without an event. A notice that was up at
            // the last refresh gets one more, on the frame it runs out, to put the label back.
            if (_powerUps != null && (_powerUps.FrenzyActive || _noticeShown))
            {
                Refresh();
            }
        }

        private void Refresh()
        {
            string notice = _notice.TextAt(Time.unscaledTime);
            _noticeShown = notice != null;

            PowerUpKind? held = _powerUps != null ? _powerUps.Slot.Held : null;
            bool hasIcon = held != null && _powerUps.Settings != null;

            _icon.enabled = hasIcon;
            if (hasIcon)
            {
                _icon.sprite = _powerUps.Settings.IconFor(held.Value);
                _icon.color = _powerUps.Settings.ColorFor(held.Value);
            }

            _button.interactable = held != null;
            _label.text = notice ?? LabelFor(held);
        }

        private string LabelFor(PowerUpKind? held)
        {

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
            _notice.Show(UiStrings.GiftAtMaxLevel, Time.unscaledTime, _noticeSeconds);
            Refresh();
        }

        private void ShowSlotFull()
        {
            _notice.Show(UiStrings.SlotFull, Time.unscaledTime, _noticeSeconds);
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
