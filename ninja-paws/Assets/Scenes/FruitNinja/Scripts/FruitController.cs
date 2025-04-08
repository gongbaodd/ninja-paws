using System.Collections;
using UnityEngine;

namespace Assets.Scenes.FruitNinja.Scripts
{

    [RequireComponent(typeof(Rigidbody), typeof(GetManager))]
    public class FruitController : MonoBehaviour
    {
        private Rigidbody rb;
        protected GameObject gameManager;

        [SerializeField] private Spawnables config;

        public event System.Action<Vector2> OnFruitDestroyed;
        protected virtual void Start()
        {
            gameManager = GetComponent<GetManager>().GameManager;

            if (config == null)
            {
                throw new System.Exception("Spawnables config not assigned. Please assign a Spawnables object in the inspector.");
            }

            var spawnContoller = gameManager.GetComponent<SpawnFruitController>();
            var speed = config.speed;
            var torque = config.torque;

            rb = GetComponent<Rigidbody>();
            rb.AddForce(spawnContoller.CalculateForceDirection(transform.position) * speed, ForceMode.Impulse);
            rb.AddTorque(RandomTorque() * torque, ForceMode.Impulse);
        }

        private Vector3 RandomTorque()
        {
            return new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var cursorController = gameManager.GetComponent<CursorController>();

                if (cursorController.IsDrawing)
                {
                    Vector2 fruitPos = transform.position;
                    OnFruitDestroyed?.Invoke(fruitPos);

                    Destroy(gameObject);
                }
            }
        }

    }
}