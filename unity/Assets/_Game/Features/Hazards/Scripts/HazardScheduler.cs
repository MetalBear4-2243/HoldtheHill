using System.Collections;
using UnityEngine;

namespace HoldTheHill.Features.Hazards
{
    /// <summary>
    /// Spawns the level's hazards on their waves, from its <see cref="LevelHazardSchedule"/>.
    /// Lives in the gameplay scene beside the spawner and listens to its wave-started event.
    /// </summary>
    [DisallowMultipleComponent]
    public class HazardScheduler : MonoBehaviour
    {
        [Tooltip("The level's spawner. Its wave-started event triggers the schedule.")]
        [SerializeField] private EnemySpawner _spawner;

        [Tooltip("What happens on which wave.")]
        [SerializeField] private LevelHazardSchedule _schedule;

        /// <summary>The schedule in use, for the Inspector and tests.</summary>
        public LevelHazardSchedule Schedule => _schedule;

        private void OnEnable()
        {
            if (_spawner != null)
            {
                _spawner.onWaveStarted.AddListener(HandleWaveStarted);
            }
        }

        private void OnDisable()
        {
            if (_spawner != null)
            {
                _spawner.onWaveStarted.RemoveListener(HandleWaveStarted);
            }

            // A level going away takes its pending hazards with it.
            StopAllCoroutines();
        }

        private void HandleWaveStarted(int waveNumber, int totalWaves)
        {
            if (_schedule == null)
            {
                return;
            }

            for (int i = 0; i < _schedule.Entries.Count; i++)
            {
                LevelHazardSchedule.Entry entry = _schedule.Entries[i];
                if (entry.Wave == waveNumber && entry.Hazard != null)
                {
                    StartCoroutine(SpawnAfter(entry));
                }
            }
        }

        // Scaled time, so a paused run holds its hazards back too.
        private IEnumerator SpawnAfter(LevelHazardSchedule.Entry entry)
        {
            if (entry.Delay > 0f)
            {
                yield return new WaitForSeconds(entry.Delay);
            }

            Instantiate(entry.Hazard, new Vector3(entry.Position.x, entry.Position.y, 0f), Quaternion.identity, transform);
        }
    }
}
