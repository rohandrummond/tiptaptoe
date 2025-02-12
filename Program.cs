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

            // Test string and pointer
            string test = "hello";
            int pointer = 0;

            // User prompt
            Console.WriteLine("Welcome to TipTapToe. Let's get practicing...");
            Console.WriteLine($"Please type the following word:");
            Console.WriteLine(test);

            do 
            {
                keyInfo = Console.ReadKey(true);
                if (keyInfo.Key != ConsoleKey.Escape)
                {

                    // Start stopwatch
                    if (!stopWatch.IsRunning) {
                        stopWatch.Start();
                    }

                    // Correct
                    if (keyInfo.KeyChar == test[pointer])
                    {
                        Console.WriteLine("Correct");
                        keyInput = new KeyInput(keyInfo.KeyChar);
                        result = true;
                        pointer ++;
                    }

                    // Backspace
                    else if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        Console.WriteLine("Backspace");
                        keyInput = new KeyInput(keyInfo.Key);
                        result = null;
                    }

                    // Incorrect
                    else
                    {
                        Console.WriteLine("Incorrect");
                        keyInput = new KeyInput(keyInfo.KeyChar);
                        result = false;
                    }

                    // Log key press
                    timeStamp = stopWatch.Elapsed;
                    var logItem = new LogItem(keyInput.ToString(), timeStamp.ToString(), result);
                    keyLog.Add(logItem);
                }
            }
            while (keyInfo.Key != ConsoleKey.Escape);
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