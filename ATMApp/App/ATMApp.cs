namespace ATMApp;

using System.Diagnostics;
using System.Web;
using ConsoleTables;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using UI;

public class ATMApp : IUserLogin, IUserAccountActions, ITransaction
{
    private List<UserAccount> userAccountList;
    private UserAccount selectedAccount;
    private IUserLogin _userLoginImplementation;
    private List<Transaction> _listOfTransactions;
    private const int minimumKeptAmount = 10;
    private readonly AppScreen screen;

    public ATMApp()
    {
        screen = new AppScreen();
    }

    public void Run()
    {
        AppScreen.Welcome();
        CheckUserCardNumberAndPassword();
        AppScreen.WelcomeCustomer(selectedAccount.FullName);
        while (true)
        {
            AppScreen.DisplayAppMenu();
            ProcessMenuOption();
        }
    }

    public void IntialiseData()
    {
        userAccountList = new List<UserAccount>
        {
            new UserAccount
            {
                Id = 1, FullName = "Daniel Holmes", AccountNumber = 12345678, CardNumber = 1234123412341234,
                CardPin = 123456, AccountBalance = 1000.00m, IsLocked = false
            },
            new UserAccount
            {
                Id = 2, FullName = "Jodie Davis", AccountNumber = 87654321, CardNumber = 1234123412341235,
                CardPin = 123512, AccountBalance = 1100.00m, IsLocked = false
            },
            new UserAccount
            {
                Id = 3, FullName = "Joe Bloggs", AccountNumber = 12345679, CardNumber = 1234123412341236,
                CardPin = 123667, AccountBalance = 11000.00m, IsLocked = true
            },
        };
        _listOfTransactions = new List<Transaction>();
    }

    public void CheckUserCardNumberAndPassword()
    {
        bool isCorrectLogin = false;

        // Loops through the user account list to check the current login details
        while (isCorrectLogin == false)
        {
            UserAccount inputAccount = AppScreen.UserLoginForm();
            AppScreen.LoginProgress();
            foreach (UserAccount account in userAccountList)
            {
                selectedAccount = account;
                if (inputAccount.CardNumber.Equals(selectedAccount.CardNumber))
                {
                    selectedAccount.TotalLogin++;

                    if (inputAccount.CardPin.Equals(selectedAccount.CardPin))
                    {
                        selectedAccount = account;

                        // Check the account isn't locked or login attempts is more than 3
                        if (selectedAccount.IsLocked || selectedAccount.TotalLogin > 3)
                        {
                            // Print a lock message
                            AppScreen.PrintLockedScreen();
                        }
                        else
                        {
                            selectedAccount.TotalLogin = 0;
                            isCorrectLogin = true;
                            break;
                        }
                    }
                }
                if (isCorrectLogin == false)
                {
                    Utility.PrintMessage("\nInvalid card number or PIN", false);
                    selectedAccount.IsLocked = selectedAccount.TotalLogin == 3;
                    if (selectedAccount.IsLocked)
                    {
                        AppScreen.PrintLockedScreen();
                    }
                }
                Console.Clear();
            }
        }


    }

    private void ProcessMenuOption()
    {
        switch (Validator.Convert<int>("an option:"))
        {
            case (int)AppMenu.CheckBalance: // Converts enum to an integer
                CheckBalance();
                break;
            case (int)AppMenu.PlaceDeposit:
                PlaceDeposit();
                break;
            case (int)AppMenu.MakeWithdrawal:
                MakeWithdrawal();
                break;
            case (int)AppMenu.InternalTransfer:
                var internalTransfer = screen.InternalTransferForm();
                ProcessInternalTransfer(internalTransfer);
                break;
            case (int)AppMenu.ViewTransaction:
                ViewTransaction();
                break;
            case (int)AppMenu.Logout:
                AppScreen.LogoutProgress();
                Utility.PrintMessage("You have successfully logged out. Please don't forget to take your card.");
                Run();
                break;
            default:
                Utility.PrintMessage("Invalid option.", false); // Runs if input is invalid
                break;
        }
    }

