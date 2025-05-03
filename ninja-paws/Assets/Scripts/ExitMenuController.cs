using UnityEngine;

public class ExitMenuController : MonoBehaviour
{
    [SerializeField] private GameObject content;

    public void ToggleMenu(bool value)
    {
        content.SetActive(value);
    }

    public void PauseGame() {
        // TODO
    }

    public void ResumeGame() {
        // TODO
    }

    void Awake()
    {
        content.SetActive(false);
    }
}
