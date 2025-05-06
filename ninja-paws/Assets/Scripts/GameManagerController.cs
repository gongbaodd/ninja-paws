using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SceneManagerController))]
[RequireComponent(typeof(AudioSource))]
public class GameManagerController : MonoBehaviour
{
    public static GameManagerController Instance { get { return instance; } }
    
    static GameManagerController instance;
    public GameSettings config;
    public List<IngredientConfig> ingredients = new();
    public int dishIndex = 0;
    public IngredientConfig[] WantedIngredients {
        get {
            return config.dishes[dishIndex].ingredients;
        }
    }

    public DishConfig CurrentDish {
        get {
            return config.dishes[dishIndex];
        }
    }

    void InitPlayerSettings() {
        bool playerUseMotion = PlayerPrefs.GetInt("UseMotion") == 1;
        config.useMotion = playerUseMotion;
    }

    void CollectIngredients() {
        var dishes = config.dishes;
        foreach (var dish in dishes) {
            ingredients.AddRange(dish.ingredients);
        }
    }

    AudioSource sfx;
    [SerializeField] GameObject bgm;
    public void PlayAmbience(AudioClip sound) {
        if (sound == null) return;
        if (sound == sfx.clip) return;
        if (sfx.isPlaying) sfx.Stop();

        sfx.clip = sound;
        sfx.Play();
    }

    public void PlayBgm(AudioClip sound) {
        var player = bgm.GetComponent<AudioSource>();
        if (sound == null) return;
        if (sound == player.clip) return;
        if (player.isPlaying) player.Stop();

        player.clip = sound;
        player.Play();
    }

    public IndicatorController.State gameState;
    void OnIndicatorStateUpdate(IndicatorController.State state) {
        gameState = state;
    }

    void OnGameEnd() {
        sfx.PlayOneShot(config.timesUpSFX);
        KeepSpawnIngredients = false;
        
        var sceneController = GetComponent<SceneManagerController>();
        sceneController.BeforeLoadScene();

        IEnumerator GotoEndAsync() {
            yield return new WaitForSeconds(config.vfxTime);
            yield return sceneController.GotoEndScene();
        }

        StartCoroutine(GotoEndAsync());
    }

    public bool KeepSpawnIngredients = false;

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

        CollectIngredients();
        sfx = GetComponent<AudioSource>();
    }

    void Start()
    {
        InitPlayerSettings();

        TimerController.OnTimeout += OnGameEnd;
        IndicatorController.OnIndicatorStateUpdate += OnIndicatorStateUpdate;
    }

    void OnDestroy()
    {
        TimerController.OnTimeout -= OnGameEnd;
        IndicatorController.OnIndicatorStateUpdate -= OnIndicatorStateUpdate;
    }

}
