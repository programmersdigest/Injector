namespace programmersdigest.Injector {
    internal struct DIItem {
        internal bool IsSingleton { get; }
        internal object Value { get; }

        public DIItem(object value, bool isSingleton) {
            Value = value;
            IsSingleton = isSingleton;
        }
    }
}
