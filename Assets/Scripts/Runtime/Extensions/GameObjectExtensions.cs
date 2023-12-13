namespace HandcraftedGames.Common
{
    using UnityEngine;

    public static class GameObjectExtension
    {

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T retVal = null;
            if (gameObject.TryGetComponent<T>(out retVal))
                return retVal;
            return gameObject.AddComponent<T>();
        }
    }
}
