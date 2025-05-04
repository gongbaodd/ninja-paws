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

    int index = 0;
    int length = 0;

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

        plate.GetComponent<PlateController>().dishConfig = dishes[index];


    }
}
