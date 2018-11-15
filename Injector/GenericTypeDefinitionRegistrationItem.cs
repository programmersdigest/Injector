using System;

namespace programmersdigest.Injector
{
    /// <summary>
    /// Registration item for generic type registrations. Used by the <see cref="DIContainer"/>.
    /// </summary>
    internal class GenericTypeDefinitionRegistrationItem : IRegistrationItem
    {
        /// <summary>
        /// The generic type definition of which to create a specific generic type that
        /// can be instanciated.
        /// </summary>
        public Type GenericTypeDefinition { get; }

        /// <summary>
        /// Creates a new <see cref="GenericTypeDefinitionRegistrationItem"/> holding
        /// the given <paramref name="genericTypeDefinition"/>.
        /// </summary>
        /// <param name="genericTypeDefinition"></param>
        public GenericTypeDefinitionRegistrationItem(Type genericTypeDefinition)
        {
            GenericTypeDefinition = genericTypeDefinition;
        }
    }
}
