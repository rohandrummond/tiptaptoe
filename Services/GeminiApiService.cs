using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using TipTapToe.Models;

namespace TipTapToe.Services
{
    public class GeminiApiService
    {

        // Constructor for creating HttpClient instance and Gemini URI 
        private readonly HttpClient httpClient;
        private readonly string geminiUri;
        public GeminiApiService()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );  
            string? geminiApiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            if (geminiApiKey == null)
            {
                throw new InvalidOperationException("Unable to find Gemini API Key.");
            }
            geminiUri = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={geminiApiKey}";
        }

        // API template for making request and handling response
        public async Task<string> MakeApiRequest(RequestParts requestParts)
        {
            try
            {
                GeminiRequest requestBody = new()
                {
                    Contents =
                    [
                        requestParts
                    ],
                    Config = new GenerationConfig
                    {
                        ResponseMimeType = "application/json",
                        ResponseSchema = new ConfigResponseSchema
                        {
                            Type = "ARRAY",
                            Items = new ConfigItems
                            {
                                Type = "OBJECT",
                                Properties = new ConfigProperties
                                {
                                    Reasoning = new ConfigType{
                                        Type = "STRING"
                                    },
                                    PracticeText = new ConfigType{
                                        Type = "STRING"
                                    }
                                }
                            }
                        }
                    }
                };
                JsonContent requestBodyJson = JsonContent.Create(requestBody);
                using HttpResponseMessage response = await httpClient.PostAsync(geminiUri, requestBodyJson);
                string responseContent = await response.Content.ReadAsStringAsync();
                var responseContentJson = JsonSerializer.Deserialize<GeminiResponse>(responseContent);
                if (responseContentJson == null)
                {
                    throw new InvalidOperationException("Error deserializing Gemini API response");
                }
                string responseData = responseContentJson.Candidates[0].Content.Parts[0].Part;
                var responseDataJson = JsonSerializer.Deserialize<List<ResponseText>>(responseData);
                if (responseDataJson == null || responseDataJson.Count == 0)
                {
                    throw new InvalidOperationException("Error deserializing data in Gemini API response");
                }
                string geminiPracticeText = responseDataJson[0].PracticeText;
                return geminiPracticeText;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        // Prompt for initial assessment
        public async Task<string> Assess(List<LogItem> keyLog, string language)
        {

            RequestParts requestParts = new()
            {
                Parts =
                [
                    new RequestPart
                    {
                        Part = $"I am building a console app for programmers to practice typing. Below is a log of a users keystrokes while typing {language} code, including key presses, timing data, and common errors. Based on this log, generate a unique 40-character, single-line sequence that targets their weaknesses while simulating real-world programming syntax."
                    },
                    new RequestPart
                    {
                        Part = JsonSerializer.Serialize(keyLog)
                    }
                ]
            };
            string geminiPracticeText = await MakeApiRequest(requestParts);
            return geminiPracticeText;
        }

        // Prompt for generating additional practice sequences
        public async Task<string> ContinuePractice(string language)
        {
            RequestParts requestParts = new()
            {
                Parts =
                [
                    new RequestPart
                    {
                        Part = $"Generate a single-line, 40-character unique sequence of characters for typing practice, focusing on programming syntax and patterns commonly used in {language}. The string should be a single line that includes a variety of characters such as letters, numbers, punctuation, and operators, mimicking code structure and real-world coding scenarios."
                    }
                ]
            };
            string geminiPracticeText = await MakeApiRequest(requestParts);
            return geminiPracticeText;
        }
    }
}