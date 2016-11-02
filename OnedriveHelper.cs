using System;

public static class OnedriveHelper
{
    public static string GetOneDrivePath()
    {
        const string keyName = @"HKEY_CURRENT_USER\Software\Microsoft\OneDrive";

        string oneDrivePath = (string)Microsoft.Win32.Registry.GetValue(keyName, "UserFolder", "");
    
        return oneDrivePath;
    }
}
