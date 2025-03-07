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
            Console.WriteLine("\t6 - Testing");
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
                case "6":
                    language = "JavaScript";
                    assessment = "console.log(\"hello\");";
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
            Console.WriteLine($"\nYou chose {language}. Let's get started with your assessment! Please type the following sequence:\n");
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
                        stopWatch.Stop();
                        stopWatch.Reset();
                        assessment = "";
                        input = "";
                        pointer = 0;
                        break;
                    }
                }
                else
                {
                    return;
                }
            }
            while (true);

            // Use Gemini Service to analyse key log and prepare custom practice sequence
            Console.WriteLine("\n\nNice work! Your key press data is being analysed by Google's Gemini AI model...");
            string practiceText;
            try
            {
                practiceText = await geminiApiService.Assess(keyLog);
                assessment = practiceText;
            }
            catch (InvalidOperationException ex)
            {
                Console.Error.WriteLine($"Gemini API Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Unexpected error: {ex.Message}");
            }
            Console.WriteLine("\nHere is your personalised typing practice. Please type the following sequence:");            
            Console.WriteLine("\n" + assessment);
        
            // Feed custom practice back to user for WPM tracking
            bool practicing = true;
            bool morePracticeRequested = false;
            do 
            {
                // Generate additional Gemini string if needed
                if (morePracticeRequested)
                {
                    Console.WriteLine("\nHold on a sec while we get your another sequence to practice with...");
                    assessment = await geminiApiService.ContinuePractice(language);                    
                    Console.WriteLine("\nGemini produced the following string:\n");
                    Console.WriteLine(assessment);
                    morePracticeRequested = false;
                }

                keyInfo = Console.ReadKey(false);
                if (keyInfo.Key != ConsoleKey.Escape)
                {
                    // Start stopwatch
                    if (!stopWatch.IsRunning) {
                        stopWatch.Start();
                    }

                    if (keyInfo.Key == ConsoleKey.Backspace)
                    {
                        // Delete character in console
                        Console.Write("\b \b");

                        // Update pointer
                        if (pointer != 0)
                        {
                            pointer -= 1;
                        }

                        // Trim character from input validation
                        if (input.Length > 0) {
                            string mutatedString = input.Remove(input.Length - 1);
                            input = mutatedString; 
                        }
                    }
                    else {
                        // Update input validation and pointer
                        keyInput = new KeyInput(keyInfo.KeyChar);
                        input += keyInput;
                        pointer += 1;
                    }

                    // Check for correct input
                    if (pointer == assessment.Length && input == assessment)
                    {
                        
                        // Calculate WPM
                        int characterCount = assessment.Length;
                        double elapsedSeconds = stopWatch.Elapsed.TotalSeconds;
                        double charactersPerMinute = characterCount / elapsedSeconds * 60;
                        double wordsPerMinute = charactersPerMinute / 5;

                        // Log result and benchmarks
                        Console.WriteLine($"\n\nCongratulations on completing your first practice round! We measured your typing speed as {wordsPerMinute:F2} WPM (Words Per Minute).");
                        Console.WriteLine("\nFor context, here are some typical WPM benchmarks:");
                        Console.WriteLine("\n\tSlow: 20 - 40 WPM");
                        Console.WriteLine("\tAverage: 41 - 60 WPM");
                        Console.WriteLine("\tFast: 61 - 80 WPM");
                        Console.WriteLine("\tProfessional: 81 - 100 WPM");
                        Console.WriteLine("\tElite: 100+ WPM");
                        
                        // Ask user whether they would like to continue
                        Console.Write($"\nWould you like to continue practicing? (y/n) ");
                        switch (Console.ReadLine())
                        {
                            case "y" or "Y" or "yes" or "YES":
                                stopWatch.Stop();
                                stopWatch.Reset();
                                assessment = "";
                                input = "";
                                pointer = 0;
                                morePracticeRequested = true;
                                break;
                            case "n" or "N" or "no" or "NO":
                                Console.WriteLine("\nNo problem, see you next time!\n");
                                practicing = false;
                                break;
                            default:
                                Console.WriteLine("\nWe didn't recognise that input, so we'll call it here. See you next time!\n");
                                practicing = false;
                                break;
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            while (practicing);
        }
    }
}