namespace HoldTheHill.Features.UI
{
    /// <summary>Every string the HUD and menus show, in one place, so they can be checked and later localized.</summary>
    public static class UiStrings
    {
        public const string HoneydewFormat = "Honeydew {0}";
        public const string WaveFormat = "Wave {0} / {1}";
        public const string QueenHealth = "Queen";
        public const string NextWave = "Next Wave";
        public const string Paused = "Paused";
        public const string Resume = "Resume";
        public const string RestartLevel = "Restart level";
        public const string Home = "Home";
        public const string Victory = "The hill is held!";
        public const string Defeat = "The Queen has fallen";

        // Tower card, for the selected standing tower.
        public const string TowerLevelFormat = "Level {0} / {1}";
        public const string UpgradeFormat = "Upgrade  {0}";
        public const string MaxLevel = "Max level";
        public const string StandingOrder = "Target";
        public const string OrderFirst = "First";
        public const string OrderLast = "Last";
        public const string OrderClosest = "Closest";
        public const string CurrentOrderMark = "▸ ";
        public const string UnknownTower = "Tower";

        // Home screen. Level titles come from Core's LevelCatalogue, beside their scene ids.
        public const string GameTitle = "Hold the Hill";
        public const string Play = "Play";
        public const string Quit = "Quit";
        public const string ChooseLevel = "Choose a hill";
        public const string Back = "Back";
    }
}
