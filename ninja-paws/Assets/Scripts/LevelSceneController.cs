using UnityEngine;
using UnityEngine.AddressableAssets;
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
            Addressables.LoadSceneAsync("FruitNinja/FruitNinja", LoadSceneMode.Single);
        });
    }
}
