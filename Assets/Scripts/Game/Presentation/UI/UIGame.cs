using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIGame : MonoBehaviour
{
    private IAudioService _audio;

    public Button homeButton;
    public Button pauseButton;
    public Button settingButton;
    public Button resumeButton;
    public Button resumeSettingButton;
    public Button replayButton;
    public Button replayGameOverButton;

    [Header("UI Texts")]
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private TextMeshProUGUI gameOverHighScoreText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("UI Panel")]
    public GameObject pausePanel;
    public GameObject setttingPanel;
    public GameObject gameOverPanel;

    public void Initialize(IAudioService audio)
    {
        _audio = audio;

        homeButton.onClick.AddListener(OnHomeGame);
        pauseButton.onClick.AddListener(OnPauseGame);
        settingButton.onClick.AddListener(OnSettingGame);
        resumeButton.onClick.AddListener(OnResumeGame);
        resumeSettingButton.onClick.AddListener(OnResumeGame);
        replayButton.onClick.AddListener(OnReplayGame);
        replayGameOverButton.onClick.AddListener(OnReplayGame);

        pausePanel.SetActive(false);
        setttingPanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    private void OnHomeGame()
    {
        _audio.PlayClickSFX();
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene");
    }

    private void OnPauseGame()
    {
       _audio.PlayClickSFX();
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
    }

    private void OnSettingGame()
    {
       _audio.PlayClickSFX();
        Time.timeScale = 0f;
        setttingPanel.SetActive(true);
    }

    private void OnResumeGame()
    {
        _audio.PlayClickSFX();
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        setttingPanel.SetActive(false);
    }

    private void OnReplayGame()
    {
        Time.timeScale = 1f;
        _audio.PlayClickSFX();
        _audio.PlayGameBGM();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    public void ShowGameOver(int currentScore, int highScore)
    {
        _audio.PlayGameOverBGM();
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);

        gameOverScoreText.text = "Score: " + currentScore;
        gameOverHighScoreText.text = "Best Score\n" + highScore;
    }
}
