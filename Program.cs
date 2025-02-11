namespace TipTapToe
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to TipTapToe. Press Escape to exit. Let's get practicing!");
            ConsoleKeyInfo keyInfo;
            do 
            {   
                keyInfo = Console.ReadKey();
                if (keyInfo.Key != ConsoleKey.Escape)
                {
                Console.WriteLine(keyInfo.Key);
                }
            }
            while (keyInfo.Key != ConsoleKey.Escape);
        }
    }
}