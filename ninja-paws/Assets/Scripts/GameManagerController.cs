using UnityEngine;

[RequireComponent(typeof(SceneManagerController))]
public class GameManagerController : MonoBehaviour
{
    public static GameManagerController Instance { get { return instance; } }
    
    static GameManagerController instance;
    public GameSettings config;

    void InitPlayerSettings() {
        bool playerUseMotion = PlayerPrefs.GetInt("UseMotion") == 1;
        config.useMotion = playerUseMotion;
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }

        InitPlayerSettings();
    }

}
