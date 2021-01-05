using UnityEngine;
using UnityEngine.SceneManagement;

static public class LevelHub
{
    private static string currentLevelName;

    public static Scene curentScene;
    public static Canvas menuCanvas;
    public static Canvas loadingCanvas;
    public static Canvas titlesCanvas;

    public static void LoadLevel(string levelName)
    {
        // Hide menu
        if (menuCanvas) menuCanvas.enabled = false;
        // Show loading
        if (loadingCanvas) loadingCanvas.enabled = true;

        // Load scene for the first time
        if (curentScene == null || !curentScene.isLoaded)
        {
            LoadScene(levelName);
        }
        else // If some level-scene was already loaded - unload it first and only then Load Another One
        {
            UnloadAndLoadScene(levelName);
        }

        // Reset time scale
        Time.timeScale = 1;
    }

    private static void LoadScene( string levelName)
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        currentLevelName = levelName;

        SceneManager.LoadScene(currentLevelName, LoadSceneMode.Additive);
    }

    private static void UnloadAndLoadScene(string levelName)
    {
        currentLevelName = levelName;

        SceneManager.UnloadSceneAsync(curentScene);
        SceneManager.sceneUnloaded += LoadNewSceneAfterUnload;
    }

    // Utility functions
    private static void LoadNewSceneAfterUnload(Scene scene)
    {
        LoadScene(currentLevelName);

        SceneManager.sceneUnloaded -= LoadNewSceneAfterUnload;
    }

    // Events
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        curentScene = scene;
        SceneManager.SetActiveScene(scene);

        // Hide loading
        if (loadingCanvas) loadingCanvas.enabled = false;
        if (titlesCanvas) titlesCanvas.enabled = false;
    }

}
