using UnityEngine;

public class MotionButtonController : MonoBehaviour
{
    GameManagerController manager;
    GameSettings config;
    [SerializeField] GameObject loading;
    [SerializeField] GameObject useMotionSprite;
    [SerializeField] GameObject useCursorSpriite;

    public static bool isMotion = false;

    void RenderSprite() {
        useMotionSprite.SetActive(!config.useMotion);
        useCursorSpriite.SetActive(config.useMotion);
    }

    public void OnToggleButtonClicked() {
        if (config.useMotion) {
            config.useMotion = false;
            loading.SetActive(false);
        } else {
            config.useMotion = true;
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

    void Update()
    {
        RenderSprite();

        if (HandTrackingReceiver.isReceivingData) {
            loading.SetActive(false);
            isMotion = true;
        }
    }
}
