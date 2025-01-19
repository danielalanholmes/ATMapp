namespace ATMApp.UI;

using System.ComponentModel;

public static class Validator
{
    public static T Convert<T>(string prompt) // Generic
    {
        bool valid = false;
        string userInput;
        
        // Keeps asking for the specified type until valid input is entered by user
        while (!valid)
        {
            userInput = Utility.GetUserInput(prompt);

            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter != null)
                {
                    return (T)converter.ConvertFromString(userInput); // Convert from string to specified type
                }
                else
                {
                    return default;
                }
            }
            catch
            {
                Utility.PrintMessage("Please enter numbers 0-9.", false);
            }
        }

        return default;
    }
}