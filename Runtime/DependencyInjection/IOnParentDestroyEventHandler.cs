namespace HandcraftedGames.Common
{
    using UnityEngine;

    /// <summary>
    /// This interface works only when used in conjunction with `GODependencyInjection`
    /// </summary>
    public interface IOnParentDestroyEventHandler
    {
        void OnParentDestroy(GameObject gameObject);
    }
}