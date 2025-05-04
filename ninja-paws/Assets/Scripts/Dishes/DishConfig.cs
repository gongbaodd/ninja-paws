using UnityEngine;


[CreateAssetMenu(fileName = "DishConfig", menuName = "Configs/dish")]
public class DishConfig : WithSpriteConfig
{
    public IngredientConfig[] ingredients;

    public Country country;

    public enum Country
    {
        Estonia,
        Latvia,
        Lithuania,
    }
}