namespace HandcraftedGames.Common
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [ScriptOrder(-10000)]
    public class SingletonGameObject : MonoBehaviour
    {
        [SerializeField] private string ObjectID;

        private void Awake()
        {
            var allSingletons = GameObject.FindObjectsOfType<SingletonGameObject>();
            foreach(var s in allSingletons)
            {
                if(s == this)
                    continue;
                if(s.ObjectID == ObjectID)
                    Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }
    }
}