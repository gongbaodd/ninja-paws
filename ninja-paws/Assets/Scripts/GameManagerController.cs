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

    void OnGameEnd() {
        sfx.PlayOneShot(config.timesUpSFX);
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
    }

    void OnDestroy()
    {
        TimerController.OnTimeout -= OnGameEnd;
    }

}
