using HoldTheHill.Core.Animation;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// What is left where an enemy died: a copy of its body playing the Death row once, then fading
    /// away. Made at run time by <see cref="EnemyHurtAndDeath"/>, in the enemy's own scene, so it
    /// unloads with the level. Drawn just under the living enemies. It is only a picture: nothing
    /// targets it and it is not in the enemy registry.
    /// </summary>
    [DisallowMultipleComponent]
    public class EnemyDeathEffect : MonoBehaviour
    {
        private SpriteRenderer _renderer;
        private Sprite[] _frames;
        private float _frameSeconds;
        private float _fadeSeconds;
        private Color _color;
        private float _clock;
        private int _shown = -1;

        /// <summary>
        /// Leaves a death at <paramref name="body"/>'s place, size, turn and colour, playing
        /// <paramref name="frames"/>. Does nothing without a body or frames.
        /// </summary>
        public static EnemyDeathEffect Spawn(SpriteRenderer body, Sprite[] frames, float frameSeconds, float fadeSeconds, Scene scene)
        {
            if (body == null || frames == null || frames.Length == 0 || !Application.isPlaying)
            {
                return null;
            }

            var holder = new GameObject("Enemy Death");
            if (scene.IsValid() && scene.isLoaded)
            {
                SceneManager.MoveGameObjectToScene(holder, scene);
            }

            Transform from = body.transform;
            holder.transform.SetPositionAndRotation(from.position, from.rotation);
            holder.transform.localScale = from.lossyScale;

            var renderer = holder.AddComponent<SpriteRenderer>();
            renderer.sharedMaterial = body.sharedMaterial;
            renderer.sortingLayerID = body.sortingLayerID;
            renderer.sortingOrder = body.sortingOrder - 1;
            renderer.flipX = body.flipX;
            renderer.flipY = body.flipY;
            renderer.color = body.color;
            renderer.sprite = frames[0];

            EnemyDeathEffect effect = holder.AddComponent<EnemyDeathEffect>();
            effect._renderer = renderer;
            effect._frames = frames;
            effect._frameSeconds = Mathf.Max(0.01f, frameSeconds);
            effect._fadeSeconds = Mathf.Max(0f, fadeSeconds);
            effect._color = body.color;
            return effect;
        }

        /// <summary>
        /// How opaque the body is <paramref name="clock"/> seconds after death: solid while the row
        /// plays, then fading to nothing over <paramref name="fadeSeconds"/>.
        /// </summary>
        public static float Opacity(float clock, float rowSeconds, float fadeSeconds)
        {
            if (clock <= rowSeconds)
            {
                return 1f;
            }

            if (fadeSeconds <= 0f)
            {
                return 0f;
            }

            return Mathf.Clamp01(1f - (clock - rowSeconds) / fadeSeconds);
        }

        private void Update()
        {
            _clock += Time.deltaTime;
            float row = SpriteRow.Length(_frameSeconds, _frames.Length);

            int index = SpriteRow.FrameAt(_clock, _frameSeconds, _frames.Length);
            if (index >= 0 && index != _shown)
            {
                _shown = index;
                _renderer.sprite = _frames[index];
            }

            float opacity = Opacity(_clock, row, _fadeSeconds);
            if (opacity <= 0f)
            {
                Destroy(gameObject);
                return;
            }

            Color color = _color;
            color.a *= opacity;
            _renderer.color = color;
        }
    }
}
