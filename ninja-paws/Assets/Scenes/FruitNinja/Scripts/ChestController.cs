using UnityEngine;

namespace Assets.Scenes.FruitNinja.Scripts
{
    [RequireComponent(typeof(GetManager), typeof(BoxCollider))]
    public class ChestController : MonoBehaviour
    {
        Vector3 targetPos;
        Collider chestCollider;
        bool isMoving = false;

        void OnTimerEnd() {
            isMoving = true;
            chestCollider.enabled = true;
        }

        void MoveToCenter() {
            transform.position = Vector3.Lerp(targetPos, transform.position, 0.05f);
        }

        void Start()
        {
            chestCollider = GetComponent<BoxCollider>();
            chestCollider.enabled = false;

            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, Camera.main.nearClipPlane);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenCenter);
            targetPos = new Vector3(worldPos.x, worldPos.y, transform.position.z);

            TimerController.OnTimerEnd += OnTimerEnd;
        }

        void OnDestroy()
        {
            TimerController.OnTimerEnd -= OnTimerEnd;            
        }

        void Update()
        {
            if (isMoving) {
                MoveToCenter();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player")) {
                var cursorController = GetComponent<GetManager>().GameManager.GetComponent<CursorController>();
                if (cursorController.IsDrawing) {
                    var manager = GetComponent<GetManager>().GameManager;
                    var controller = manager.GetComponent<FruitNinjaController>();
                    controller.Win();
                }
            }
        }

    }
}
