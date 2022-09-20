using UnityEngine.Scripting;

[assembly: Preserve]

namespace HandcraftedGames.Common
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class MonoBehaviourLogExtension
    {
        public static void Log(this MonoBehaviour mb, string text)
        {
            Debug.Log("<color=green>[" + UnityEngine.Time.frameCount + "] [" + mb.gameObject.name + "] [" + mb.GetType().Name + "]</color> " + text);
        }
        public static void LogWarning(this MonoBehaviour mb, string text)
        {
            Debug.LogWarning("<color=yellow>[" + UnityEngine.Time.frameCount + "] [" + mb.gameObject.name + "] [" + mb.GetType().Name + "]</color> " + text);
        }
        public static void LogError(this MonoBehaviour mb, string text)
        {
            Debug.LogError("<color=red>[" + UnityEngine.Time.frameCount + "] [" + mb.gameObject.name + "] [" + mb.GetType().Name + "]</color> " + text);
        }
    }

    public static class GenericLogExtension
    {
        public static void Log(this System.Object obj, string text)
        {
            Debug.Log("<color=green>[" + UnityEngine.Time.frameCount + "] [" + obj.GetType().Name + "]</color> " + text);
        }
        public static void LogWarning(this System.Object obj, string text)
        {
            Debug.LogWarning("<color=yellow>[" + UnityEngine.Time.frameCount + "] [" + obj.GetType().Name + "]</color> " + text);
        }
        public static void LogError(this System.Object obj, string text)
        {
            Debug.LogError("<color=red>[" + UnityEngine.Time.frameCount + "] [" + obj.GetType().Name + "]</color> " + text);
        }
    }
}