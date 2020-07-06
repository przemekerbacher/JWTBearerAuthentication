using Newtonsoft.Json;
using JWTAutentication.Models;

namespace JWTAutentication.Helpers
{
    public class RefreshTokenManager<T> : IRefreshTokenManager<T> where T : class
    {
        private string _secret;

        public RefreshTokenManager(AppSettings appSettings)
        {
            _secret = appSettings.MD5_Secret;
        }

        public T EncodeToken(string token)
        {
            var crypto = new Crypto();

            var encrypted = crypto.Decrypt(token, _secret);

            var refreshToken = JsonConvert.DeserializeObject<T>(encrypted);

            return refreshToken;
        }

        public string GenerateToken(T @object)
        {
            var data = JsonConvert.SerializeObject(@object);

            var crypto = new Crypto();

            var encrypted = crypto.Encrypt(data, _secret);

            return encrypted;
        }
    }
}