namespace ATMApp;

using UI;
    
public class Entry
{
    static void Main(string[] args)
    {
        ATMApp atmApp = new ATMApp();
        atmApp.IntialiseData();
        atmApp.Run();
    }
}