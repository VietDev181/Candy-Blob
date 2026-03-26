using System;

public class GameService
{
    public GameStateMachine StateMachine { get; private set; }

    private ScoreUseCase scoreUseCase;

    public event Action<int> OnScoreChanged;
    public event Action<int, int> OnGameOver;

    public GameService(ScoreUseCase scoreUseCase)
    {
        this.scoreUseCase = scoreUseCase;

        scoreUseCase.OnScoreChanged += (score) =>
        {
            OnScoreChanged?.Invoke(score);
        };
    }

    public void SetStateMachine(GameStateMachine machine)
    {
        StateMachine = machine;
    }

    public void AddScore(int value)
    {
        scoreUseCase.AddScore(value);
    }

    public void GameOver()
    {
        int current = scoreUseCase.GetCurrentScore();
        int high = scoreUseCase.GetHighScore();

        OnGameOver?.Invoke(current, high);

        StateMachine.ChangeState(GameStateType.GameOver);
    }

    public void ResetGame()
    {
        scoreUseCase.Reset();
    }
}
