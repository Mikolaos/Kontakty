using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class StrongPasswordAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is not string password) return false;

        return password.Length >= 8 &&
               Regex.IsMatch(password, "[A-Z]") &&  
               Regex.IsMatch(password, "[a-z]") &&     
               Regex.IsMatch(password, "[0-9]") &&    
               Regex.IsMatch(password, "[^a-zA-Z0-9]"); 
    }

    public override string FormatErrorMessage(string name)
    {
        return "The password must be at least 8 characters long and include an uppercase letter, a lowercase letter, a digit, and a special character.";
    }
}