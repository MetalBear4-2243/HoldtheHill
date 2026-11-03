using UnityEngine;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// The numbers and art behind the Queen Mantis's mechanics: the pray pause, the white heal, the
    /// black raise, her summons and her looks. Data, not code (UNITY_RULES 32). Her health, speed,
    /// damage and bounty stay on the prefab's <see cref="BasicEnemy"/> and <c>Bounty</c>, like every enemy's.
    /// </summary>
    [CreateAssetMenu(fileName = "QueenMantisData", menuName = "Hold the Hill/Queen Mantis Data")]
    public class QueenMantisData : ScriptableObject
    {
        [Header("Shade changes")]
        [Tooltip("Seconds she stands still at each shade change. She can't heal or raise meanwhile: the opening to hit back.")]
        [Min(0f)] public float PraySeconds = 1f;

        [Header("White: healing")]
        [Tooltip("Hurt enemies within this many world units of her are healed. Never herself.")]
        [Min(0f)] public float HealRadius = 3f;

        [Tooltip("Health given to each of them per heal.")]
        [Min(0f)] public float HealAmount = 6f;

        [Tooltip("Seconds between heals. A pulse ring shows each one.")]
        [Min(0.05f)] public float HealInterval = 1f;

        [Header("Black: raising husks")]
        [Tooltip("Bodies within this many world units of her can be raised. Their markers show inside this reach.")]
        [Min(0f)] public float RaiseRadius = 4f;

        [Tooltip("At most this many husks per raise.")]
        [Min(1)] public int HusksPerRaise = 3;

        [Tooltip("Seconds between raises.")]
        [Min(0.05f)] public float RaiseInterval = 4f;

        [Tooltip("Seconds the chosen bodies glow before they rise. A shade change in this time stops the raise.")]
        [Min(0f)] public float RaiseWindUp = 0.6f;

        [Tooltip("A husk's health as a share of the Mantis Spawn prefab's; 0.5 is half.")]
        [Range(0.05f, 1f)] public float HuskHealthFraction = 0.5f;

        [Tooltip("A body older than this many seconds can no longer be raised; its marker fades out over this time.")]
        [Min(0f)] public float DeathMemorySeconds = 12f;

        [Tooltip("How many recent deaths she remembers at most.")]
        [Min(1)] public int DeathLogCapacity = 12;

        [Header("Summons")]
        [Tooltip("The Mantis Spawn prefab she summons, and raises husks as.")]
        public BasicEnemy SpawnPrefab;

        [Tooltip("Mantis Spawn she brings with her when she walks in.")]
        [Min(0)] public int SummonOnEntry = 3;

        [Tooltip("Mantis Spawn she calls at each shade change, at the start of the pray pause.")]
        [Min(0)] public int SummonOnShadeChange = 2;

        [Tooltip("World units from her where summoned Mantis Spawn appear.")]
        [Min(0f)] public float SummonRingRadius = 0.7f;

        [Tooltip("If on, summoned Mantis Spawn walk at her speed, so they stay inside her heal and raise reach.")]
        public bool EscortsKeepHerPace = true;

        [Header("Looks")]
        [Tooltip("Her sprite in the white shade.")]
        public Sprite WhiteSprite;

        [Tooltip("Her sprite in the black shade.")]
        public Sprite BlackSprite;

        [Tooltip("In-between frames from black to white, played forwards into white and backwards into black during the pray pause.")]
        public Sprite[] ShiftFrames = new Sprite[0];

        [Tooltip("How fast her body turns to face the road, in degrees per second.")]
        [Min(0f)] public float TurnDegreesPerSecond = 360f;

        [Tooltip("A ring sprite for the heal pulse and the body markers.")]
        public Sprite RingSprite;

        [Tooltip("The heal pulse at its start; it grows out to the heal radius and fades.")]
        public Color HealPulseColor = new Color(0.95f, 1f, 0.8f, 0.8f);

        [Tooltip("A body she can raise; fades as the body gets older.")]
        public Color MarkerColor = new Color(0.35f, 0.15f, 0.5f, 0.85f);

        [Tooltip("A chosen body during the wind-up, just before it rises.")]
        public Color WindUpColor = new Color(0.75f, 0.45f, 1f, 1f);

        [Tooltip("Sprite colour of a raised husk, so it reads as one.")]
        public Color HuskTint = new Color(0.45f, 0.38f, 0.55f);

        /// <summary>Her sprite in <paramref name="shade"/>.</summary>
        public Sprite SpriteFor(QueenMantisShade shade)
        {
            return shade == QueenMantisShade.Black ? BlackSprite : WhiteSprite;
        }
    }
}
