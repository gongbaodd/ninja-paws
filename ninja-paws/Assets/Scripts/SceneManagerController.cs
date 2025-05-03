using UnityEngine;
using UnityEditor;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SceneManagerController : MonoBehaviour
{
    [SerializeField] GameObject loading;
    [SerializeField] AssetReference levelMenuScene;
    AsyncOperationHandle loadHandler;

    public void GotoLevelMenuScene() {
        loading.SetActive(true);
        loadHandler = levelMenuScene.LoadSceneAsync(LoadSceneMode.Single);

        loadHandler.Completed += (handle) => {
            loading.SetActive(false);  
        };
    }

    void Awake()
    {
        loading.SetActive(false);
    }
}
