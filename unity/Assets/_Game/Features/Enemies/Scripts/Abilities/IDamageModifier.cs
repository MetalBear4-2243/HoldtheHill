namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// An ability beside a <see cref="BasicEnemy"/> that changes a hit before it lands: the Rhino
    /// Beetle's shield soaks part of it, the Rolly Polly's curl lets only a share through.
    /// <see cref="BasicEnemy"/> finds these on its own object and asks each in component order.
    /// </summary>
    public interface IDamageModifier
    {
        /// <summary>Takes a hit of <paramref name="damage"/> and returns how much of it reaches health. Zero means none: no event, no flinch.</summary>
        double ModifyDamage(double damage);
    }
}
