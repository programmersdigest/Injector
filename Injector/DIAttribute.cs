using System;

namespace programmersdigest.Injector
{
    /// <summary>
    /// Declares the annotated construtor to be the constructor used by the
    /// <see cref="DIContainer"/> for instance creation.
    /// Only one constructor in any class may be decorated with the <see cref="DIAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    public class DIAttribute : Attribute
    {
    }
}