    public void CheckBalance()
    {
        Utility.PrintMessage($"Your account balance is: {Utility.FormatAmount(selectedAccount.AccountBalance)}");
    }

    public void PlaceDeposit()
    {
        Console.WriteLine("\nThis machine only accepts £10 and £20 notes\n");
        var transaction_amt = Validator.Convert<int>($"amount {AppScreen.cur}");
        
        // Simulate counting
        Console.WriteLine("\nConfirming amount, checking notes.\n");
        Utility.PrintDotAnimation();
        Console.WriteLine("");
        
        // Guard clause
        if (transaction_amt <= 0)
        {
            Utility.PrintMessage("Amount needs to be greater than 0, please try again.", false);
            return;
        }

        if (transaction_amt % 10 != 0)
        {
            Utility.PrintMessage($"Deposit amount must be a multiple of 10 0r 20, please try again.", false);
            return;
        }

        if (PreviewBankNotesCount(transaction_amt) == false)
        {
            Utility.PrintMessage($"You have cancelled your action.", false);
        }
        
        // bind transaction details to transaction object
        InsertTransaction(selectedAccount.Id, TransactionType.Deposit, transaction_amt, "");
        
        //update account balance
        selectedAccount.AccountBalance += transaction_amt;
        
        // print success message
        Utility.PrintMessage($"Your deposit of {Utility.FormatAmount(transaction_amt)} was " +
                             $"succesful.", true);
    }

    public void MakeWithdrawal()
    {
        var transaction_amt = 0;
        int selectedAmount = AppScreen.SelectAmount();
        if (selectedAmount == -1)
        {
            selectedAmount = AppScreen.SelectAmount();
        }
        else if (selectedAmount != 0)
        {
            transaction_amt = selectedAmount;
        }
        else
        {
            transaction_amt = Validator.Convert<int>($"amount {AppScreen.cur}");
        }
        
        // input validation on transaction amounts
        if (transaction_amt <= 0)
        {
            Utility.PrintMessage("Amount must be greater than 0.", false);
            return;
        }

        if (transaction_amt % 10 != 0)
        {
            Utility.PrintMessage("You can only withdraw £10 or £20 notes.");
            return;
        }
        
        // business logic validations
        if (transaction_amt > selectedAccount.AccountBalance)
        {
            Utility.PrintMessage($"Withdrawal failed, insufficient funds." +
                $"{Utility.FormatAmount(transaction_amt)}", false);
            return;
        }

        if ((selectedAccount.AccountBalance - transaction_amt) < minimumKeptAmount)
        {
            Utility.PrintMessage($"$Insufficient funds, you must have a minimum of " + $"{Utility.FormatAmount(minimumKeptAmount)}", false);
            return;
        }
        
        // Bind withdrawal details to transaction object
        InsertTransaction(selectedAccount.Id, TransactionType.Withdrawal,-transaction_amt, "");
        
        // Update balance
        selectedAccount.AccountBalance -= transaction_amt;
        
        // Success message
        Utility.PrintMessage($"Withdrawal of " +
                             $"{Utility.FormatAmount(transaction_amt)}" +
                             " successful", true);
    }

    private bool PreviewBankNotesCount(int amount)
    {
        int twentyNotesCount = amount / 20;
        int tenNotesCount = (amount % 20) / 10;
        
        Console.WriteLine("\nSummary");
        Console.WriteLine("--------");
        Console.WriteLine($"{AppScreen.cur}20 X {twentyNotesCount} = {20 * twentyNotesCount}");
        Console.WriteLine($"{AppScreen.cur}10 X {tenNotesCount} = {10 * tenNotesCount}");
        Console.WriteLine($"Total amount: {Utility.FormatAmount(amount)}\n\n");

        int opt = Validator.Convert<int>("Press 1 to confirm");
        return opt.Equals(1); // returns a bool either false or true depending on confrirmation. If not 1 returns false, if 1 true.
    }

