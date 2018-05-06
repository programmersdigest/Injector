[![Build status](https://ci.appveyor.com/api/projects/status/vtldv38jemgkhmvf/branch/master?svg=true)](https://ci.appveyor.com/project/programmersdigest/injector/branch/master)
# Injector
A simple dependency injection container with focus on the bare necessities.

## Description
This project implements a simple and easy to use dependency injection container. 

**Disclaimer**: This project is provided "as is". It may be incomplete and/or faulty. The author(s) of this project cannot be held responsible for any damage occurring due to using this software.

## Features
- Constructor injection only (see why bellow)
- Small and clean interface, easy to use
- Provides singleton and instance support

## Main concepts
Every application contains a single dependency injection container (diContainer), which holds registrations. A registration is a mapping from a _contract_ to either a _type_ or a _singleton_. A type is created on demand, whenever the container is asked to provide an instance of this type. In contrast, the container holds and always provides the _same instance_ of a singleton.

Typically, requesting a contract from the container happens implicitly: the dependent type defines required contracts as parameters of its constructor. On creation of an instance of the dependent type (via the diContainer), all dependencies get resolved and provided to the constructor. This is a recursive process: if a dependency has to be created, it in turn gets its dependencies provided.

Injector provides _constructor injection only_. This is a conscious decision. Constructor injection explicitly _documents and enforces_ all dependencies of a type. It is not possible to create an instance of a type without providing all dependencies.
Other forms of injection (e.g. property injection) do _not enforce_ all dependencies to be provided. Whenever a dependent type accesses one of its dependencies, it is required to check whether the dependency is actually available. If done incorrectly, this will lead to errors which are impossible when using constructor injection.
For this reason, we accept somewhat bulkier code (constructor parameters, member variables, assignments in the constructor vs. simple auto-properties) in exchange for stability and reduced error potential.

## Usage
Grab the latest version from NuGet https://www.nuget.org/packages/programmersdigest.Injector

**Initialization**
```
// Initialize the container
var diContainer = new DIContainer();

// Register singletons ...
// ... with default initialization
diContainer.RegisterSingleton<ISingleton1, MySingleton1>();
// ... providing additional constructor arguments
diContainer.RegisterSingleton<ISingleton2, MySingleton2>("The answer is", 42);
// ... or inserting an already initialized instance
var mySingleton3 = new MySingleton3();
diContainer.RegisterSingleton<ISingleton1>(mySingleton);

// Register instances ...
diContainer.RegisterType<IService, Service>();

// The contract can be an interface or a type ...
diContainer.RegisterType<IInterfaceContract, Impl1>();
diContainer.RegisterType<TypeContract, Impl2>();

// ... and can be omitted when the contract matches the registered type
diContainer.RegisterType<Impl3>();    // Impl3 will be registered under the contract Impl3
```

**Requesting contracts**
```
// Contracts can be requested directly (but there is usually no need to do so)
var service = diContainer.Get<IService>();

// Typically, dependencies are requested as parameters of a types constructor
public DependentType(IService service, ISingleton2 mySingleton) { ... }

// These dependencies get resolved automatically, if the type is in turn registered in the diContainer
diContainer.Register<DependentType>();
var dependentType = diContainer.Get<DependentType>();

// If a dependent type should not be registered in the diContainer, there is a
// way to create arbitrary types from the container (provided all dependencies are
// registered in the container.
var unregisteredType = diContainer.MakeInstance<UnregisteredType>();
```

**Ambiguous constructors**
```
// If there are multiple constructors, the dependency injection constructor has to manually be defined
// via the DIAttribute
public MultiCtor() { ... }

[DI]
public MultiCtor(IService service) { ... }
```

**Lazy loading**
```
// Dependencies can be lazy-loaded using the Builder<T>
public LazyCtor(Builder<IService> serviceBuilder) { ... }

// The diContainer will automatically resolve these dependencies and provide a Builder<IService> if
// the contract IService is known.
// The actual instance is created only when it is requested from the builder
var service = serviceBuilder.Build();
```

## Todos
- Add comments on public members
