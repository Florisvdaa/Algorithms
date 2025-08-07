using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// this class cannot be attached to an object
public static class Loader
{
    private class LoadingMonoBehaviour : MonoBehaviour { }

    private static Scene targetScene;

    public enum Scene
    {
        MainMenuScene,
        LoadingScene,
        AStarScene,
        GameOfLifeScene,
        FlockingSystemScene,
        LStyleScene,
        EcoSystemScene
    }

    private static Action onLoaderCallback;
    private static AsyncOperation loadingAsyncOperation;

    public static void Load(Scene targetScene)
    {
        // Set the loader callback action to load the taerget scene
        onLoaderCallback = () =>
        {
            GameObject loadingGameObject = new GameObject("Loading Game Object");
            loadingGameObject.AddComponent<LoadingMonoBehaviour>().StartCoroutine(LoadSceneAsync(targetScene));
        };

        // load the loading scene
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }
    /// <summary>
    /// Triggered after first update in LoaderCallback
    /// Execute the loader callback action which will load the target scene
    /// </summary>
    public static void LoaderCallback()
    {
        if (onLoaderCallback != null)
        {
            onLoaderCallback();
            onLoaderCallback = null;
        }
    }

    private static IEnumerator LoadSceneAsync(Scene scene)
    {
        yield return null;

        loadingAsyncOperation = SceneManager.LoadSceneAsync(scene.ToString());

        while (!loadingAsyncOperation.isDone)
        {
            yield return null;
        }
    }

    public static float GetLoadingProgress()
    {
        if (loadingAsyncOperation != null)
        {
            return loadingAsyncOperation.progress;
        }
        else
        {
            return 1f;
        }
    }
}