    public void InsertTransaction(long _UserBankAccountID, TransactionType _tranType, decimal _tranAmount, string _desc)
    {
        // create new transaction object
        var transaction = new Transaction()
        {
            TransactionId = Utility.GetTransactionId(),
            UserBankAccountId = _UserBankAccountID,
            TransactionDate = DateTime.Now,
            TransactionType = _tranType,
            TransactionAmount = _tranAmount,
            Description = _desc
        };
        
        // add transaction object to the list
        _listOfTransactions.Add(transaction);
    }

    public void ViewTransaction()
    {
        var filteredTransactionList =
            _listOfTransactions.Where(t => t.UserBankAccountId == selectedAccount.Id).ToList();
        // check is there is a transaction
        if (filteredTransactionList.Count <= 0)
        {
            Utility.PrintMessage("You have not made any transactions yet.", true);
        }
        else
        {
            var table = new ConsoleTable("Id", "Transaction Date", "Type", "Description", "Amount" + AppScreen.cur);
            foreach (var tran in filteredTransactionList)
            {
                table.AddRow(tran.TransactionId, tran.TransactionDate, tran.TransactionType, tran.Description,
                    tran.TransactionAmount);
            }

            table.Options.EnableCount = false;
            table.Write();
            Utility.PrintMessage($"You have {filteredTransactionList.Count} transaction(s)", true);
        }
    }

    private void ProcessInternalTransfer(InternalTransfer internalTransfer)
    {
        if (internalTransfer.TransferAmount <= 0)
        {
            Utility.PrintMessage("Amount needs to be 1p or more.", false);
            return;
        }
        
        // check sender's account balance
        if (internalTransfer.TransferAmount > selectedAccount.AccountBalance)
        {
            Utility.PrintMessage($"Transfer failed, insufficient funds to transfer {Utility.FormatAmount(internalTransfer.TransferAmount)}.", false);
            return;
        }
        
        // Check minimum kept amount
        if ((selectedAccount.AccountBalance - minimumKeptAmount) < minimumKeptAmount)
        {
            Utility.PrintMessage($"Transfer failed, your account needs to have a minimum of" +
                                 $"{Utility.FormatAmount(minimumKeptAmount)}", false);
            return;
        }
        
        // check receiver's account number is valid (LINQ method)
        var selectedBankAccountReceiver = (from userAcc in userAccountList
            where userAcc.AccountNumber == internalTransfer.RecipientBankAccountNumber
            select userAcc).FirstOrDefault();
        if (selectedBankAccountReceiver == null)
        {
            Utility.PrintMessage("Transfer failed, invalid receiver account number", false);
            return;
        }
        
        // Check receiver's name
        if (selectedBankAccountReceiver.FullName != internalTransfer.RecipientBankAccountName)
        {
            Utility.PrintMessage("Transfer failed, recipient's name does not match");
            return;
        }
        
        // Add transaction to transaction record (sender)
        InsertTransaction(selectedAccount.Id, TransactionType.Transfer, internalTransfer.TransferAmount,"Transferred " +
            $"to {selectedBankAccountReceiver.AccountNumber} ({selectedBankAccountReceiver.FullName}");
        
        // Update sender's account balance
        selectedAccount.AccountBalance -= internalTransfer.TransferAmount;
        
        // Add transaction record (receiver)
        InsertTransaction(selectedBankAccountReceiver.Id, TransactionType.Transfer, internalTransfer.TransferAmount, "Transferred" + 
            $"to {selectedAccount.AccountNumber} ({selectedAccount.FullName})");
        
        // Update receiver's account balance
        selectedBankAccountReceiver.AccountBalance -= internalTransfer.TransferAmount;
        
        // Print success message
        Utility.PrintMessage("You have successfully transferred " + 
                             $"{Utility.FormatAmount(internalTransfer.TransferAmount)} to " +
                             $"{internalTransfer.RecipientBankAccountName}", true);
    }
}