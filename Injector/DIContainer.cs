using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace programmersdigest.Injector
{
    /// <summary>
    /// Container for dependency injection (DI).
    /// </summary>
    public class DIContainer
    {
        private ConcurrentDictionary<Type, IRegistrationItem> _registry = new ConcurrentDictionary<Type, IRegistrationItem>();

        /// <summary>
        /// Registers the given <paramref name="type"/> under the given
        /// <paramref name="contract"/>. The <paramref name="contract"/> should
        /// typically be an interface which the <paramref name="type"/> has to implement.
        /// If <paramref name="contract"/> has already been registered, the old
        /// registration gets overwritten.
        /// </summary>
        /// <param name="contract">
        /// The contract under which the <paramref name="type"/> can be retrieved.
        /// </param>
        /// <param name="type">
        /// The type to create when an instance of <paramref name="contract"/> is requested.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// In case <paramref name="contract"/> or <paramref name="type"/> are null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// In case <paramref name="type"/> does not inherit of or implement <paramref name="contract"/>.
        /// </exception>
        public void RegisterType(Type contract, Type type)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!contract.IsAssignableFrom(type))
            {
                throw new ArgumentOutOfRangeException(nameof(type), $"Type {type.Name} must inherit of or implement contract {contract.Name}.");
            }

            var item = new TypeRegistrationItem(type);
            _registry.AddOrUpdate(contract, item, (key, old) => item);
        }

        /// <summary>
        /// Registers the given <typeparamref name="TType"/> under the given
        /// <typeparamref name="TContract"/>. The <typeparamref name="TContract"/> should
        /// typically be an interface which the <typeparamref name="TType"/> has to implement.
        /// If <typeparamref name="TContract"/> has already been registered, the old
        /// registration gets overwritten.
        /// </summary>
        /// <typeparam name="TContract">
        /// The contract under which an instance of <typeparamref name="TType"/> can be retrieved.
        /// </typeparam>
        /// <typeparam name="TType">
        /// The type to create when an instance of <typeparamref name="TContract"/> is requested.
        /// </typeparam>
        public void RegisterType<TContract, TType>() where TContract : class where TType : TContract
        {
            RegisterType(typeof(TContract), typeof(TType));
        }

        /// <summary>
        /// Registers the given <typeparamref name="TType"/> under the given the contract
        /// <typeparamref name="TType"/>.
        /// If <paramref name="TType"/> has already been registered, the old
        /// registration gets overwritten.
        /// </summary>
        /// <typeparam name="TType">
        /// The type to create when an instance of <typeparamref name="TType"/> is requested.
        /// </typeparam>
        public void RegisterType<TType>() where TType : class
        {
            RegisterType(typeof(TType), typeof(TType));
        }

        /// <summary>
        /// Registers the given <paramref name="instance"/> under the given
        /// <paramref name="contract"/>. The <paramref name="contract"/> should
        /// typically be an interface which <paramref name="instance"/> has to implement.
        /// If <paramref name="instance"/> is not provided, a new instance gets created via <see cref="MakeInstance(Type)"/>.
        /// If <paramref name="contract"/> has already been registered, the old
        /// registration gets overwritten.
        /// </summary>
        /// <param name="contract">
        /// The contract under which <paramref name="instance"/> can be retrieved.
        /// </param>
        /// <param name="instance">
        /// Optional: The singleton instance to return when a <paramref name="contract"/> is requested.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// In case <paramref name="contract"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// In case <paramref name="instance"/> does not inherit of or implement <paramref name="contract"/>.
        /// </exception>
        public object RegisterSingleton(Type contract, object instance = null)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (instance == null)
            {
                instance = MakeInstance(contract);
            }

            if (!contract.IsAssignableFrom(instance.GetType()))
            {
                throw new ArgumentOutOfRangeException(nameof(instance), $"Instance of type {instance.GetType().Name} must inherit of or implement contract {contract.Name}.");
            }

            var item = new SingletonRegistrationItem(instance);
            _registry.AddOrUpdate(contract, item, (key, old) => item);

            return instance;
        }

        /// <summary>
        /// Registers the given <paramref name="instance"/> under the given
        /// <typeparamref name="TContract"/>. The <typeparamref name="TContract"/> should
        /// typically be an interface which <paramref name="instance"/> has to implement.
        /// If <paramref name="instance"/> is not provided, a new instance gets created via <see cref="MakeInstance(Type)"/>.
        /// If <typeparamref name="TContract"/> has already been registered, the old
        /// registration gets overwritten.
        /// </summary>
        /// <typeparam name="TContract">
        /// The contract under which <paramref name="instance"/> can be retrieved.
        /// </typeparam>
        /// <param name="instance">
        /// Optional: The singleton instance to return when a <typeparamref name="TContract"/> is requested.
        /// </param>
        public TContract RegisterSingleton<TContract>(TContract instance = null) where TContract : class
        {
            return (TContract)RegisterSingleton(typeof(TContract), instance);
        }

        /// <summary>
        /// Creates a new instance of <typeparamref name="TType"/> and registers it
        /// under the given <typeparamref name="TContract"/>. The <typeparamref name="TContract"/>
        /// should typically be an interface which <paramref name="instance"/> has to implement.
        /// If <typeparamref name="TContract"/> has already been registered, the old
        /// registration gets overwritten.
        /// </summary>
        /// <typeparam name="TContract">
        /// The contract under which the instance of <typeparamref name="TType"/> can be retrieved.
        /// </typeparam>
        /// <typeparam name="TType">
        /// The type of which to create a singleton instance to return when a <typeparamref name="TContract"/> is requested.
        /// </typeparam>
        public TContract RegisterSingleton<TContract, TType>() where TContract : class where TType : TContract
        {
            var instance = MakeInstance(typeof(TType)) as TContract;
            return RegisterSingleton(instance);
        }

        /// <summary>
        /// Tries to get or create an instance of the type registered under the given
        /// <paramref name="contract"/>.
        /// </summary>
        /// <param name="contract">The contract of which to retrieve an instance.</param>
        /// <returns>The prepared instance of <paramref name="contract"/>.</returns>
        /// <exception cref="ArgumentNullException">In case <paramref name="contract"/> is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// In case no registration can be found for <paramref name="contract"/> or the registration
        /// is invalid.
        /// </exception>
        public object Get(Type contract)
        {
            if (contract == null)
            {
                throw new ArgumentNullException(nameof(contract));
            }

            if (contract.GetTypeInfo().IsGenericType && contract.GetGenericTypeDefinition() == typeof(Builder<>))
            {
                return Activator.CreateInstance(contract, this);
            }

            if (!_registry.TryGetValue(contract, out var item))
            {
                throw new InvalidOperationException($"Unknown contract: \"{contract.Name}\". Make sure the contract has been registered before retrieving an instance.");
            }

            switch (item)
            {
                case SingletonRegistrationItem singletonRegistrationItem:
                    return singletonRegistrationItem.Instance;
                case TypeRegistrationItem typeRegistrationItem:
                    return MakeInstance(typeRegistrationItem.Type);
                default:
                    throw new InvalidOperationException($"The contract \"{contract.Name}\" resulted in an invalid registration.");
            }
        }

        /// <summary>
        /// Tries to get or create an instance of the type registered under the given
        /// <typeparamref name="TContract"/>.
        /// </summary>
        /// <typeparam name="TContract">The contract of which to retrieve an instance.</typeparam>
        /// <returns>The prepared instance of <typeparamref name="TContract"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// In case no registration can be found for <typeparamref name="TContract"/> or the registration
        /// is invalid.
        /// </exception>
        public TContract Get<TContract>() where TContract : class
        {
            return (TContract)Get(typeof(TContract));
        }

        /// <summary>
        /// Creates an instance of the given <paramref name="type"/> using the registered
        /// dependencies for constructor injection.
        /// </summary>
        /// <param name="type">The type of which to create an instance.</param>
        /// <returns>The prepared instance of <paramref name="type"/>.</returns>
        /// <exception cref="InvalidOperationException">In case no matching constructor can be found.</exception>
        /// <seealso cref="Activator.CreateInstance(Type)">For more information on exceptions that may occur.</seealso>
        public object MakeInstance(Type type)
        {
            // Try to find a constructor with DIAttribute or use default constructor.
            var ctors = type.GetTypeInfo().DeclaredConstructors
                            .Where(c => c.IsPublic && !c.IsStatic);

            ConstructorInfo ctor = null;
            if (ctors.Count() == 1)
            {
                ctor = ctors.First();
            }
            else
            {
                ctor = ctors.FirstOrDefault(c => c.GetCustomAttributes(typeof(DIAttribute), false).Any());
            }

            if (ctor == null)
            {
                throw new InvalidOperationException($"Unable to find a matching constructor for contract \"{type.Name}\". Please annotate the constructor to be used with the {nameof(DIAttribute)}");
            }

            // Fill constructor parameters.
            var args = ctor.GetParameters();
            var values = new List<object>();

            foreach (var arg in args)
                values.Add(Get(arg.ParameterType));

            return Activator.CreateInstance(type, values.ToArray());
        }

        /// <summary>
        /// Creates an instance of <typeparamref name="T"/> using the registered
        /// dependencies for constructor injection.
        /// </summary>
        /// <typeparam name="T">The type of which to create an instance.</typeparam>
        /// <returns>The prepared instance of <typeparamref name="T"/>.</returns>
        public T MakeInstance<T>() where T : class
        {
            return (T)MakeInstance(typeof(T));
        }
    }
}
