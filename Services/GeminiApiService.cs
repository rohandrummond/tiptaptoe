using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using TipTapToe.Models;

namespace TipTapToe.Services
{
    public class GeminiApiService
    {
        public async Task GeminiApiPostRequest(List<LogItem> keyLog)
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
                return;
            }
            string geminiUri = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={geminiApiKey}";
            GeminiBody geminiBody = new()
            {
                Contents =
                [
                    new ContentParts
                    {
                        Parts =
                        [
                            new ContentPart
                            {
                                Part = "I am building a typing practice tool for programmers. Below is a log of a user's keystrokes while typing code. The log includes key presses, timing data, and potential error patterns. Based on this log, generate a new body of text for the user to practice typing. The text should focus on improving their weaknesses, reinforcing their strengths, and simulating real-world programming tasks."
                            },
                            new ContentPart
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
            string jsonString = JsonSerializer.Serialize(geminiBody, new JsonSerializerOptions{
                WriteIndented = true
            });
            Console.WriteLine(jsonString);
            JsonContent geminiJsonBody = JsonContent.Create(geminiBody);
            using HttpResponseMessage response = await client.PostAsync(geminiUri, geminiJsonBody);
            string responseContentString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContentString);
        }
    }
}