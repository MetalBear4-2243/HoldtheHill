namespace HoldTheHill.Core.Save
{
    /// <summary>
    /// Brings save data written by an older build up to <see cref="CurrentVersion"/>. When the
    /// layout changes, raise <see cref="CurrentVersion"/> and add one step to <see cref="Upgrade"/>.
    /// </summary>
    public static class SaveMigrations
    {
        /// <summary>The layout this build writes.</summary>
        public const int CurrentVersion = 1;

        /// <summary>
        /// Upgrades <paramref name="data"/> in place, one version at a time. Returns false (and
        /// leaves the data alone) when it was written by a newer build, which this one can't read safely.
        /// </summary>
        public static bool Upgrade(SaveData data)
        {
            if (data.version > CurrentVersion)
            {
                return false;
            }

            while (data.version < CurrentVersion)
            {
                switch (data.version)
                {
                    // 0: a file with no version field. Version 1 added nothing it lacks; the
                    // missing list is filled in below.
                    case 0:
                        data.version = 1;
                        break;

                    default:
                        // A gap in the steps: no way to upgrade from here.
                        return false;
                }
            }

            data.levels ??= new System.Collections.Generic.List<LevelRecord>();
            return true;
        }
    }
}
