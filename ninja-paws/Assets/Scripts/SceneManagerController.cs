using UnityEngine;
using UnityEditor;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;


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
        if (loadHandler.IsValid())
        {
            Addressables.UnloadSceneAsync(loadHandler, true);
        }

        loadHandler = scene.LoadSceneAsync(LoadSceneMode.Single, true);

        loadHandler.Completed += (handle) =>
        {
            loading.SetActive(false);
            isLoading = false;
        };
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

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    void Awake()
    {
        loading.SetActive(false);
    }
}
