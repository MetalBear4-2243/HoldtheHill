using HoldTheHill.Core;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HoldTheHill.Features.UI
{
    /// <summary>
    /// A developer help box on the HUD listing the keys and clicks, from <see cref="UiStrings"/>.
    /// Opens with the tutorial level only (<see cref="LevelInfo.ShowsControlsHelp"/>), since on later
    /// maps it covers the road; the ToggleHelp action (F1) hides and shows it, and once the player has
    /// used F1 the box stays as they left it. Lives under the HUD object, so it goes away with the HUD
    /// between levels and is set again when the next run's HUD comes on.
    /// </summary>
    [DisallowMultipleComponent]
    public class ControlsOverlay : MonoBehaviour
    {
        [Tooltip("The box to show or hide.")]
        [SerializeField] private GameObject _panel;

        [Tooltip("The text inside the box; filled from UiStrings on enable.")]
        [SerializeField] private TMP_Text _text;

        [Tooltip("The game's actions asset. Uses its Gameplay map: ToggleHelp.")]
        [SerializeField] private InputActionAsset _actions;

        private InputAction _toggle;
        private bool _playerChose;

        private void Awake()
        {
            if (_actions != null)
            {
                _toggle = _actions.FindActionMap("Gameplay", throwIfNotFound: true)
                    .FindAction("ToggleHelp", throwIfNotFound: true);
            }
            else
            {
                Debug.LogWarning("ControlsOverlay has no actions asset, so F1 will not hide it.", this);
            }
        }

        private void OnEnable()
        {
            if (_text != null)
            {
                _text.text = UiStrings.ControlsText;
            }

            // The HUD comes on with each run; until the player has pressed F1, the level decides.
            if (!_playerChose && _panel != null)
            {
                _panel.SetActive(OpenLevelShowsHelp());
            }

            if (_toggle != null)
            {
                _toggle.Enable();
            }
        }

        private static bool OpenLevelShowsHelp()
        {
            SceneLoader loader = SceneLoader.Current;
            string levelId = loader != null ? loader.OpenLevelId : null;

            // A level opened straight from its scene, or one not on the list, keeps the box open.
            return levelId == null || !LevelCatalogue.TryFind(levelId, out LevelInfo level) || level.ShowsControlsHelp;
        }

        private void OnDisable()
        {
            if (_toggle != null)
            {
                _toggle.Disable();
            }
        }

        private void Update()
        {
            if (_toggle != null && _toggle.WasPressedThisFrame() && _panel != null)
            {
                _panel.SetActive(!_panel.activeSelf);
                _playerChose = true;
            }
        }
    }
}
