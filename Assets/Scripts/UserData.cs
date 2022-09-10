using UnityEngine;

public class UserData
{
    private const string CurrentLevelKey = "CurrentLevel";

    public int CurrentLevel => GetCurrentLevel();

    public void SetCurrentLevel()
    {
        PlayerPrefs.SetInt(CurrentLevelKey, CurrentLevel + 1);
    }

    public void ResetLevel()
    {
        PlayerPrefs.SetInt(CurrentLevelKey, 0);
        ResetedProgressWindow.Show();
    }

    private int GetCurrentLevel()
    {
        return PlayerPrefs.GetInt(CurrentLevelKey);
    }
}
