using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class LevelSceneController : MonoBehaviour
{
    UIDocument ui;
    void Awake()
    {
        ui = GetComponent<UIDocument>();

        var level1Button = ui.rootVisualElement.Q<Button>("level1Button");

        level1Button.RegisterCallback<ClickEvent>(evt => {
            SceneManager.LoadScene("FruitNinja", LoadSceneMode.Single);
        });
    }
}
