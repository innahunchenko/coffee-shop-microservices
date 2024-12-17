using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text;

namespace ShoppingCart.API.Services
{
    public class SecureTokenService : ISecureTokenService
    {
        private readonly byte[] key;
        public SecureTokenService(IConfiguration configuration)
        {
            string base64Key = configuration["SecureTokenSettings:CartKey"];
            if (string.IsNullOrEmpty(base64Key))
            {
                throw new InvalidOperationException("SecureToken key is missing.");
            }

            key = Convert.FromBase64String(base64Key);
        }

        public string EncodeCartId(Guid cartId)
        {
            var plainText = Encoding.UTF8.GetBytes(cartId.ToString());
            using var hmac = new HMACSHA256(key);
            var signature = hmac.ComputeHash(plainText);

            var combined = new byte[plainText.Length + signature.Length];
            Buffer.BlockCopy(plainText, 0, combined, 0, plainText.Length);
            Buffer.BlockCopy(signature, 0, combined, plainText.Length, signature.Length);

            return WebEncoders.Base64UrlEncode(combined);
        }

        public Guid DecodeCartId(string encodedCartId)
        {
            var combined = WebEncoders.Base64UrlDecode(encodedCartId);
            var plainTextLength = combined.Length - 32; // HMACSHA256 produces a 32-byte signature

            var plainText = new byte[plainTextLength];
            var signature = new byte[32];

            Buffer.BlockCopy(combined, 0, plainText, 0, plainTextLength);
            Buffer.BlockCopy(combined, plainTextLength, signature, 0, 32);

            using var hmac = new HMACSHA256(key);
            var expectedSignature = hmac.ComputeHash(plainText);

            if (!CryptographicOperations.FixedTimeEquals(signature, expectedSignature))
            {
                throw new InvalidOperationException("Invalid token");
            }

            return Guid.Parse(Encoding.UTF8.GetString(plainText));
        }
    }
}
