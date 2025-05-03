using UnityEngine;
using UnityEditor;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;

public class SceneManagerController : MonoBehaviour
{
    [SerializeField] GameObject loading;
    [SerializeField] AssetReference levelMenuScene;
    AsyncOperationHandle loadHandler;
    public void BeforeLoadScene() {
        loading.SetActive(true);
    }

    public void GotoLevelMenuScene() {

        loadHandler = levelMenuScene.LoadSceneAsync(LoadSceneMode.Single);

        loadHandler.Completed += (handle) => {
            loading.SetActive(false);
        };
    }

    IEnumerator UnloadPreviousSceneDelay(float seconds) {
        yield return new WaitForSeconds(seconds);
    }

    void Awake()
    {
        loading.SetActive(false);
    }
}
