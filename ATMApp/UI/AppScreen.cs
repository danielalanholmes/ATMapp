namespace ATMApp.UI;

using Domain.Entities;

public class AppScreen
{

    internal const string cur = "£";
    internal static void Welcome()
    {
        Console.Clear();
        Console.Title = "Dan's ATM App";
        Console.ForegroundColor = ConsoleColor.White;
            
        // Welcome message
        Console.WriteLine("\n\n------------Welcome to Dan's ATM app!------------\n");
            
        // Enter card prompt
        Console.WriteLine("Please insert your credit or debit card");
        Console.WriteLine("NOTE: ATM machine validates your card before proceeding. Please enter your long card number");
            
        Utility.PressEnterToContinue();
    }
    
    internal static UserAccount UserLoginForm()
    {
        // Keep asking user input until account details entered are correct
        UserAccount tempUserAccount = new UserAccount();

        tempUserAccount.CardNumber = Validator.Convert<long>("your card number.");
        tempUserAccount.CardPin = Convert.ToInt32(Utility.GetSecretInput("Enter your card PIN"));
        return tempUserAccount;
    }

    internal static void LoginProgress()
    {
        Console.WriteLine("\nChecking card number and PIN");
        Utility.PrintDotAnimation();
    }

    internal static void PrintLockedScreen()
    {
        Console.Clear();
        Utility.PrintMessage("Your account is locked, please go to your nearest branch to unlock it. Thank you.", true);
        Utility.PressEnterToContinue();
        Environment.Exit(1);
    }
    
    public static void WelcomeCustomer(string fullName)
    {
        Console.WriteLine($"Welcome back, {fullName}");
    }

    internal static void DisplayAppMenu()
    {
        Console.Clear();
        Console.WriteLine("-------Dan's ATM App: Menu-------");
        Console.WriteLine(":                               :");
        Console.WriteLine("1. Account Balance              :");
        Console.WriteLine("2. Cash deposit                 :");
        Console.WriteLine("3. Withdraw                     :");
        Console.WriteLine("4. Transfer                     :");
        Console.WriteLine("5. Transactions                 :");
        Console.WriteLine("6. Log Out                      :");
    }

    internal static void LogoutProgress()
    {
        Console.WriteLine("Thank you for using my ATM application.");
        Utility.PrintDotAnimation();
        Console.Clear();
    }

    internal static int SelectAmount()
    {
        Console.WriteLine("");
        Console.WriteLine(":1.{0}10       5.{0}80", cur);
        Console.WriteLine(":2.{0}20       6.{0}100", cur);
        Console.WriteLine(":3.{0}40       7.{0}200", cur);
        Console.WriteLine(":4.{0}50       8.{0}250", cur);
        Console.WriteLine(":0.Other");
        Console.WriteLine("");

        int selectedAmount = Validator.Convert<int>("option:");
        switch (selectedAmount)
        {
            case 1:
                return 10;
                break;
            case 2:
                return 20;
                break;
            case 3:
                return 40;
                break;
            case 4:
                return 50;
                break;
            case 5:
                return 80;
                break;
            case 6:
                return 100;
                break;
            case 7:
                return 200;
                break;
            case 8:
                return 250;
                break;
            case 0:
                return 0;
                break;
            default:
                Utility.PrintMessage("This ATM only stores £10 and £10 notes.", false);
                return -1;
            break;
        }
    }

    internal InternalTransfer InternalTransferForm()
    {
        // collect data for the transfer
        var internalTransfer = new InternalTransfer();
        internalTransfer.RecipientBankAccountNumber = Validator.Convert<long>("recipient's account number:");
        internalTransfer.TransferAmount = Validator.Convert<decimal>($"amount {cur}");
        internalTransfer.RecipientBankAccountName = Utility.GetUserInput("recipient's name:");
        return internalTransfer;
    }
}