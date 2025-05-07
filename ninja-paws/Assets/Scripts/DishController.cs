using TMPro;
using UnityEngine;

public class DishController : MonoBehaviour
{
    GameManagerController manager;
    GameSettings config;
    DishConfig[] dishes;
    [SerializeField] GameObject leftButton;
    [SerializeField] GameObject rightButton;
    [SerializeField] GameObject plate;
    [SerializeField] GameObject lockObj;
    [SerializeField] GameObject PlayButton;

    public int index = 0;
    public int length = 0;

    public void NextDish()
    {
        index = (index + 1) % length;
    }

    public void PreviousDish()
    {
        index = (index - 1 + length) % length;
    }

    public void SetDish()
    {
        manager.dishIndex = index;
    }

    void Awake()
    {
        PlayButton.SetActive(false);
    }

    void Start()
    {
        manager = GameManagerController.Instance;
        config = manager.config;
        dishes = config.dishes;
        length = dishes.Length;

        index = manager.dishIndex;
    }

    void Update()
    {
        leftButton.SetActive(index > 0);
        rightButton.SetActive(index < length - 1);

        plate.GetComponent<PlateController>().dishConfig = dishes[index];

        bool isLocked = index > manager.lastUnlockedDishIndex;
        lockObj.SetActive(isLocked);
        PlayButton.SetActive(!isLocked);
    }
}
