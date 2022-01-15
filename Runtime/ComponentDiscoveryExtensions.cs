namespace HandcraftedGames.Common
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class GameObjectExtensions
    {
        public static T DiscoverComponent<T>(this GameObject gameObject)
        {
            var retVal = gameObject.GetComponent<T>();
            if(retVal == null)
                retVal = gameObject.GetComponentInChildren<T>();
            if(retVal == null)
                retVal = gameObject.GetComponentInParent<T>();
            return retVal;
        }
        public static T DiscoverComponent<T>(this MonoBehaviour mono)
        {
            var retVal = mono.GetComponent<T>();
            if(retVal == null)
                retVal = mono.GetComponentInChildren<T>();
            if(retVal == null)
                retVal = mono.GetComponentInParent<T>();
            return retVal;
        }
    }
}