using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scenes.FruitNinja.Scripts
{
    [RequireComponent(typeof(UIDocument), typeof(GetManager))]
    public class TimerController : MonoBehaviour
    {
        GameObject manager;
        float timeLeft;
        Label label;

        public static event System.Action OnTimerEnd;

        IEnumerator TimerCoroutine()
        {
            while (timeLeft > 0)
            {
                timeLeft -= 1;

                if (timeLeft < 6) {
                    label.text = timeLeft.ToString("0");
                }

                if (timeLeft == 1)
                {
                    label.text = "0";
                    OnTimerEnd?.Invoke();
                }

                yield return new WaitForSeconds(1f);
            }
        }



        void Start()
        {
            manager = GetComponent<GetManager>().GameManager;

            var config = manager.GetComponent<SpawnFruitController>().Config;
            var ui = GetComponent<UIDocument>();
            var root = ui.rootVisualElement;
            label = root.Q<Label>("timerLabel");

            timeLeft = config.timeInSeconds;

            StartCoroutine(TimerCoroutine());
        }
    }
}


