public class PlayingState : IGameState
{
    private GameService gameService;

    public PlayingState(GameService service)
    {
        gameService = service;
    }

    public void Enter()
    {
        gameService.ResetGame();
    }

    public void Exit() { }

    public void Update()
    {
        // Logic nếu cần
    }
}