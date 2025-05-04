using UnityEngine;

public class ExitMenuController : MonoBehaviour
{
    [SerializeField] private GameObject content;

    GameManagerController manager;

    public void ToggleMenu(bool value)
    {
        content.SetActive(value);
    }

    public void PauseGame() {
        manager.KeepSpawnIngredients = false;
    }

    public void ResumeGame() {
        manager.KeepSpawnIngredients = true;
    }

    void Awake()
    {
        content.SetActive(false);
    }

    void Start()
    {
        manager = GameManagerController.Instance;
    }
}
