using System;

namespace Client.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class CalledFromUnityAttribute : Attribute
    {
        // noting
    }
}