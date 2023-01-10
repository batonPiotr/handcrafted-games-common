namespace HandcraftedGames.Common
{
    using UnityEngine;

    public class SerializeFieldThroughPropertyAttribute : PropertyAttribute
    {
        public string PropertyName;
        public bool DrawItself = false;

        public SerializeFieldThroughPropertyAttribute(string propertyName)
        {
            this.PropertyName = propertyName;
        }
    }
}