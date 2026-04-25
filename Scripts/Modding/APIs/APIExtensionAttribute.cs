using System;

namespace RollPunk.Modding.APIs
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class APIExtensionAttribute : Attribute
    {
        public APIExtensionAttribute() { }
    }
}
