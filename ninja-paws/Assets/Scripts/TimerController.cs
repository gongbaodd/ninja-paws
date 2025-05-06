using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    GameManagerController manager;
    GameSettings config;
    float timeLeft;
    [SerializeField] TMP_Text label;
    public static System.Action OnTimeout;
    void Start()
    {
        manager = GameManagerController.Instance;
        config = manager.config;
        timeLeft = config.timeInSeconds;
        label.text = "";
    }

    void Update()
    {
        if (!manager.KeepSpawnIngredients) return;

        if (timeLeft < 0) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft < config.showCountDownTime + 1) {
            label.text = timeLeft.ToString("0");
        }

        if (timeLeft <= 0) {
            OnTimeout?.Invoke();
        }
    }
}
