using UnityEngine;

public class IndicatorController : MonoBehaviour
{
    GameManagerController manager;
    GameSettings config;
    DishConfig dish;
    [SerializeField] GameObject Items3;
    [SerializeField] GameObject Items4;
    [SerializeField] GameObject Items5;

    void Start()
    {
        manager = GameManagerController.Instance;
        config = manager.config;
        var dishes = config.dishes;
        dish = dishes[manager.dishIndex];

        Items3.SetActive(dish.ingredients.Length == 3);
        Items4.SetActive(dish.ingredients.Length == 4);
        Items5.SetActive(dish.ingredients.Length == 5);
    }
}
