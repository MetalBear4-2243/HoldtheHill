using System;

namespace HoldTheHill.Features.Economy
{
    /// <summary>
    /// The one currency of a run. Plain C#, so the rules can be tested without a scene;
    /// <see cref="HoneydewWallet"/> puts one in a scene.
    /// </summary>
    /// <remarks>
    /// The amount never goes below zero: spending more than is held fails and changes nothing.
    /// </remarks>
    public sealed class Honeydew
    {
        /// <summary>Raised when the amount changes, with the new amount.</summary>
        public event Action<int> Changed;

        /// <summary>Starts with <paramref name="startingAmount"/>, or zero if that is negative.</summary>
        public Honeydew(int startingAmount)
        {
            Amount = Math.Max(0, startingAmount);
        }

        /// <summary>How much Honeydew is held.</summary>
        public int Amount { get; private set; }

        /// <summary>Adds Honeydew. Nothing happens for zero or less.</summary>
        public void Earn(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            Set(Amount + amount);
        }

        /// <summary>True if at least <paramref name="amount"/> is held.</summary>
        public bool CanAfford(int amount)
        {
            return Amount >= amount;
        }

        /// <summary>Takes Honeydew if enough is held. Zero or less always succeeds and changes nothing.</summary>
        public bool TrySpend(int amount)
        {
            if (amount <= 0)
            {
                return true;
            }

            if (!CanAfford(amount))
            {
                return false;
            }

            Set(Amount - amount);
            return true;
        }

        /// <summary>Puts the amount at an exact value, never below zero. For run start and restarts.</summary>
        public void Set(int amount)
        {
            amount = Math.Max(0, amount);
            if (amount == Amount)
            {
                return;
            }

            Amount = amount;
            Changed?.Invoke(Amount);
        }
    }
}
