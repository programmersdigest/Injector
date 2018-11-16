namespace programmersdigest.Injector
{
    /// <summary>
    /// Creates an instance of type <typeparamref name="TContract"/> on demand.
    /// Used in conjunction with the <see cref="DIContainer"/>: Any class can
    /// request an <see cref="IBuilder{T}"/> instead of directly requesting T. This
    /// allows for the class to instantiate an instance of T at any point using
    /// the mechanisms of the <see cref="DIContainer"/>.
    /// </summary>
    /// <typeparam name="TContract">The contract this builder may instantiate instances for.</typeparam>
    public interface IBuilder<TContract> where TContract : class
    {
        /// <summary>
        /// Creates an instance of <typeparamref name="TContract"/> using the
        /// <see cref="DIContainer"/>.
        /// </summary>
        /// <returns>The prepared instance of <typeparamref name="TContract"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// In case no registration can be found for <typeparamref name="TContract"/> or the registration
        /// is invalid.
        /// </exception>
        TContract Build();
    }
}
