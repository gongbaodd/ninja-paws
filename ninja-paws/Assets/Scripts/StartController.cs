using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class StartController : MonoBehaviour
{

    UIDocument ui;
    void Awake()
    {
        ui = GetComponent<UIDocument>();

        var PlayButton = ui.rootVisualElement.Q<Button>("playButton");

        PlayButton.RegisterCallback<ClickEvent>(evt =>
        {
            SceneManager.LoadScene("Levels", LoadSceneMode.Single);
        });
    }
}
