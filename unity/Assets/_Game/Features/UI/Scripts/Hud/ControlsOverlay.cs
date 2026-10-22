using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HoldTheHill.Features.UI
{
    /// <summary>
    /// A developer help box on the HUD listing the keys and clicks, from <see cref="UiStrings"/>.
    /// Shown by default; the ToggleHelp action (F1) hides and shows it. Lives under the HUD object,
    /// so it goes away with the HUD between levels.
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
                _text.text = UiStrings.ControlsTitle + "\n" + string.Join("\n", UiStrings.ControlsLines);
            }

            if (_toggle != null)
            {
                _toggle.Enable();
            }
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
            }
        }
    }
}
