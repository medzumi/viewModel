using System;

namespace ViewModel
{
    [AttributeUsage(AttributeTargets.Field)]
    public class BindAttribute : Attribute
    {
        public readonly string key;

        public BindAttribute(string key)
        {
            this.key = key;
        }
    }
}