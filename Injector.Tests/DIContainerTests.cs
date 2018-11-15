using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace programmersdigest.Injector.Tests
{
    [TestClass]
    public class DIContainerTests
    {
        #region MakeInstance

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MakeInstance_TypeIsNull_ShouldThrowArgumentNullException()
        {
            var container = new DIContainer();
            container.MakeInstance(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MakeInstance_TypeDoesNotImplementConstructor_ShouldInvalidOperationException()
        {
            var container = new DIContainer();
            container.MakeInstance(typeof(ClassWithoutConstructor));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MakeInstance_TypeHasMultipleConstructors_ShouldThrowInvalidOperationException()
        {
            var container = new DIContainer();
            container.MakeInstance(typeof(ClassWithMultipleConstructors));
        }

        [TestMethod]
        public void MakeInstance_TypeHasAmbiguousDIAttributes_ShouldUseFirstConstructor()
        {
            var container = new DIContainer();
            // The following line would fail, because the second constructor is a
            // ClassWithAmbiguousDIAttributes(string testString) in which testString
            // cannot be provided by the DIContainer.
            var instance = container.MakeInstance(typeof(ClassWithAmbiguousDIAttributes));

            Assert.IsNotNull(instance);
        }

        [DataTestMethod]
        [DataRow(typeof(ClassWithSingleConstructor))]
        [DataRow(typeof(ClassWithSingleTypeparam<string>))]
        [DataRow(typeof(ClassWithMultipleTypeparams<int, DateTime, string, object>))]
        [DataRow(typeof(ClassWithMultipleConstructorsAndDIAttribute))]
        public void MakeInstance_VariousTypes_ShouldCreateInstances(Type type)
        {
            var container = new DIContainer();
            var instance = container.MakeInstance(type);

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, type);
        }

        [TestMethod]
        public void MakeInstance_TypeRequiresRegisteredType_ShouldCreateInstance()
        {
            var container = new DIContainer();
            container.RegisterType<ClassWithSingleConstructor, ClassWithSingleConstructor>();

            var instance = container.MakeInstance(typeof(ClassExpectingInstance));

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(ClassExpectingInstance));
        }

        [TestMethod]
        public void MakeInstance_TypeRequiresRegisteredSingleton_ShouldCreateInstance()
        {
            var container = new DIContainer();
            var classWithSingleConstructor = new ClassWithSingleConstructor();
            container.RegisterSingleton<ClassWithSingleConstructor>(classWithSingleConstructor);

            var instance = container.MakeInstance(typeof(ClassExpectingInstance));

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(ClassExpectingInstance));
        }

        [TestMethod]
        public void MakeInstance_TypeRequiresRegisteredTypeWithTypeparam_ShouldCreateInstance()
        {
            var container = new DIContainer();
            container.RegisterType<ClassWithSingleTypeparam<string>, ClassWithSingleTypeparam<string>>();

            var instance = container.MakeInstance(typeof(ClassExpectingInstanceWithTypeparam));

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(ClassExpectingInstanceWithTypeparam));
        }

        [TestMethod]
        public void MakeInstance_TypeRequiresMultipleRegisteredTypes_ShouldCreateInstance()
        {
            var container = new DIContainer();
            container.RegisterType<ClassWithSingleConstructor, ClassWithSingleConstructor>();
            container.RegisterType<ClassWithSingleTypeparam<int>, ClassWithSingleTypeparam<int>>();

            var instance = container.MakeInstance(typeof(ClassExpectingMultipleInstances));

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(ClassExpectingMultipleInstances));
        }

        #endregion

        #region RegisterType

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterType_ContractIsNull_ShouldThrowArgumentNullException()
        {
            var container = new DIContainer();
            container.RegisterType(null, typeof(ClassWithSingleConstructor));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterType_TypeIsNull_ShouldThrowArgumentNullException()
        {
            var container = new DIContainer();
            container.RegisterType(typeof(ClassWithSingleConstructor), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RegisterType_TypeDoesNotImpementContract_ShouldThrowArgumentOutOfRangeException()
        {
            var container = new DIContainer();
            container.RegisterType(typeof(ClassWithSingleConstructor), typeof(ClassExpectingInstanceWithTypeparam));
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RegisterType_GenericTypeDefinitionDoesNotImplementContract_ShouldThrowArgumentOutOfRangeException()
        {
            var container = new DIContainer();
            container.RegisterType(typeof(IClassWithSingleTypeparamAndInterface<>), typeof(ClassWithSingleTypeparam<>));
        }

        [TestMethod]
        public void RegisterType_ContractHasAlreadyBeenRegistered_ShouldRegisterNewType()
        {
            var container = new DIContainer();
            
            container.RegisterType(typeof(IClassWithInterface), typeof(ClassWithInterface));
            container.RegisterType(typeof(IClassWithInterface), typeof(ClassWithInterface2));

            var actualInstance = container.Get<IClassWithInterface>();

            Assert.IsInstanceOfType(actualInstance, typeof(ClassWithInterface2));
        }

        [DataTestMethod]
        [DataRow(typeof(ClassWithSingleConstructor), typeof(ClassWithSingleConstructor))]
        [DataRow(typeof(ClassExpectingInstance), typeof(ClassExpectingInstance))]
        [DataRow(typeof(ClassExpectingInstanceWithTypeparam), typeof(ClassExpectingInstanceWithTypeparam))]
        [DataRow(typeof(IClassWithInterface), typeof(ClassWithInterface))]
        [DataRow(typeof(ClassWithSingleTypeparam<>), typeof(ClassWithSingleTypeparam<>))]
        [DataRow(typeof(IClassWithSingleTypeparamAndInterface<>), typeof(ClassWithSingleTypeparamAndInterface<>))]
        public void RegisterType_VariousContractsAndTypes_ShouldRegisterItems(Type contract, Type type)
        {
            var container = new DIContainer();
            container.RegisterType(contract, type);
        }

        #endregion

        #region RegisterSingleton

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterSingleton_ContractIsNull_ShouldThrowArgumentNullException()
        {
            var container = new DIContainer();
            container.RegisterSingleton(null, new ClassWithSingleConstructor());
        }

        [TestMethod]
        public void RegisterSingleton_InstanceIsNull_CreatesAndRegistersNewInstance()
        {
            var container = new DIContainer();
            container.RegisterSingleton(typeof(ClassWithSingleConstructor), null);

            var actualInstance = container.Get<ClassWithSingleConstructor>();

            Assert.IsNotNull(actualInstance);
            Assert.IsInstanceOfType(actualInstance, typeof(ClassWithSingleConstructor));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RegisterSingleton_TypeDoesNotImpementContract_ShouldThrowArgumentOutOfRangeException()
        {
            var container = new DIContainer();
            container.RegisterSingleton(typeof(ClassWithSingleConstructor), new ClassWithInterface());
        }

        [TestMethod]
        public void RegisterSingleton_ContractHasAlreadyBeenRegistered_ShouldRegisterNewInstance()
        {
            var container = new DIContainer();

            var instance1 = new ClassWithSingleConstructor();
            var instance2 = new ClassWithSingleConstructor();

            container.RegisterSingleton(typeof(ClassWithSingleConstructor), instance1);
            container.RegisterSingleton(typeof(ClassWithSingleConstructor), instance2);

            var actualInstance = container.Get<ClassWithSingleConstructor>();

            Assert.AreEqual(instance2, actualInstance);
        }

        [DataTestMethod]
        [DataRow(typeof(ClassWithSingleConstructor), typeof(ClassWithSingleConstructor))]
        [DataRow(typeof(IClassWithInterface), typeof(ClassWithInterface))]
        public void RegisterSingleton_VariousContractsAndTypes_ShouldRegisterItems(Type contract, Type type)
        {
            var instance = Activator.CreateInstance(type);

            var container = new DIContainer();
            container.RegisterSingleton(contract, instance);
        }

        #endregion

        #region Get

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Get_ContractIsNull_ShouldThrowArgumentNullException()
        {
            var container = new DIContainer();
            container.Get(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Get_ContractIsUnknown_ShouldThrowInvalidOperationException()
        {
            var container = new DIContainer();
            container.Get(typeof(ClassWithInterface));
        }

        [TestMethod]
        public void Get_ContractIsType_ShouldCreateInstance()
        {
            var container = new DIContainer();
            container.RegisterType<IClassWithInterface, ClassWithInterface>();

            var instance = container.Get<IClassWithInterface>();

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(ClassWithInterface));
        }

        [TestMethod]
        public void Get_ContractIsType_ShouldReturnNewInstanceEachTime()
        {
            var container = new DIContainer();
            container.RegisterType<IClassWithInterface, ClassWithInterface>();

            var instance1 = container.Get<IClassWithInterface>();
            var instance2 = container.Get<IClassWithInterface>();

            Assert.AreNotEqual(instance1, instance2);
        }

        [TestMethod]
        public void Get_ContractIsSingleton_ShouldReturnInstance()
        {
            var container = new DIContainer();
            container.RegisterSingleton<IClassWithInterface>(new ClassWithInterface());

            var instance = container.Get<IClassWithInterface>();

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(ClassWithInterface));
        }

        [TestMethod]
        public void Get_ContractIsSingleton_ShouldReturnSameInstanceEachTime()
        {
            var container = new DIContainer();
            container.RegisterSingleton<IClassWithInterface>(new ClassWithInterface());

            var instance1 = container.Get<IClassWithInterface>();
            var instance2 = container.Get<IClassWithInterface>();

            Assert.AreEqual(instance1, instance2);
        }

        [TestMethod]
        public void Get_ContractIsSingleton_ShouldReturnRegisteredInstance()
        {
            var container = new DIContainer();
            var registeredInstance = new ClassWithInterface();
            container.RegisterSingleton<IClassWithInterface>(registeredInstance);

            var instance = container.Get<IClassWithInterface>();

            Assert.AreEqual(registeredInstance, instance);
        }

        [TestMethod]
        public void Get_RegistrationIsTypeWithTypeParam_ShouldReturnSpecificTypedInstance()
        {
            var container = new DIContainer();
            container.RegisterType<ClassWithSingleTypeparam<string>>();

            var instance = container.Get<ClassWithSingleTypeparam<string>>();

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(ClassWithSingleTypeparam<string>));
        }
        
        [TestMethod]
        public void Get_TypeRequiresTypeParam_ShouldReturnSpecificTypedInstance()
        {
            var container = new DIContainer();
            container.RegisterType(typeof(IClassWithSingleTypeparamAndInterface<>), typeof(ClassWithSingleTypeparamAndInterface<>));

            var instance = container.Get<IClassWithSingleTypeparamAndInterface<ClassWithSingleConstructor>>();

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(ClassWithSingleTypeparamAndInterface<ClassWithSingleConstructor>));
        }

        [TestMethod]
        public void Get_BuilderForType_ShouldReturnBuilderForType()
        {
            var container = new DIContainer();
            container.RegisterType<ClassWithSingleConstructor>();

            var instance = container.Get<Builder<ClassWithSingleConstructor>>();

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Builder<ClassWithSingleConstructor>));
        }

        [TestMethod]
        public void Get_BuilderForSingletonType_ShouldReturnBuilderForSingletonType()
        {
            var container = new DIContainer();
            container.RegisterSingleton<ClassWithSingleConstructor>();

            var instance = container.Get<Builder<ClassWithSingleConstructor>>();

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Builder<ClassWithSingleConstructor>));
        }

        [TestMethod]
        public void Get_BuilderForTypeThatRequiresTypeParam_ShouldReturnBuilderForSpecificTypedInstance()
        {
            var container = new DIContainer();
            container.RegisterType(typeof(ClassWithSingleTypeparam<>), typeof(ClassWithSingleTypeparam<>));

            var instance = container.Get<Builder<ClassWithSingleTypeparam<int>>>();

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Builder<ClassWithSingleTypeparam<int>>));
        }

        #endregion
    }
}
