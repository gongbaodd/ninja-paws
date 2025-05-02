using UnityEngine;

public enum Country {
    Estonia,
    Latvia,
    Lithuania,
}
[CreateAssetMenu(fileName = "DishConfig", menuName = "Configs/dish")]
public class DishConfig: WithSpriteConfig
{
    public IngredientConfig[] ingredients;

    public Country country;
}