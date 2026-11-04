using System.Collections.Generic;
using HoldTheHill.Features.Enemies;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HoldTheHill.Features.UI
{
    /// <summary>
    /// The Queen Mantis's health bar, shown only while she walks. Marks at the thirds show where she
    /// changes shade; the fill takes her shade's colour and the label names the shade in words (and
    /// says when she prays), so it never relies on colour alone. Follows the first of
    /// <see cref="QueenMantisBoss.Live"/> through her events.
    /// </summary>
    public class QueenMantisBossBar : MonoBehaviour
    {
        [Tooltip("Everything the bar draws, on a child, hidden while no Queen Mantis walks; this component stays on to listen.")]
        [SerializeField] private GameObject _panel;

        [Tooltip("A filled Image; its fill amount is her health over her maximum.")]
        [SerializeField] private Image _fill;

        [Tooltip("Her name and what her shade does.")]
        [SerializeField] private TMP_Text _label;

        [Tooltip("Fill colour in her white shade.")]
        [SerializeField] private Color _whiteFill = new Color(0.93f, 0.93f, 0.85f);

        [Tooltip("Fill colour in her black shade; light enough to read on the dark bar.")]
        [SerializeField] private Color _blackFill = new Color(0.55f, 0.35f, 0.75f);

        private QueenMantisBoss _shown;

        private void OnEnable()
        {
            QueenMantisBoss.Appeared += HandleAppeared;
            QueenMantisBoss.Gone += HandleGone;
            ShowFirstLive();
        }

        private void OnDisable()
        {
            QueenMantisBoss.Appeared -= HandleAppeared;
            QueenMantisBoss.Gone -= HandleGone;
            Watch(null);
        }

        private void HandleAppeared(QueenMantisBoss boss)
        {
            if (_shown == null)
            {
                Watch(boss);
            }
        }

        // She has already left Live when this is raised, so the next one (if any) takes over.
        private void HandleGone(QueenMantisBoss boss)
        {
            if (boss == _shown)
            {
                ShowFirstLive();
            }
        }

        private void ShowFirstLive()
        {
            IReadOnlyList<QueenMantisBoss> live = QueenMantisBoss.Live;
            Watch(live.Count > 0 ? live[0] : null);
        }

        private void Watch(QueenMantisBoss boss)
        {
            // By reference: she may already count as destroyed, and her events still need letting go.
            if (!ReferenceEquals(_shown, null))
            {
                _shown.OnHealthChanged -= HandleHealthChanged;
                _shown.ShadeChanged -= HandleShadeChanged;
                _shown.PrayEnded -= Refresh;
            }

            _shown = boss;
            if (_shown != null)
            {
                _shown.OnHealthChanged += HandleHealthChanged;
                _shown.ShadeChanged += HandleShadeChanged;
                _shown.PrayEnded += Refresh;
            }

            if (_panel != null)
            {
                _panel.SetActive(_shown != null);
            }
            Refresh();
        }

        private void HandleHealthChanged(double current, double max)
        {
            Refresh();
        }

        private void HandleShadeChanged(QueenMantisShade shade)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (_shown == null)
            {
                return;
            }

            bool black = _shown.Shade == QueenMantisShade.Black;
            if (_fill != null)
            {
                _fill.fillAmount = _shown.HealthFraction;
                _fill.color = black ? _blackFill : _whiteFill;
            }

            if (_label != null)
            {
                string state = _shown.IsPraying ? UiStrings.MantisPraying : black ? UiStrings.MantisBlack : UiStrings.MantisWhite;
                _label.text = string.Format(UiStrings.MantisBarFormat, UiStrings.QueenMantis, state);
            }
        }
    }
}
