using TipTapToe.Models;
using TipTapToe.Services;
using System.Diagnostics;

namespace TipTapToe
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {

            string assessment = "";
            string language = "";
            string input = "";
            int pointer = 0;

            // User language question
            Console.WriteLine("\nWelcome to TipTapToe. Let's get practicing!");
            Console.WriteLine("\nPlease choose an option from the following list:");
            Console.WriteLine("\n\t1 - Python");
            Console.WriteLine("\t2 - C++");
            Console.WriteLine("\t3 - Java");
            Console.WriteLine("\t4 - C#");
            Console.WriteLine("\t5 - JavaScript");
            Console.Write("\nWhat would you like to practice? ");

            // Read user input and set language/assessment variables
            switch (Console.ReadLine())
            {
                case "1":
                    language = "Python";
                    assessment = "name = input(\"Enter your name: \") print(\"Hello, \" + name + \"!\")";
                    break;
                case "2":
                    language = "C++";
                    assessment = "#include <iostream>; int main() { std::string n; std::cin >> n; std::cout << \"Hello, \" << n << \"!\"; }";
                    break;
                case "3":
                    language = "Java";
                    assessment = "import java.util.*; class Main { public static void main(String[] a) { System.out.println(\"Hello, \" + new Scanner(System.in).nextLine() + \"!\"); } }";
                    break;
                case "4":
                    language = "C#";
                    assessment = "using System; class P { static void Main() { Console.WriteLine(\"Hello, \" + Console.ReadLine() + \"!\"); } }";
                    break;
                case "5":
                    language = "JavaScript";
                    assessment = "console.log(\"Hello, \" + prompt(\"Enter your name:\") + \"!\");";
                    break;
                default:
                    Console.WriteLine("\nYou're off to a bad start, looks like you made a typo. Please try again.\n");
                    break;
            }

            if (assessment.Length == 0)
            {
                return;
            }

            GeminiApiService geminiApiService = new();

            // Key tracking and stopwatch 
            ConsoleKeyInfo keyInfo;
            Stopwatch stopWatch = new();

            // Log variables
            List<LogItem> keyLog = [];
            KeyInput keyInput;
            bool? result;
            TimeSpan timeStamp;


            // User typing assessment prompt
            Console.WriteLine($"\nYou choose {language}. Let's get started with your assessment! Please type the following sequence:\n");
            Console.WriteLine(assessment);

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
                        if (pointer < assessment.Length && keyInfo.KeyChar == assessment[pointer])
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
                    if (pointer == assessment.Length && input == assessment)
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

            string? practiceText = await geminiApiService.GeminiApiPostRequest(keyLog);
            if (practiceText == null)
            {
                return;
            }
            Console.WriteLine(practiceText);
        }

    }

}