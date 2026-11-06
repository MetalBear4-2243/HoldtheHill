using UnityEngine;
using UnityEngine.UI;

namespace HoldTheHill.Features.UI
{
    /// <summary>
    /// The home screen in the <c>Home</c> scene: a title page with Play and Quit, and the level
    /// select page Play opens. The level buttons on that page carry their own
    /// <see cref="Levels.LevelButton"/>; this only switches pages and quits.
    /// </summary>
    public class HomeScreen : MonoBehaviour
    {
        [Tooltip("The page with the title, Play and Quit. Shown when the scene opens.")]
        [SerializeField] private GameObject _titlePage;

        [Tooltip("The page with one button per level and Back.")]
        [SerializeField] private GameObject _levelSelectPage;

        [SerializeField] private Button _play;
        [SerializeField] private Button _quit;
        [SerializeField] private Button _back;

        private void OnEnable()
        {
            _play.onClick.AddListener(ShowLevelSelect);
            _quit.onClick.AddListener(QuitGame);
            _back.onClick.AddListener(ShowTitle);
            ShowTitle();
        }

        private void OnDisable()
        {
            _play.onClick.RemoveListener(ShowLevelSelect);
            _quit.onClick.RemoveListener(QuitGame);
            _back.onClick.RemoveListener(ShowTitle);
        }

        private void ShowTitle()
        {
            _titlePage.SetActive(true);
            _levelSelectPage.SetActive(false);
        }

        private void ShowLevelSelect()
        {
            _titlePage.SetActive(false);
            _levelSelectPage.SetActive(true);
        }

        private static void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
