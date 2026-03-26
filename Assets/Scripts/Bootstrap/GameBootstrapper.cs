using System.Collections.Generic;
using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private AudioService audioServiceMono;
    [SerializeField] private GameController gameController;
    [SerializeField] private GridController gridController;
    [SerializeField] private UIGame uiGame;
    [SerializeField] private UISetting uiSetting;

    void Awake()
    {
        // Infrastructure
        IAudioService audio = audioServiceMono;
        SaveSystem saveSystem = new SaveSystem();
        
        // ── Domain
        ScoreModel scoreModel = new ScoreModel(saveSystem.LoadHighScore());
        ScoreUseCase scoreUseCase = new ScoreUseCase(scoreModel);

        // ── Application
        GameService gameService = new GameService(scoreUseCase);
        IGridService gridService = new GridService(gridController.rows, gridController.cols);

        // State machine
        var states = new Dictionary<GameStateType, IGameState>
        {
            { GameStateType.Playing, new PlayingState(gameService) },
            { GameStateType.GameOver, new GameOverState() }
        };

        GameStateMachine stateMachine = new GameStateMachine(states);
        gameService.SetStateMachine(stateMachine);
        
        // ── Presentation
        uiGame.Initialize(audio);
        uiSetting.Initialize(audio);
        gameController.Initialize(gameService, saveSystem, audio);
        gridController.Initialize(gridService, audio, gameController);
        gridController.Initialize(gridService, audio, gameController);

        // ── Boot
        stateMachine.ChangeState(GameStateType.Playing);
    }
}
