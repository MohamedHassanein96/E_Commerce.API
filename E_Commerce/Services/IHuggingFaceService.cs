namespace E_Commerce.Services
{
    public interface IHuggingFaceService
    {
        Task<string> AnalyzeSentimentAsync(string inputText);
    }
}
