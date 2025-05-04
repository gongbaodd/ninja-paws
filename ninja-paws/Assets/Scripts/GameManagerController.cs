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
            sceneController.GotoEndScene();
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

        InitPlayerSettings();
        CollectIngredients();
        sfx = GetComponent<AudioSource>();
    }

    void Start()
    {
        TimerController.OnTimeout += OnGameEnd;
        IndicatorController.OnIndicatorStateUpdate += OnIndicatorStateUpdate;
    }

    void OnDestroy()
    {
        TimerController.OnTimeout -= OnGameEnd;
        IndicatorController.OnIndicatorStateUpdate -= OnIndicatorStateUpdate;
    }

}
