using TMPro;
using UnityEngine;

public class DishController : MonoBehaviour
{
    GameManagerController manager;
    GameSettings config;

    DishConfig[] dishes;

    [SerializeField] GameObject dishRenderer;

    [SerializeField] TMP_Text label;

    [SerializeField] GameObject dishButton;

    [SerializeField] GameObject leftButton;

    [SerializeField] GameObject rightButton;

    [SerializeField] GameObject Items3;

    [SerializeField] GameObject Items4;

    [SerializeField] GameObject Items5;

    int index = 0;
    int length = 0;

    public void NextDish() {
        index = (index + 1) % length;
    }

    public void PreviousDish() {
        index = (index - 1 + length) % length;
    }

    public void SetDish() {
        manager.dishIndex = index;
    }

    void UpdateIngredients() {
        var Items = Items3;
        switch(dishes[index].ingredients.Length) {
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

        var dish = dishes[index];

        for (int i = 0; i < dish.ingredients.Length; i++) {
            var ingredient = dish.ingredients[i];
            var item = Items.transform.GetChild(i).gameObject;

            if (ingredient.sprite) {
                item.GetComponent<SpriteRenderer>().sprite = ingredient.sprite;
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
        dishes = config.dishes;
        length = dishes.Length;
    }

    void Update()
    {
        leftButton.SetActive(index > 0);
        rightButton.SetActive(index < length - 1);

        label.text = dishes[index].itemName;

        if (dishes[index].sprite) {
            dishRenderer.GetComponent<SpriteRenderer>().sprite = dishes[index].sprite;
        }

        Items3.SetActive(dishes[index].ingredients.Length == 3);
        Items4.SetActive(dishes[index].ingredients.Length == 4);
        Items5.SetActive(dishes[index].ingredients.Length == 5);

        UpdateIngredients();
    }
}
