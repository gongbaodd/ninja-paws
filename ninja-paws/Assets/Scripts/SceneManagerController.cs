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

    async Task LoadSceneAsync(AssetReference scene, LoadSceneMode mode = LoadSceneMode.Single)
    {
        var downloadHandle = Addressables.DownloadDependenciesAsync(scene);
        await downloadHandle.Task;

        loadHandler = scene.LoadSceneAsync(mode, true);
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

    [SerializeField] AssetReference introScene;
    public async Task GotoIntroScene()
    {
        await LoadSceneAsync(introScene);
        PlayStartSceneMusic();
    }

    [SerializeField] AssetReference winScene;
    public async Task GotoWinScene()
    {
        await LoadSceneAsync(winScene);
        PlayStartSceneMusic();
    }

    public void ReloadGameScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
