namespace ATMApp.UI;

using System.Globalization;
using System.Text;
using Microsoft.VisualBasic;

public static class Utility
{

    private static long tranId;
    private static CultureInfo culture = new CultureInfo("en-GB");

    // add to value to transcation ID every time method is called (first ID = 1, 2nd is 2 etc)
    public static long GetTransactionId()
    {
        return ++tranId;
    }
    // takes a prompt
    public static string GetSecretInput(string prompt)
    {
        bool isPrompt = true;
        string asterics = "";

        StringBuilder input = new StringBuilder();
        
        while (true)
        {
            if(isPrompt)
                Console.WriteLine(prompt);
            isPrompt = false;
            ConsoleKeyInfo inputKey = Console.ReadKey(true);

            if (inputKey.Key == ConsoleKey.Enter)
            {
                if (input.Length == 6)
                {
                    break;
                }
                else
                {
                    PrintMessage("Please enter 6 digits,", false);
                    isPrompt = true;
                    input.Clear();
                    isPrompt = true;
                    continue;
                }
            }

            if (inputKey.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input.Remove(input.Length - 1, 1);
            } else if (inputKey.Key != ConsoleKey.Backspace)
            {
                input.Append(inputKey.KeyChar);
                Console.Write(asterics + "*");
            }
        }

        return input.ToString();
    }
    public static void PrintMessage(string msg, bool success = true)
    {
        if (success)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }
        Console.WriteLine(msg);
        Console.ForegroundColor = ConsoleColor.White;
        PressEnterToContinue();
    }
    public static string GetUserInput(string prompt)
    {
        Console.WriteLine($"Enter {prompt}");
        return Console.ReadLine();
    }

    public static void PrintDotAnimation(int timer = 10)
    {
        for(int i = 0; i < timer; i++)
        {
            Console.Write(".");
            Thread.Sleep(200); // delays thread for 200 milliseconds
        }
        Console.Clear();
    }
    public static void PressEnterToContinue()
    {
        // Enter prompt
        Console.WriteLine("\n\nPress enter to continue...\n");
        Console.ReadLine();
    }

    public static string FormatAmount(decimal amt)
    {
        return String.Format(culture, "{0:C2}", amt);
    }
}