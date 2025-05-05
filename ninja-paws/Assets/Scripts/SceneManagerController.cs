using UnityEngine;
using UnityEditor;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;


public class SceneManagerController : MonoBehaviour
{
    GameManagerController manager;
    GameSettings config;
    [SerializeField] GameObject loading;
    AsyncOperationHandle loadHandler;
    public bool isLoading = false;

    public void BeforeLoadScene()
    {
        loading.SetActive(true);
        isLoading = true;
    }

    async Task LoadSceneAsync(AssetReference scene)
    {
        if (loadHandler.IsValid())
        {
            var unLoadHandle = Addressables.UnloadSceneAsync(loadHandler, true);
            await unLoadHandle.Task;
        }

        loadHandler = scene.LoadSceneAsync(LoadSceneMode.Single, true);
        await loadHandler.Task;

        loading.SetActive(false);
        isLoading = false;
    }
    [SerializeField] AssetReference levelMenuScene;
    public async Task GotoLevelMenuScene()
    {
        await LoadSceneAsync(levelMenuScene);

        manager.PlayAmbience(config.levelsAmbienceMusic);
        manager.PlayBgm(config.levelsNonDiegeticMusic);
    }

    [SerializeField] AssetReference startScene;
    public async Task GotoStartScene()
    {
        await LoadSceneAsync(startScene);

        PlayStartSceneMusic();
    }

    void PlayStartSceneMusic() {
        manager.PlayAmbience(config.startAmbienceMusic);
        manager.PlayBgm(config.startNonDiegeticMusic);
    }

    [SerializeField] AssetReference gameScene;
    public async Task GotoGameScene()
    {
        await LoadSceneAsync(gameScene);

        manager.PlayAmbience(config.gameAmbienceMusic);
        manager.PlayBgm(config.gameNonDiegeticMusic);
    }

    [SerializeField] AssetReference endScene;
    public async Task GotoEndScene()
    {
        await LoadSceneAsync(endScene);

        manager.PlayAmbience(config.endingAmbienceMusic);
        manager.PlayAmbience(config.endingNonDiegeticMusic);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    void Awake()
    {
        loading.SetActive(false);
    }

    bool isFirstTime = false;

    void Start()
    {
        manager = GameManagerController.Instance;
        config = manager.config;

        if (!isFirstTime) {
            PlayStartSceneMusic();
        }
    }
}
