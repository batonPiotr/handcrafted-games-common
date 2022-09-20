namespace HandcraftedGames.Common.Serialization
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using UnityEngine;

    public class UnityFileStreamProvider : IFileStreamProvider
    {
        public IReusableStream CreateStreamWithFilenameSalt(string filenameSalt)
        {
            string hashedFilenameSalt = "";
            using (SHA256 sha256Hash = SHA256.Create())
            {
                var salt = System.Text.Encoding.UTF8.GetBytes(filenameSalt);
                var hash = sha256Hash.ComputeHash(salt);
                hashedFilenameSalt = BitConverter.ToString(hash).Replace("-", string.Empty);
            }

            var combinedPath = Path.Combine(Application.persistentDataPath, hashedFilenameSalt + ".json");
            this.Log("Combined path: " + combinedPath);
            return new GenericReusableStream(() => File.Open(combinedPath, FileMode.OpenOrCreate, FileAccess.ReadWrite));
        }
    }
}