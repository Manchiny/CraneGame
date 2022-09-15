using UnityEngine;

public class UserData
{
    private const string CurrentLevelKey = "CurrentLevel";
    private const string SoundKey = "SoundOn";

    public int CurrentLevel => GetCurrentLevel();
    public bool IsSoundOn()
    {
        if (PlayerPrefs.HasKey(SoundKey) == false)
            PlayerPrefs.SetInt(SoundKey, BoolToInt(true));

        return IntToBool(PlayerPrefs.GetInt(SoundKey), SoundKey);
    }

        public void SetCurrentLevel()
    {
        PlayerPrefs.SetInt(CurrentLevelKey, CurrentLevel + 1);
    }

    public void ResetLevel()
    {
        PlayerPrefs.SetInt(CurrentLevelKey, 0);
        ResetedProgressWindow.Show();
    }

    public void SetSoundActive(bool isActive)
    {
        if (IsSoundOn() == isActive)
            return;

        PlayerPrefs.SetInt(SoundKey, BoolToInt(isActive));
    }

    private int GetCurrentLevel()
    {
        return PlayerPrefs.GetInt(CurrentLevelKey);
    }

    private int BoolToInt(bool value) => value ? 1 : 0;

    private bool IntToBool(int value, string key)
    {
        if (value != 0 && value != 1)
            Debug.LogError($"Bool value {key} = {value} in not correct!");

        return value == 0 ? false : true;
    }
}
