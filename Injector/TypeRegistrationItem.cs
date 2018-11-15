using System;

namespace programmersdigest.Injector
{
    /// <summary>
    /// Registration item for type registrations. Used by the <see cref="DIContainer"/>.
    /// </summary>
    internal class TypeRegistrationItem : IRegistrationItem
    {
        /// <summary>
        /// The type to create an instance of.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Creates a new <see cref="TypeRegistrationItem"/> holding the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to create an instance of.</param>
        public TypeRegistrationItem(Type type)
        {
            Type = type;
        }
    }
}
