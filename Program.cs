using TipTapToe.Models;
using TipTapToe.Services;
using System.Diagnostics;

namespace TipTapToe
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {

            GeminiApiService geminiApiService = new();

            // Key tracking and stopwatch 
            ConsoleKeyInfo keyInfo;
            Stopwatch stopWatch = new();

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
            foreach (var item in keyLog)
            {
                Console.WriteLine(item);
            }

            await geminiApiService.GeminiApiPostRequest(keyLog);
        }

    }

}