using UnityEngine;
using UnityEditor;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using System.Collections;
using System;


public class SceneManagerController : MonoBehaviour
{
    [SerializeField] GameObject loading;
    AsyncOperationHandle loadHandler;
    public bool isLoading = false;

    public void BeforeLoadScene()
    {
        loading.SetActive(true);
        isLoading = true;
    }

    void LoadScene(AssetReference scene)
    {
        StartCoroutine(LoadSceneAsync(scene));
    }

    IEnumerator LoadSceneAsync(AssetReference scene)
    {
        if (loadHandler.IsValid())
        {
            var unLoadHandle = Addressables.UnloadSceneAsync(loadHandler, true);
            yield return unLoadHandle;
        }

        loadHandler = scene.LoadSceneAsync(LoadSceneMode.Single, true);
        yield return loadHandler;

        loading.SetActive(false);
        isLoading = false;
    }
    [SerializeField] AssetReference levelMenuScene;
    public void GotoLevelMenuScene()
    {
        LoadScene(levelMenuScene);
    }

    [SerializeField] AssetReference startScene;
    public void GotoStartScene()
    {
        LoadScene(startScene);
    }

    [SerializeField] AssetReference gameScene;
    public void GotoGameScene()
    {
        LoadScene(gameScene);
    }

    [SerializeField] AssetReference endScene;
    public void GotoEndScene()
    {
        LoadScene(endScene);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    void Awake()
    {
        loading.SetActive(false);
    }
}
