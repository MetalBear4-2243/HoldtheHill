using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// The Fire Ant's flame stream, as a picture: while the tower is attacking it sends a steady run
    /// of <see cref="FlamePuff"/>s at its target, spread over the tower's cone, each reaching about the
    /// tower's range. The damage is the tower's own melee swing in a cone (<see cref="Tower.ConeDegrees"/>).
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Tower))]
    public class FlameJet : MonoBehaviour
    {
        [Tooltip("The puff prefab (a FlamePuff with its loop).")]
        [SerializeField] private FlamePuff _puffPrefab;

        [Tooltip("Puffs sent per second while flaming.")]
        [SerializeField, Min(1f)] private float _puffsPerSecond = 16f;

        [Tooltip("How fast a puff flies, in world units per second.")]
        [SerializeField, Min(0.1f)] private float _puffSpeed = 6f;

        [Tooltip("Share of the cone the puffs are spread over (1 = the whole cone).")]
        [SerializeField, Range(0f, 1f)] private float _spreadShare = 0.6f;

        [Tooltip("How far from the tower's centre the puffs start, in world units.")]
        [SerializeField, Min(0f)] private float _startDistance = 0.25f;

        private Tower _tower;
        private float _flamingFor;
        private float _untilNextPuff;

        private void Awake()
        {
            _tower = GetComponent<Tower>();
        }

        private void OnEnable()
        {
            _tower.Attacked += HandleAttacked;
        }

        private void OnDisable()
        {
            _tower.Attacked -= HandleAttacked;
        }

        // Each swing keeps the stream going a little past the next one, so it does not flicker.
        private void HandleAttacked()
        {
            _flamingFor = _tower.FireInterval * 1.5f + 0.05f;
        }

        private void Update()
        {
            if (_flamingFor <= 0f)
            {
                return;
            }

            _flamingFor -= Time.deltaTime;
            Tower tower = _tower;
            if (_puffPrefab == null || tower.CurrentTarget == null || !tower.CanAct)
            {
                return;
            }

            _untilNextPuff -= Time.deltaTime;
            while (_untilNextPuff <= 0f)
            {
                _untilNextPuff += 1f / _puffsPerSecond;
                SendPuff(tower);
            }
        }

        private void SendPuff(Tower tower)
        {
            Vector3 here = transform.position;
            Vector3 aim = tower.CurrentTarget.transform.position - here;
            aim.z = 0f;
            if (aim.sqrMagnitude < 0.0001f)
            {
                return;
            }

            float halfSpread = tower.ConeDegrees * 0.5f * _spreadShare;
            float angle = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg + Random.Range(-halfSpread, halfSpread);
            Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0f);

            FlamePuff puff = Instantiate(_puffPrefab, here + direction * _startDistance, Quaternion.identity);
            SceneManager.MoveGameObjectToScene(puff.gameObject, gameObject.scene);
            float reach = Mathf.Max(0.1f, tower.Range - _startDistance);
            puff.Launch(direction * _puffSpeed, reach / _puffSpeed);
        }
    }
}
