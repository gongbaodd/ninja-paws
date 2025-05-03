using UnityEngine;
using UnityEditor;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public enum GameScene {
    Start,
    Levels,
}

public class SceneManagerController : MonoBehaviour
{
    [SerializeField] GameObject loading;
    AsyncOperationHandle loadHandler;
    AsyncOperationHandle unloadHandler;
    public bool isLoading = false;

    public GameScene currentScene = GameScene.Start;
    public void BeforeLoadScene()
    {
        loading.SetActive(true);
        isLoading = true;
    }

    void LoadScene(AssetReference scene)
    {
        print("loading");
        if (loadHandler.IsValid())
        {
            unloadHandler = Addressables.UnloadSceneAsync(loadHandler, true);

            unloadHandler.Completed += (handle) => {
                print("unloaded");
            };
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
        currentScene = GameScene.Levels;
    }

    [SerializeField] AssetReference startScene;
    public void GotoStartScene()
    {
        LoadScene(startScene);
        currentScene = GameScene.Start;
    }
    void Awake()
    {
        loading.SetActive(false);
    }
}
