using UnityEngine;

public class UserData
{
    private const string CURRENT_LEVEL = "CurrentLevel";
    public int CurrentLevel => GetCurrentLevel();

    public void SetCurrentLevel()
    {
        PlayerPrefs.SetInt(CURRENT_LEVEL, CurrentLevel + 1);
    }

    public void ResetLevel()
    {
        PlayerPrefs.SetInt(CURRENT_LEVEL, 0);
        ResetedProgressWindow.Of();
    }

    private int GetCurrentLevel()
    {
        return PlayerPrefs.GetInt(CURRENT_LEVEL);
    }
}
