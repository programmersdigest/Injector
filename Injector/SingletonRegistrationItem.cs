namespace programmersdigest.Injector
{
    /// <summary>
    /// Registration item for singleton registrations. Used by the <see cref="DIContainer"/>.
    /// </summary>
    internal class SingletonRegistrationItem : IRegistrationItem
    {
        /// <summary>
        /// The singleton instance.
        /// </summary>
        public object Instance { get; }

        /// <summary>
        /// Creates a new <see cref="SingletonRegistrationItem"/> holding the given
        /// singleton <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The singleton instance.</param>
        public SingletonRegistrationItem(object instance)
        {
            Instance = instance;
        }
    }
}
