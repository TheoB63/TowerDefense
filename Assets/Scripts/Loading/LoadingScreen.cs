using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    AsyncOperation async;

    void Start()
    {
        StartCoroutine(LoadGame());
    }

    IEnumerator LoadGame()
    {
        async = SceneManager.LoadSceneAsync("Menu");
        async.allowSceneActivation = false;

        //need to load more here

        while (async.progress < 0.9f)
        {
            yield return null;
        }
        async.allowSceneActivation = true;
    }
}