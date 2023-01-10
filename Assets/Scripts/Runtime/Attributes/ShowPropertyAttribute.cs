namespace HandcraftedGames.Common
{
    using UnityEngine;
    using System;

    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class ShowPropertyAttribute : Attribute
    {
        public string Label;
        public string Tooltip;
        public bool Readonly;

        public ShowPropertyAttribute(string label)
        {
            this.Label = label;
        }
    }
}