using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using TipTapToe.Models;

namespace TipTapToe.Services
{
    public class GeminiApiService
    {
        public async Task<string?> GeminiApiPostRequest(List<LogItem> keyLog)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );  
            string? geminiApiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            if (geminiApiKey == null)
            {
                Console.Error.WriteLine("Unable to find Gemini API Key.");
                return null;
            }
            string geminiUri = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={geminiApiKey}";
            GeminiRequest requestBody = new()
            {
                Contents =
                [
                    new RequestParts
                    {
                        Parts =
                        [
                            new RequestPart
                            {
                                Part = "I am building a typing practice tool for programmers. Below is a log of a user's keystrokes while typing code. The log includes key presses, timing data, and potential error patterns. Based on this log, generate a new body of text for the user to practice typing. The text should focus on improving their weaknesses, reinforcing their strengths, and simulating real-world programming tasks."
                            },
                            new RequestPart
                            {
                                Part = JsonSerializer.Serialize(keyLog)
                            }
                        ]
                    }
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
            using HttpResponseMessage response = await client.PostAsync(geminiUri, requestBodyJson);
            string responseContent = await response.Content.ReadAsStringAsync();
            var responseContentJson = JsonSerializer.Deserialize<GeminiResponse>(responseContent);
            if (responseContentJson == null)
            {
                Console.WriteLine("Error deserializing Gemini API response");
                return null;
            }
            string responseData = responseContentJson.Candidates[0].Content.Parts[0].Part;
            var responseDataJson = JsonSerializer.Deserialize<List<ResponseText>>(responseData);
            if (responseDataJson == null || responseDataJson.Count == 0)
            {
                Console.WriteLine("Error deserializing Gemini API response data");
                return null;
            }
            string geminiPracticeText = responseDataJson[0].PracticeText;
            return geminiPracticeText;
        }
    }
}