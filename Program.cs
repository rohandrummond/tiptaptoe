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
            char key;
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
                        result = true;
                        pointer ++;
                    }

                    // Backspace
                    else if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        Console.WriteLine("Backspace");
                        result = null;
                    }

                    // Incorrect
                    else
                    {
                        Console.WriteLine("Incorrect");
                        result = false;
                    }

                    // Log key press
                    key = keyInfo.KeyChar;
                    timeStamp = stopWatch.Elapsed;
                    var logItem = new LogItem
                    {
                        Key = key,
                        Timestamp = timeStamp,
                        Result = result,
                    };
                    keyLog.Add(logItem);
                }
            }
            while (keyInfo.Key != ConsoleKey.Escape);
        }

        public class LogItem
        {
            public required char Key { get; set; }
            public required TimeSpan Timestamp { get; set; }
            public bool? Result { get; set; }
        }
        
    }
}