namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// An ability beside a <see cref="BasicEnemy"/> that acts the moment it dies, while it is
    /// still in the fight and before anyone hears of the death: the Centipede splits here, so its
    /// halves are registered before the win check sees the parent go.
    /// </summary>
    public interface IDeathHandler
    {
        /// <summary>Called once, at the start of the death, before <see cref="BasicEnemy.OnDied"/>.</summary>
        void BeforeDeath(BasicEnemy enemy);
    }
}
