namespace ATMApp.Domain.Interfaces;

using Enums;

public interface ITransaction
{
    void InsertTransaction(long _userBankAccountId, TransactionType _tranType, decimal _tranAmount, string _desc);
    void ViewTransaction();
}