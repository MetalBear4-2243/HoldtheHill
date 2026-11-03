using HoldTheHill.Features.Enemies;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace HoldTheHill.Features.GameFlow
{
    /// <summary>
    /// Test keys for the Editor and development builds; a release build turns them off. F5 sends a
    /// centipede down the last stretch of road to the Queen, F6 adds Honeydew, F7 sends the next wave
    /// now, F8 sends a Queen Mantis down the whole road, and F9 takes a third of the first Queen
    /// Mantis's health (one shade change per press). Sits in the UI scene and acts on <see cref="LevelRun.Current"/>.
    /// </summary>
    [DisallowMultipleComponent]
    public class DevKeys : MonoBehaviour
    {
        [Tooltip("What F5 sends at the Queen.")]
        [SerializeField] private BasicEnemy _centipedePrefab;

        [Tooltip("What F8 sends down the road.")]
        [SerializeField] private BasicEnemy _queenMantisPrefab;

        [Tooltip("How much Honeydew F6 adds.")]
        [SerializeField, Min(1)] private int _honeydewGift = 500;

        private void Awake()
        {
            // True in the Editor and in development builds.
            if (!Debug.isDebugBuild)
            {
                enabled = false;
            }
        }

        private void Update()
        {
            LevelRun run = LevelRun.Current;
            Keyboard keys = Keyboard.current;
            if (run == null || keys == null || run.Outcome != RunOutcome.Playing)
            {
                return;
            }

            if (keys.f5Key.wasPressedThisFrame)
            {
                SendCentipedeAtQueen(run);
            }

            if (keys.f6Key.wasPressedThisFrame && run.Wallet != null)
            {
                run.Wallet.Honeydew.Earn(_honeydewGift);
            }

            if (keys.f7Key.wasPressedThisFrame)
            {
                run.StartNextWave();
            }

            if (keys.f8Key.wasPressedThisFrame)
            {
                SendQueenMantis(run);
            }

            if (keys.f9Key.wasPressedThisFrame)
            {
                HurtQueenMantisByAThird();
            }
        }

        // From the first waypoint, like a spawned one, so she walks in with her escort.
        private void SendQueenMantis(LevelRun run)
        {
            EnemyPath path = FindAnyObjectByType<EnemyPath>();
            if (_queenMantisPrefab == null || path == null || path.Waypoints.Count < 2)
            {
                Debug.LogWarning("Dev keys: no Queen Mantis prefab or no road to send her down.", this);
                return;
            }

            BasicEnemy mantis = Instantiate(_queenMantisPrefab, path.Waypoints[0], Quaternion.identity);
            SceneManager.MoveGameObjectToScene(mantis.gameObject, run.gameObject.scene);
            mantis.SetPath(path.Waypoints, 0, teleportToStart: true);
        }

        // A third of her maximum, so each press crosses one shade change; a hair more, so rounding
        // never leaves her just above the line.
        private static void HurtQueenMantisByAThird()
        {
            if (QueenMantisBoss.Live.Count == 0)
            {
                Debug.LogWarning("Dev keys: no Queen Mantis is walking; F8 sends one.");
                return;
            }

            QueenMantisBoss mantis = QueenMantisBoss.Live[0];
            mantis.TakeDamage(mantis.MaxHealth / 3.0 + 0.001);
        }

        // Starts on the next-to-last waypoint, so the centipede walks only the final stretch: the
        // place to see what a hit on the arrival frame does (EN-1).
        private void SendCentipedeAtQueen(LevelRun run)
        {
            EnemyPath path = FindAnyObjectByType<EnemyPath>();
            if (_centipedePrefab == null || path == null || path.Waypoints.Count < 2)
            {
                Debug.LogWarning("Dev keys: no centipede prefab or no road to send it down.", this);
                return;
            }

            int last = path.Waypoints.Count - 1;
            BasicEnemy centipede = Instantiate(_centipedePrefab, path.Waypoints[last - 1], Quaternion.identity);
            SceneManager.MoveGameObjectToScene(centipede.gameObject, run.gameObject.scene);
            centipede.SetPath(path.Waypoints, last, teleportToStart: false);
        }
    }
}
