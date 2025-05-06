using TMPro;
using UnityEngine;

public class MotionButtonController : MonoBehaviour
{
    GameManagerController manager;
    GameSettings config;
    [SerializeField] GameObject loading;
    [SerializeField] GameObject useMotionSprite;
    [SerializeField] GameObject useCursorSpriite;
    [SerializeField] TMP_Text content;

    public static bool isMotion = false;

    void RenderSprite() {
        useMotionSprite.SetActive(!manager.useMotion);
        useCursorSpriite.SetActive(manager.useMotion);

        if (manager.useMotion) {
            content.text = "use cursor";
        } else {
            content.text = "use motion";
        }
    }

    public void OnToggleButtonClicked() {
        if (manager.useMotion)
        {
            // goto cursor
            isMotion = false;
            manager.useMotion = false;
            loading.SetActive(false);
            OnCursor?.Invoke();
        } else {
            // goto motion
            manager.useMotion = true;
            loading.SetActive(true);
        }
    }

    void Awake()
    {
        loading.SetActive(false);
    }

    void Start()
    {
        manager = GameManagerController.Instance;
        config = manager.config;
    }

    public static event System.Action OnMotion;
    public static event System.Action OnCursor;

    void Update()
    {
        RenderSprite();

        if (HandTrackingReceiver.isReceivingData) {
            loading.SetActive(false);
            isMotion = true;
            OnMotion?.Invoke();
        }
    }
}
