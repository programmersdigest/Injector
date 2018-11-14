﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace programmersdigest.Injector
{
    public class DIContainer
    {
        private ConcurrentDictionary<Type, DIItem> _registry = new ConcurrentDictionary<Type, DIItem>();

        public object RegisterSingleton(Type contract, object instance = null)
        {
            if (instance == null)
            {
                instance = MakeInstance(contract);
            }

            var item = new DIItem(instance, true);
            _registry.AddOrUpdate(contract, item, (key, old) => item);

            return instance;
        }

        public TContract RegisterSingleton<TContract>(TContract instance = null) where TContract : class
        {
            return (TContract)RegisterSingleton(typeof(TContract), instance);
        }

        public TContract RegisterSingleton<TContract, TType>() where TContract : class where TType : TContract
        {
            var instance = MakeInstance(typeof(TType)) as TContract;
            return RegisterSingleton(instance);
        }

        public void RegisterType(Type contract, Type type)
        {
            var item = new DIItem(type, false);
            _registry.AddOrUpdate(contract, item, (key, old) => item);
        }

        public void RegisterType<TContract, TType>() where TContract : class where TType : TContract
        {
            RegisterType(typeof(TContract), typeof(TType));
        }

        public void RegisterType<TType>() where TType : class
        {
            RegisterType(typeof(TType), typeof(TType));
        }

        public object Get(Type contract)
        {
            if (contract.GetTypeInfo().IsGenericType && contract.GetGenericTypeDefinition() == typeof(Builder<>))
            {
                return Activator.CreateInstance(contract, this);
            }

            if (!_registry.TryGetValue(contract, out var item))
            {
                throw new InvalidOperationException($"Unknown contract: \"{contract.Name}\". Make sure the contract has been registered before retrieving an instance.");
            }

            if (item.IsSingleton)
            {
                return item.Value;
            }
            else
            {
                var type = item.Value as Type;
                if (type == null)
                {
                    throw new InvalidOperationException($"The contract \"{contract.Name}\" returned an invalid item. Please check your registrations.");
                }

                return MakeInstance(type);
            }
        }

        public TContract Get<TContract>() where TContract : class
        {
            return (TContract)Get(typeof(TContract));
        }

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

        public T MakeInstance<T>() where T : class
        {
            return (T)MakeInstance(typeof(T));
        }
    }
}
