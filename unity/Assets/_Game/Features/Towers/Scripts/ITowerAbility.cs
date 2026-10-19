namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// A component that does a tower's work in place of its own attack, such as laying mines or
    /// slowing enemies. The <see cref="Tower"/> beside it keeps its numbers (range, damage, interval)
    /// for the ability to read, but does not hit or shoot by itself.
    /// </summary>
    public interface ITowerAbility
    {
    }
}
