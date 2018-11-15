namespace programmersdigest.Injector.Tests
{
    internal class ClassWithoutConstructor
    {
        private ClassWithoutConstructor()
        {
        }
    }

    internal class ClassWithMultipleConstructors
    {
        public ClassWithMultipleConstructors()
        {
        }

        public ClassWithMultipleConstructors(string testString)
        {
        }
    }

    internal class ClassWithSingleConstructor
    {
        public ClassWithSingleConstructor()
        {
        }
    }

    internal class ClassWithSingleTypeparam<T>
    {
        public ClassWithSingleTypeparam()
        {
        }
    }

    internal class ClassWithMultipleTypeparams<T, U, V, W>
    {
        public ClassWithMultipleTypeparams()
        {
        }
    }

    internal class ClassExpectingInstance
    {
        public ClassExpectingInstance(ClassWithSingleConstructor test)
        {
        }
    }

    internal class ClassExpectingMultipleInstances
    {
        public ClassExpectingMultipleInstances(ClassWithSingleConstructor test1, ClassWithSingleConstructor test2, ClassWithSingleTypeparam<int> test3)
        {
        }
    }

    internal class ClassExpectingInstanceWithTypeparam
    {
        public ClassExpectingInstanceWithTypeparam(ClassWithSingleTypeparam<string> test)
        {
        }
    }

    internal interface IClassWithInterface
    {
    }

    internal class ClassWithInterface : IClassWithInterface
    {
    }

    internal class ClassWithInterface2 :IClassWithInterface
    {
    }
}
