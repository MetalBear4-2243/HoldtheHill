using UnityEngine;

namespace HoldTheHill.Features.Combat
{
    /// <summary>
    /// How a hit should be treated. Resistances and defense mechanics key off this.
    /// </summary>
    public enum DamageType
    {
        Physical,
        Magic,
        True,
        Fire,
        Poison,
        Lightning
    }

    /// <summary>
    /// One instance of damage travelling from a source to a target.
    /// Struct so hits cost no allocation during heavy waves.
    /// </summary>
    public struct DamageInfo
    {
        public float Amount;
        public GameObject Source;
        public Vector2 HitPoint;
        public DamageType Type;
        public bool IsCritical;

        public DamageInfo(float amount, GameObject source, Vector2 hitPoint, DamageType type = DamageType.Physical)
        {
            Amount = amount;
            Source = source;
            HitPoint = hitPoint;
            Type = type;
            IsCritical = false;
        }
    }
}
