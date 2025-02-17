using System.Collections;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TipTapToe
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {

            // Set up HTTP client
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );    

            // Key tracking and stopwatch 
            ConsoleKeyInfo keyInfo;
            Stopwatch stopWatch = new Stopwatch();

            // Log variables
            List<LogItem> keyLog = [];
            KeyInput keyInput;
            bool? result;
            TimeSpan timeStamp;

            // Test string, input validation and pointer
            string test = "helloworld";
            string input = "";
            int pointer = 0;

            // User prompt
            Console.WriteLine("Welcome to TipTapToe. Let's get practicing...");
            Console.WriteLine($"Please type the following word:");
            Console.WriteLine(test);

            do 
            {
                keyInfo = Console.ReadKey(false);
                if (keyInfo.Key != ConsoleKey.Escape)
                {

                    // Start stopwatch
                    if (!stopWatch.IsRunning) {
                        stopWatch.Start();
                    }

                    // Handle backspace
                    if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        // Delete character in console
                        Console.Write("\b \b");

                        // Update pointer
                        if (pointer != 0)
                        {
                            pointer -= 1;
                        }

                        // Trim last char from input validation 
                        if (input.Length > 0) {
                            string mutatedString = input.Remove(input.Length - 1);
                            input = mutatedString; 
                        }

                        // Record result and key press
                        result = null;
                        keyInput = new KeyInput(keyInfo.Key);
                    }
                    else {

                        // Record correct or incorrect result
                        if (pointer < test.Length && keyInfo.KeyChar == test[pointer])
                        {
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }

                        // Update input validation and pointer
                        keyInput = new KeyInput(keyInfo.KeyChar);
                        input += keyInput;
                        pointer += 1;
                    }

                    // Log key press
                    timeStamp = stopWatch.Elapsed;
                    var logItem = new LogItem(keyInput.ToString(), timeStamp.ToString(), result);
                    keyLog.Add(logItem);

                    // Check for correct input 
                    if (pointer == test.Length && input == test)
                    {
                        break;
                    }
                }
            }
            while (keyInfo.Key != ConsoleKey.Escape);

            // Print key press log
            PrintLog(keyLog);

            await GeminiApiRequest(client, keyLog);
        }

        private static async Task GeminiApiRequest(HttpClient client, List<LogItem> keyLog)
        {
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

        // Gemini API body class
        public class GeminiBody
        {
            [JsonPropertyName("contents")]
            public required List<ContentParts> Contents { get; set; } 

            [JsonPropertyName("generationConfig")]
            public required GenerationConfig Config { get; set; } 
        }

        // Gemini API content classes
        public class ContentParts
        {
            [JsonPropertyName("parts")]
            public required ArrayList Parts { get; set; }
        }

        public class ContentPart
        {
            [JsonPropertyName("text")]
            public required string Part { get; set; }
        }

        // Gemini API config classes
        public class GenerationConfig
        {
            [JsonPropertyName("response_mime_type")]
            public required string ResponseMimeType { get; set; }

            [JsonPropertyName("response_schema")]
            public required ConfigResponseSchema ResponseSchema { get; set; }
        }

        public class ConfigResponseSchema
        {
            [JsonPropertyName("type")]
            public required string Type { get; set; }
            
            [JsonPropertyName("items")]
            public required ConfigItems Items { get; set; }
        }

        public class ConfigItems
        {
            [JsonPropertyName("type")]
            public required string Type { get; set; }

            [JsonPropertyName("properties")]
            public required ConfigProperties Properties { get; set; }
        }

        public class ConfigProperties
        {
            [JsonPropertyName("reasoning")]
            public required ConfigType Reasoning { get; set; }

            [JsonPropertyName("practiceText")]
            public required ConfigType PracticeText { get; set; }
        }

        public class ConfigType
        {
            [JsonPropertyName("type")]
            public required string Type { get; set; }
        }

        // Function for printing log to console
        public static void PrintLog(List<LogItem> keyLog)
        {
            foreach (var item in keyLog)
            {
                Console.WriteLine(item);
            }
        }

        // Struct for storing key that was pressed
        public struct KeyInput
        {
            public char? Char { get; set; }
            public ConsoleKey? Key { get; set;}

            public KeyInput(char c)
            {
                Char = c;
                Key = null;
            }
            public KeyInput(ConsoleKey k)
            {
                Char = null;
                Key = k;
            }

            public readonly override string ToString()
            {
                return Char?.ToString() ?? Key?.ToString() ?? string.Empty;
            }
        }

        // Class for logging key press
        public class LogItem(string key, string timestamp, bool? result)
        {
            public string Key { get; set; } = key;
            public string Timestamp { get; set; } = timestamp;
            public bool? Result { get; set; } = result;

            public override string ToString()
            {
                return $"Key: {Key}, " + $"Timestamp: {Timestamp}, " + $"Result: {(Result != null ? Result : "null")}";
            }
        }
        
    }
}