#if UNITY_EDITOR
using System.Collections;
using HoldTheHill.Features.Enemies;
using HoldTheHill.Features.Towers;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace HoldTheHill.Tests
{
    /// <summary>
    /// A copy of an enemy that already wears an overlay (a Centipede half without a split prefab
    /// is a copy of its dying parent) must animate its own overlay, not show one frozen frame.
    /// Batch-mode frames are tiny, so every wait is on the clock. Editor only: the prefab is
    /// loaded through the AssetDatabase.
    /// </summary>
    public class StatusOverlayPlayModeTests
    {
        private const string EnemyPrefab = "Assets/_Game/Features/Enemies/Prefabs/Enemy_Basic.prefab";
        private const string Kind = "Poison";

        private Scene _scene;
        private Texture2D _texture;
        private Sprite[] _frames;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            _scene = SceneManager.CreateScene("StatusOverlayPlayModeTests");
            SceneManager.SetActiveScene(_scene);
            _texture = new Texture2D(2, 1);
            _frames = new[]
            {
                Sprite.Create(_texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f),
                Sprite.Create(_texture, new Rect(1f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f),
            };
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return SceneManager.UnloadSceneAsync(_scene);
            for (int i = 0; i < _frames.Length; i++)
            {
                Object.Destroy(_frames[i]);
            }
            Object.Destroy(_texture);
        }

        [UnityTest]
        public IEnumerator ACopiedEnemy_AnimatesTheOverlayItCopied()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(EnemyPrefab);
            Assert.That(prefab, Is.Not.Null, "Enemy_Basic.prefab is missing.");
            BasicEnemy parent = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity).GetComponent<BasicEnemy>();
            StatusOverlay.Show(parent, Kind, _frames, 0.05f, 5f);
            yield return null;

            BasicEnemy copy = Object.Instantiate(parent.gameObject, Vector3.right, Quaternion.identity).GetComponent<BasicEnemy>();
            yield return null;
            StatusOverlay.Show(copy, Kind, _frames, 0.05f, 5f);

            Transform holder = copy.transform.Find("Overlay " + Kind);
            Assert.That(holder, Is.Not.Null, "The copy should carry the overlay child it copied.");
            Assert.That(copy.GetComponentsInChildren<StatusOverlay>(true).Length, Is.EqualTo(1), "Show must reuse the copied overlay, not add a second.");
            var renderer = holder.GetComponent<SpriteRenderer>();

            bool sawFirst = false;
            bool sawSecond = false;
            float until = Time.time + 2f;
            while (!(sawFirst && sawSecond) && Time.time < until)
            {
                yield return null;
                sawFirst |= renderer.sprite == _frames[0];
                sawSecond |= renderer.sprite == _frames[1];
            }

            Assert.That(sawFirst && sawSecond, Is.True, "The copy's overlay should step through both frames.");
        }
    }
}
#endif
