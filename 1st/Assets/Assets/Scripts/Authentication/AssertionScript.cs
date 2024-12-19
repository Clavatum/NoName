using System.Text.RegularExpressions;

public static class AssertionScript
{
    public static bool IsValidUsername(string username, out string errorMessage)
    {
        if (string.IsNullOrEmpty(username))
        {
            errorMessage = "Username cannot be empty!";
            return false;
        }

        if (!Regex.IsMatch(username, @"^[a-zA-Z0-9.\-_@]{3,20}$"))
        {
            errorMessage = "Username must be 3-20 characters long and contain only letters, numbers, and symbols like ., -, @ or _.";
            return false;
        }

        errorMessage = null;
        return true;
    }

    public static bool IsValidPassword(string password, out string errorMessage)
    {
        if (string.IsNullOrEmpty(password))
        {
            errorMessage = "Password cannot be empty!";
            return false;
        }

        if (password.Length < 8 || password.Length > 30)
        {
            errorMessage = "Password must be 8-30 characters long.";
            return false;
        }

        if (!Regex.IsMatch(password, "[a-z]"))
        {
            errorMessage = "Password must include at least one lowercase letter.";
            return false;
        }

        if (!Regex.IsMatch(password, "[A-Z]"))
        {
            errorMessage = "Password must include at least one uppercase letter.";
            return false;
        }

        if (!Regex.IsMatch(password, "[0-9]"))
        {
            errorMessage = "Password must include at least one number.";
            return false;
        }

        if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?""{}|<>_\-+=]"))
        {
            errorMessage = "Password must include at least one special character.";
            return false;
        }

        errorMessage = null;
        return true;
    }
}
