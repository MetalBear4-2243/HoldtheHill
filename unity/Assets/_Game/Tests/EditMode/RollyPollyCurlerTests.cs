using HoldTheHill.Features.Enemies;
using NUnit.Framework;
using UnityEngine;

namespace HoldTheHill.Tests
{
    /// <summary>
    /// The curl as an ability beside a plain <see cref="BasicEnemy"/>: the hit that curls it lands
    /// in full and shrinks, darkens and speeds it; hits while curled land at the curled share.
    /// The curl rule itself is covered by <see cref="RollyPollyCurlTests"/>.
    /// </summary>
    public class RollyPollyCurlerTests
    {
        private GameObject _object;
        private BasicEnemy _enemy;
        private RollyPollyCurler _curler;
        private SpriteRenderer _body;

        [SetUp]
        public void SetUp()
        {
            _object = new GameObject("TestRollyPolly");
            _object.transform.localScale = Vector3.one;
            _body = new GameObject("Body").AddComponent<SpriteRenderer>();
            _body.transform.SetParent(_object.transform, false);
            _body.color = Color.white;

            _enemy = _object.AddComponent<BasicEnemy>();
            _curler = _object.AddComponent<RollyPollyCurler>();
            _enemy.SetMaxHealth(100.0, resetCurrentHealth: true);
            _enemy.MoveSpeed = 1.3f;
        }

        [TearDown]
        public void TearDown()
        {
            if (_object != null)
            {
                Object.DestroyImmediate(_object);
            }
        }

        [Test]
        public void TheFirstHit_LandsInFull_AndCurlsIt()
        {
            _enemy.TakeDamage(10.0);

            Assert.That(_enemy.CurrentHealth, Is.EqualTo(90.0));
            Assert.That(_curler.Curl.IsCurled, Is.True);
            Assert.That(_curler.Curl.CurlsLeft, Is.EqualTo(2));
        }

        [Test]
        public void Curled_ItIsSmaller_Darker_AndRollsTwiceAsFast()
        {
            _enemy.TakeDamage(10.0);

            Assert.That(_object.transform.localScale.x, Is.EqualTo(0.75f).Within(0.0001f));
            Assert.That(_enemy.MoveSpeed, Is.EqualTo(2.6f).Within(0.0001f));
            Assert.That(_body.color.r, Is.EqualTo(0.55f).Within(0.0001f));
            Assert.That(_body.color.b, Is.EqualTo(0.65f).Within(0.0001f));
        }

        [Test]
        public void HitsWhileCurled_LandAtFortyPercent()
        {
            _enemy.TakeDamage(10.0);
            _enemy.TakeDamage(10.0);

            Assert.That(_enemy.CurrentHealth, Is.EqualTo(86.0).Within(0.0001));
            Assert.That(_curler.Curl.CurlsLeft, Is.EqualTo(2), "A hit while curled uses no curl.");
        }

        [Test]
        public void TheCurledShare_IsWhatTheDamageEventReports()
        {
            float applied = -1f;
            _enemy.TakeDamage(10.0);
            _enemy.OnDamaged += (enemy, hit, amount) => applied = amount;

            _enemy.TakeDamage(10.0);

            Assert.That(applied, Is.EqualTo(4f).Within(0.0001f));
        }

        [Test]
        public void ALethalFirstHit_KillsIt_CurlOrNot()
        {
            _enemy.TakeDamage(100.0);

            Assert.That(_enemy.IsDead, Is.True);
        }
    }
}
