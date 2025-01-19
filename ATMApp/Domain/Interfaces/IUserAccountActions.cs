namespace ATMApp.Domain.Interfaces;

public interface IUserAccountActions
{
    void CheckBalance();
    void PlaceDeposit();
    void MakeWithdrawal();
    
}