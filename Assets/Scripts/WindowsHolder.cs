using System;
using System.Collections.Generic;

public class WindowsHolder
{
    public static readonly Dictionary<Type, string> Windows = new Dictionary<Type, string>
    {
        [typeof(MainMenuWindow)] = "Windows/MainMenuWindow",
        [typeof(LoadingWindow)] = "Windows/LoadingWindow"
    };
}

