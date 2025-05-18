using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
public class SentimentResult
{
    public string Label { get; set; }
    public double Score { get; set; }
}

namespace E_Commerce.Services
{
    public class HuggingFaceService : IHuggingFaceService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _modelUrl;

        public HuggingFaceService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiKey = configuration["HuggingFace:ApiToken"]!;
            _modelUrl = "https://api-inference.huggingface.co/models/nlptown/bert-base-multilingual-uncased-sentiment";
        }

        public async Task<string> AnalyzeSentimentAsync(string inputText)
        {
            var requestBody = new { inputs = inputText };
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.PostAsync(_modelUrl, content);

            if (!response.IsSuccessStatusCode)
                return $"Error: {response.StatusCode}";

            var responseJson = await response.Content.ReadAsStringAsync();

            // ✅ عشان يتطابق مع label و score من الـ JSON
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // ✅ تعديل الـ Deserialize بالشكل الصحيح
            var parsed = JsonSerializer.Deserialize<List<List<SentimentResult>>>(responseJson, options);

            var bestResult = parsed?.FirstOrDefault()?.OrderByDescending(r => r.Score).FirstOrDefault();

            return bestResult != null ? bestResult.Label : "No result";
        }


    }
}