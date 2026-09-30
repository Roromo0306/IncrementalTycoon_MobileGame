using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    private GameCompositionRoot compositionRoot;

    private void Awake()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        compositionRoot = new GameCompositionRoot();
        compositionRoot.Build();

        Debug.Log("GameBootstrapper: Game initialized.");
    }
}