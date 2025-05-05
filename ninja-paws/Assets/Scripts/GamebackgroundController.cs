using UnityEngine;

public class GamebackgroundController : MonoBehaviour
{
    GameManagerController manager;
    GameSettings config;
    DishConfig.Country country;

    void RenderBackground() {
        switch (country)
        {
            case DishConfig.Country.Estonia:
                gameObject.GetComponent<Renderer>().material = config.EstoniaBackground;
                break;
            case DishConfig.Country.Latvia:
                gameObject.GetComponent<Renderer>().material = config.LatviaBackground;
                break;
            case DishConfig.Country.Lithuania:
                gameObject.GetComponent<Renderer>().material = config.LithuaniaBackground;
                break;
        }
    }
    
    void Start()
    {
        manager = GameManagerController.Instance;
        config = manager.config;

        var dish = manager.CurrentDish;
        country = dish.country;

        RenderBackground();
    }
}
