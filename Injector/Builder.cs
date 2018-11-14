namespace programmersdigest.Injector
{
    public class Builder<T> where T : class
    {
        private DIContainer _diContainer;

        public Builder(DIContainer diContainer)
        {
            _diContainer = diContainer;
        }

        public T Build()
        {
            return _diContainer.Get<T>();
        }
    }
}
