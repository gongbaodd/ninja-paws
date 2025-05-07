using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlateController : MonoBehaviour
{
    GameManagerController manager;
    GameSettings config;
    public DishConfig dishConfig;
    [SerializeField] GameObject dishRenderer;
    [SerializeField] TMP_Text label;
    [SerializeField] GameObject Items3;
    [SerializeField] GameObject Items4;
    [SerializeField] GameObject Items5;

    void UpdateIngredients()
    {
        var Items = Items3;
        switch (dishConfig.ingredients.Length)
        {
            case 3:
                Items = Items3;
                break;
            case 4:
                Items = Items4;
                break;
            case 5:
                Items = Items5;
                break;
        }

        var dish = dishConfig;

        for (int i = 0; i < dish.ingredients.Length; i++)
        {
            var ingredient = dish.ingredients[i];
            var item = Items.transform.GetChild(i).gameObject;

            if (ingredient.sprite)
            {
                item.GetComponent<Image>().sprite = ingredient.sprite;
            }
            var label = item.transform.GetChild(0).GetComponent<TMP_Text>();
            label.text = ingredient.itemName;
        }
    }

    void Awake()
    {
        Items3.SetActive(false);
        Items4.SetActive(false);
        Items5.SetActive(false);
    }


    void Start()
    {
        manager = GameManagerController.Instance;
        config = manager.config;
        dishConfig = dishConfig == null ? manager.CurrentDish : dishConfig;
    }

    void Update()
    {
        if (dishConfig.sprite)
        {
            dishRenderer.GetComponent<SpriteRenderer>().sprite = dishConfig.sprite;
        }

        label.text = dishConfig.itemName;

        Items3.SetActive(dishConfig.ingredients.Length == 3);
        Items4.SetActive(dishConfig.ingredients.Length == 4);
        Items5.SetActive(dishConfig.ingredients.Length == 5);

        UpdateIngredients();
    }
}
