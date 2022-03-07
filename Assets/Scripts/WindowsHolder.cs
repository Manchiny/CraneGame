using System;
using System.Collections.Generic;

public class WindowsHolder
{
    public static readonly Dictionary<Type, string> Windows = new Dictionary<Type, string>
    {
        [typeof(MainMenuWindow)] = "Windows/MainMenuWindow",
        [typeof(LoadingWindow)] = "Windows/LoadingWindow",
        [typeof(GameMainWindow)] = "Windows/GameMainWindow",
        [typeof(ExitLevelWindow)] = "Windows/ExitLevelWindow",
        [typeof(LevelCompleteWindow)] = "Windows/LevelCompleteWindow",
        [typeof(LevelFailedWindow)] = "Windows/LevelFailedWindow",
        [typeof(NoMoreLevelsWindow)] = "Windows/NoMoreLevelsWindow",
        [typeof(ConfirmWindow)] = "Windows/ConfirmWindow",
        [typeof(ResetedProgressWindow)] = "Windows/ResetedProgressWindow"
    };
}

