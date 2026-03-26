using UnityEngine;

public class SaveSystem
{
    private const string HIGH_SCORE_KEY = "HighScore";

    public int LoadHighScore()
    {
        return PlayerPrefs.GetInt(HIGH_SCORE_KEY);
    }

    public void SaveHighScore(int score)
    {
        PlayerPrefs.SetInt(HIGH_SCORE_KEY, score);
        PlayerPrefs.Save();
    }
}
