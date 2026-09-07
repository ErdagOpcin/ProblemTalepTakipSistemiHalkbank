using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProblemTalepTakipSistemiHalkbank.Services
{
    public class CityDto
    {
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string sehirIsmi { get; set; } = string.Empty;

    }
    public class ApiResponse
    {
        [JsonPropertyName("data")]
        public List<CityDto> Data { get; set;} = new();
    }
    public class CityApiService
    {
        private readonly HttpClient _httpClient;

        public CityApiService(HttpClient httpclient)
        {
            _httpClient = httpclient;
        }
        public async Task<List<CityDto>> GetCitiesAsync()
        {
            // 1. API adresini belirle (Türkiye illerini veren açık kaynak JSON servisi)
            var url = "https://turkiyeapi.dev/api/v1/provinces";
            try
            {
                // 2. GET isteğini asenkron olarak gönder
                var response = await _httpClient.GetAsync(url);
                // 3. İstek başarılı mı (HTTP 200 OK) kontrol et
                if(!response.IsSuccessStatusCode)
                {
                    return new List<CityDto>();
                }
                // 4. Gelen gövdeyi (JSON metnini) oku
                var jsonString = await response.Content.ReadAsStringAsync();
                // 5. Küçük-büyük harf duyarlılığını kaldıran ayar
                var options = new JsonSerializerOptions();
                options.PropertyNameCaseInsensitive = true;

                // 6. JSON metnini CityDto listesine dönüştür (Deserialize)
                var result = JsonSerializer.Deserialize<ApiResponse>(jsonString, options);

                return result?.Data ?? new List<CityDto>();
            }
            catch(Exception)
            {
                // Ağ hatası veya bağlantı kopması durumunda uygulamanın çökmemesi için boş liste dön
                return new List<CityDto>();
            }
        }
    }
}