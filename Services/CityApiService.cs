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


    public class DistrictDto
    {
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string ilceIsmi { get; set; } = string.Empty;

        [JsonPropertyName("provinceId")]
        public int ProvinceId { get; set; }
    }


    public class ApiResponse
    {
        [JsonPropertyName("data")]
        public List<CityDto> Data { get; set; } = new();
    }


    public class DistrictApiResponse
    {
        [JsonPropertyName("data")]
        public List<DistrictDto> Data { get; set; } = new();
    }


    public class CityApiService
    {
        private readonly HttpClient _httpClient;

        public CityApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<List<CityDto>> GetCitiesAsync()
        {
            var url =
                "https://api.turkiyeapi.dev/v2/provinces?limit=100";

            try
            {
                var response =
                    await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return new List<CityDto>();
                }

                var jsonString =
                    await response.Content.ReadAsStringAsync();

                var options =
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                var result =
                    JsonSerializer.Deserialize<ApiResponse>(
                        jsonString,
                        options
                    );

                return result?.Data
                    ?? new List<CityDto>();
            }
            catch (Exception)
            {
                return new List<CityDto>();
            }
        }


        public async Task<List<DistrictDto>> GetDistrictsAsync(
            int provinceId)
        {
            var url =
                $"https://api.turkiyeapi.dev/v2/districts?provinceId={provinceId}&limit=1000&sort=name";

            try
            {
                var response =
                    await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return new List<DistrictDto>();
                }

                var jsonString =
                    await response.Content.ReadAsStringAsync();

                var options =
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                var result =
                    JsonSerializer.Deserialize<DistrictApiResponse>(
                        jsonString,
                        options
                    );

                return result?.Data
                    ?? new List<DistrictDto>();
            }
            catch (Exception)
            {
                return new List<DistrictDto>();
            }
        }
    }
}