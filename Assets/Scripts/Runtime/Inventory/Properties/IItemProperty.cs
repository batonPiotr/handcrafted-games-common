namespace HandcraftedGames.Inventory
{
    using System;
    using UnityEngine;
    using HandcraftedGames.Common.Serialization;

    public interface IItemProperty
    {
        public string PropertyId { get; }
        public string Name { get; }

        public IItemProperty Clone();
    }

    public interface IHasRuntimeData: ISerializable
    {
    }

    public interface IItemPropertyWithRuntimeData: IItemProperty, IHasRuntimeData {}
}