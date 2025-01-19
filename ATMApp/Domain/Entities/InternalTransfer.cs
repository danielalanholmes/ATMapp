namespace ATMApp.Domain.Entities;

using System.Transactions;

public class InternalTransfer
{
    public decimal TransferAmount { get; set; }
    public long RecipientBankAccountNumber { get; set; }
    public string RecipientBankAccountName { get; set; }
}