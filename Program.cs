using System.Diagnostics;

namespace TipTapToe
{
    internal class Program
    {
        static void Main(string[] args)
        {

            // Key tracking and stopwatch 
            ConsoleKeyInfo keyInfo;
            Stopwatch stopWatch = new Stopwatch();

            // Log variables
            List<LogItem> keyLog = [];
            KeyInput keyInput;
            bool? result;
            TimeSpan timeStamp;

            // Test string, input validation and pointer
            string test = "hello";
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
                return Char.ToString() + Key.ToString();
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

        // Function for printing log to console
        public static void PrintLog(List<LogItem> keyLog)
        {
            foreach (var item in keyLog)
            {
                Console.WriteLine(item);
            }
        }

    }
}