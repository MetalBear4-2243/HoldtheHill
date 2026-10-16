using HoldTheHill.Core;
using UnityEngine;
using UnityEngine.UI;

namespace HoldTheHill.Features.Levels
{
    /// <summary>A level select button: clicking it asks the <see cref="SceneLoader"/> to open its level.</summary>
    [RequireComponent(typeof(Button))]
    public class LevelButton : MonoBehaviour
    {
        [Tooltip("The level's id from the LevelCatalogue, e.g. L00_TutorialHill. Names the scene pair to load.")]
        [SerializeField] private string _levelId;

        private Button _button;

        /// <summary>The level this button opens.</summary>
        public string LevelId => _levelId;

        private void Awake()
        {
            _button = GetComponent<Button>();
            if (!LevelCatalogue.TryFind(_levelId, out _))
            {
                Debug.LogError($"'{_levelId}' is not in the LevelCatalogue, so this button opens nothing.", this);
            }
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(Open);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(Open);
        }

        private void Open()
        {
            SceneLoader loader = SceneLoader.Current;
            if (loader != null)
            {
                loader.LoadLevel(_levelId);
            }
            else
            {
                Debug.LogError("No SceneLoader is loaded, so the level can't be opened.", this);
            }
        }
    }
}
