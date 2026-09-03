using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace ProblemTalepTakipSistemiHalkbank.Services
{
    public class PasswordLeakService
    {
        private readonly HttpClient _httpClient;

        public PasswordLeakService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> IsPasswordLeakedAsync(string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            byte[] hashBytes = SHA1.HashData(passwordBytes);

            string hash = Convert.ToHexString(hashBytes);

            string prefix = hash.Substring(0, 5);
            string suffix = hash.Substring(5);

            var response = await _httpClient.GetStringAsync(
                $"https://api.pwnedpasswords.com/range/{prefix}");

            var lines = response.Split('\n');

            foreach (var line in lines)
            {
                var parts = line.Trim().Split(':');

                if (parts.Length == 2 &&
                    parts[0].Equals(suffix, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}