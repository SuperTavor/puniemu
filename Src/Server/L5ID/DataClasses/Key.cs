using Newtonsoft.Json;
using System.Security.Cryptography;
namespace Puniemu.Src.Server.L5ID.DataClasses
{
    public class Key
    {
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("signature")]
        public string Signature { get; set; }

        [JsonIgnore]
        public const string UDKEY_PREFIX = "d-";
        [JsonIgnore]
        public const string GDKEY_PREFIX = "g-";
        public Key(string? keyValue = null, string ? prefix = null)
        {
            //Autogenerate key
            if (keyValue == null)
            {
                if(prefix != null)
                    keyValue = GenerateKey(prefix);
            }
            Value = keyValue!;
            //Signature can be empty, game doesn't care (presumably until you start managing different l5id accounts, which we won't do)
            Signature = "";
        }

        public static string GenerateKey(string prefix)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomBytes = new byte[16];
                rng.GetBytes(randomBytes);
                string hexString = BitConverter.ToString(randomBytes).Replace("-", "").ToLowerInvariant();
                return prefix + hexString;
            }
        }
    }
}
