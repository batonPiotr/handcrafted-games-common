namespace HandcraftedGames.Common.Serialization
{
    public interface IIdentifiable
    {
        /// <summary>
        /// Idenitifies an object in HandcraftedGames serialization tools.
        /// </summary>
        /// <para>null: When trying to save with null, it will be generated and saved as new object</para>
        /// <para>When value is there and it exists in the data repo, the object will be overwritten. Otherwise, it will be saved as new object. </para>
        int? Id { get; set; }
    }
}