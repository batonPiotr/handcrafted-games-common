namespace HandcraftedGames.Common.Serialization
{
    using HandcraftedGames.Common.Dependencies.SimpleJSON;

    public interface ISerializable
    {
        /// <summary>
        /// It provides an object that is serializable with Unity's JsonUtility
        /// </summary>
        JSONNode Serialize();

        /// <summary>
        /// Populate data and restore the state based on an object that has been provided when serialized.
        /// </summary>
        /// <param name="data"></param>
        void Deserialize(JSONNode data);
    }
}