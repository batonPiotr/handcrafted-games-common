namespace HandcraftedGames.Common
{
    using UnityEngine;

    public class InjectableBehaviour: MonoBehaviour
    {
        protected GODependencyInjection DIContainer;

        virtual protected void Awake()
        {
            DIContainer = gameObject.GetGODependencyInjection();
            DIContainer.ResolveDependencies(this);
        }
    }
}