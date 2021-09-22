using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersonProfile_DF.Business
{	
    public static class TokenHelper
    {
        public static string GenerateUserToken(Guid tokenKey, params string[] tokenValues)
        {
            // Any individual value in "tokenValues" should NOT contain a comma because we'll be using COMMA as an indicator to separate the tokenValues.
            // Comma is not a base64 character so we can use it to separate the values on receiving end.

            if (tokenValues == null || !tokenValues.Any())
            {
                throw new Exception("Authentication token cannot be generated.");
            }

            string stringRepresentatingAllData = string.Join(",", tokenValues);

            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = tokenKey.ToByteArray();
            byte[] values = Encoding.UTF8.GetBytes(stringRepresentatingAllData);
            byte[] tokenData = new byte[time.Length + key.Length + values.Length];

            Buffer.BlockCopy(time, 0, tokenData, 0, time.Length);
            Buffer.BlockCopy(key, 0, tokenData, time.Length, key.Length);
            Buffer.BlockCopy(values, 0, tokenData, time.Length + key.Length, values.Length);

            return Convert.ToBase64String(tokenData.ToArray());
        }

        public static bool IsUserTokenValid(string token, out Guid key, out List<string> tokenValues)
        {
            if(string.IsNullOrEmpty(token))
            {
                tokenValues = null;
                key = Guid.Empty;
                return false;
            }

            bool isValidToken = false;
            tokenValues = null;
            key = Guid.Empty;

            byte[] tokenData = Convert.FromBase64String(token);
            DateTime expiryTime = DateTime.FromBinary(BitConverter.ToInt64(tokenData.Take(8).ToArray(), 0));

            if (expiryTime > DateTime.UtcNow.AddSeconds(-901))
            {
                tokenValues = new List<string>();

                key = new Guid(tokenData.Skip(8).Take(16).ToArray());
                var stringRepresentingAllTokenValues = Encoding.UTF8.GetString(tokenData.Skip(24).ToArray());

                tokenValues = stringRepresentingAllTokenValues.Split(',').ToList<string>();

                isValidToken = true;
            }

            return isValidToken;
        }
    }
}

