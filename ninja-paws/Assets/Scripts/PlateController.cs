using UnityEngine;

public class PlateController : MonoBehaviour
{
    GameManagerController manager;
    GameSettings config;
    public DishConfig dishConfig;
    [SerializeField] GameObject dishRenderer;
    [SerializeField] GameObject flag;

    void Start() 
    {
        manager = GameManagerController.Instance;
        config = manager.config;
        dishConfig = dishConfig == null ?  manager.CurrentDish : dishConfig;
    }

    void Update()
    {
        if (dishConfig.sprite) {
            dishRenderer.GetComponent<SpriteRenderer>().sprite = dishConfig.sprite;
        }

        switch (dishConfig.country) {
            case DishConfig.Country.Estonia:
                flag.GetComponent<Renderer>().material = config.EstoniaFlag;
                break;
            case DishConfig.Country.Latvia:
                flag.GetComponent<Renderer>().material = config.LatviaFlag;
                break;
            case DishConfig.Country.Lithuania:
                flag.GetComponent<Renderer>().material = config.LithuaniaFlag;
                break;
        }
    }
}
