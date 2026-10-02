using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBootstrapper : MonoBehaviour
{
    private const string GameSceneName =
        "Game";

    private GameCompositionRoot
        compositionRoot;


    private void Awake()
    {
        DontDestroyOnLoad(
            gameObject
        );

        InitializeGame();
    }


    private void Update()
    {
        compositionRoot?.Tick();
    }


    private void InitializeGame()
    {
        compositionRoot =
            new GameCompositionRoot();

        compositionRoot.Build();

        SceneManager.sceneLoaded +=
            OnSceneLoaded;

        SceneManager.LoadScene(
            GameSceneName
        );
    }


    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode loadSceneMode)
    {
        if (scene.name != GameSceneName)
        {
            return;
        }

        GameSceneReferences sceneReferences =
            FindObjectOfType<
                GameSceneReferences
            >();

        if (sceneReferences == null)
        {
            Debug.LogError(
                "GameSceneReferences was not found in the Game scene."
            );

            return;
        }

        compositionRoot.InitializeGameplay(
            sceneReferences.FloorDefinitions,
            sceneReferences.TowerView,
            sceneReferences.EconomyView
        );

        SceneManager.sceneLoaded -=
            OnSceneLoaded;
    }


    private void OnDestroy()
    {
        SceneManager.sceneLoaded -=
            OnSceneLoaded;

        compositionRoot?.Dispose();
    }
}