using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private UIGame uiGame;

    private GameService gameService;
    private SaveSystem saveSystem;
    private IAudioService audioService;

    public void Initialize(GameService service, SaveSystem save, IAudioService audio)
    {
        gameService = service;
        saveSystem = save;
        audioService = audio;

        gameService.OnScoreChanged += uiGame.UpdateScore;
        gameService.OnGameOver += HandleGameOver;
    }

    private void Start()
    {
        audioService.PlayGameBGM();
    }

    private void Update()
    {
        gameService.StateMachine.Update();
    }

    public void AddScore(int value)
    {
        gameService.AddScore(value);
    }

    public void GameOver()
    {
        gameService.GameOver();
    }

    private void HandleGameOver(int current, int high)
    {
        saveSystem.SaveHighScore(high);
        audioService.PlayGameOverBGM();
        uiGame.ShowGameOver(current, high);
    }

    private void OnDestroy()
    {
        if (gameService != null)
        {
            gameService.OnScoreChanged -= uiGame.UpdateScore;
            gameService.OnGameOver -= HandleGameOver;
        }
    }
}
