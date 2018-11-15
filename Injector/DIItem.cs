namespace programmersdigest.Injector
{
    /// <summary>
    /// Internal storage class for items registered in the <see cref="DIContainer"/>.
    /// </summary>
    internal struct DIItem
    {
        /// <summary>
        /// If set to <c>true</c>, declares the registration to be a singleton registration,
        /// where the instance is held instead of a general <see cref="Type"/>.
        /// </summary>
        internal bool IsSingleton { get; }

        /// <summary>
        /// The value held by this <see cref="DIItem"/>.
        /// If <see cref="IsSingleton"/> is <c>true</c>, <see cref="Value"/> is an instance
        /// of the registered type. If <see cref="IsSingleton"/> is <c>false</c>, a
        /// <see cref="Type"/> is held from which an instance can be created as necessary.
        /// </summary>
        internal object Value { get; }

        /// <summary>
        /// Creates a new <see cref="DIItem"/> for use in the <see cref="DIContainer"/>.
        /// </summary>
        /// <param name="value">The value this <see cref="DIItem"/> holds.</param>
        /// <param name="isSingleton">Declares the registration to be a singleton registration.</param>
        public DIItem(object value, bool isSingleton)
        {
            Value = value;
            IsSingleton = isSingleton;
        }
    }
}
