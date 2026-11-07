namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// Something a tower's shot or mine leaves behind that takes its strength from the tower's
    /// level (the Sap Ant's puddle). <see cref="Projectile"/> configures it the moment it is spawned,
    /// so an upgrade reaches the effect without a second prefab.
    /// </summary>
    public interface ITowerEffect
    {
        /// <summary>Takes the launching tower's ability settings. Fields left at 0 keep the prefab's own.</summary>
        void Configure(in TowerAbilitySettings ability);
    }
}
