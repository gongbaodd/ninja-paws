using TMPro;
using UnityEngine;

public class ResultBoardController : MonoBehaviour
{
    [SerializeField] TMP_Text redoLabel;
    [SerializeField] GameObject Items;
    [SerializeField] GameObject NextButton;
    GameManagerController manager;
    IndicatorController.State gameState;
    void RenderItems()
    {
        var items = gameState.checkItems;

        if (items == null) return;

        var ingredients = manager.ingredients;

        if (items.Count < 5)
        {
            Items.transform.GetChild(4).gameObject.SetActive(false);
        }

        if (items.Count < 4)
        {
            Items.transform.GetChild(3).gameObject.SetActive(false);
        }

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var sprite = ingredients.Find(x => x.itemName == item.itemName).sprite;
            var label = Items.transform.GetChild(i).transform.Find("label").GetComponent<TMP_Text>();
            label.text = item.count.ToString();

            if (ingredients[i].sprite)
            {
                Items.transform.GetChild(i).transform.Find("sprite").GetComponent<SpriteRenderer>().sprite = sprite;
            }
        }

    }
    void Start()
    {
        manager = GameManagerController.Instance;
        gameState = manager.gameState;

        redoLabel.text = $"{gameState.finishedCount} finished, {gameState.redoCount} ruined";

        NextButton.SetActive(gameState.finishedCount > 0);

        RenderItems();
    }
}
