using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class ResultBoardController : MonoBehaviour
{
    [SerializeField] TMP_Text redoLabel;
    [SerializeField] GameObject Items;
    [SerializeField] GameObject NextButton;
    [SerializeField] TMP_Text ManagerComment;
    GameManagerController manager;
    GameSettings config;
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

    async Task<string> GetManagerComment()
    {
        var dishConfig = config.dishes[manager.dishIndex];
        var name = dishConfig.itemName.Replace(" ", "_");
        var url = $"https://ninja-paws-server.solitary-forest-8d89.workers.dev/review/{name}?finishedCount={gameState.finishedCount}&ruinedCount={gameState.redoCount}";

        try
        {
            foreach (var item in gameState.checkItems)
            {
                if (item.count > 0)
                {
                    url += $"&{item.itemName}={item.count}";
                }
            }
        }
        catch
        {
            print("no game state");
        }



        using UnityWebRequest request = UnityWebRequest.Get(url);

        var asyncOpteration = request.SendWebRequest();

        while (!asyncOpteration.isDone)
        {
            await Task.Yield();
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            return request.downloadHandler.text;
        }

        return null;
    }

    async Task RenderComment()
    {
        string comment = await GetManagerComment();
        if (comment != null)
        {
            ManagerComment.text = $"Health Inspector: \n {comment}";
        }
    }

    public void GoNext()
    {
        var sceneManager = manager.GetComponent<SceneManagerController>();
        var currIndex = manager.dishIndex;
        if (currIndex < manager.config.dishes.Length - 1)
        {
            manager.dishIndex = currIndex + 1;

            if (gameState.finishedCount > 0 && manager.lastUnlockedDishIndex == currIndex)
            {
                manager.lastUnlockedDishIndex = currIndex + 1;
            }

            _ = sceneManager.GotoLevelMenuScene();
        }
        else
        {
            _ = sceneManager.GotoWinScene();
        }
    }
    async void Start()
    {
        manager = GameManagerController.Instance;
        config = manager.config;
        gameState = manager.gameState;

        await RenderComment();

        redoLabel.text = $"{gameState.finishedCount} finished, {gameState.redoCount} ruined";

        NextButton.SetActive(gameState.finishedCount > 0);

        RenderItems();
    }
}
