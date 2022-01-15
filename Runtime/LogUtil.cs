namespace HandcraftedGames.Common
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    public static class MBLogUtilExtension
    {
        public static void Log(this MonoBehaviour mb, string text)
        {
            Debug.Log(mb.gameObject.name + "." + mb.GetType() + ": " + text);
        }
    }
}