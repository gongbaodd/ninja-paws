using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class IngredientController : MonoBehaviour
{
    Rigidbody rb;
    GameManagerController manager;
    GameSettings gameConfig;
    Vector2 targetPos;
    IngredientConfig config;
    GameObject caughtVFX;
    [SerializeField] GameObject item;
    Vector2 CalculateForceDirection()
    {
        Vector2 pos = transform.position;
        Vector2 direction = (targetPos - pos).normalized;
        return direction;
    }
    Vector3 RandomTorque()
    {
        return new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
    }
    void Init()
    {
        var all = manager.ingredients.ToArray();
        var wanted = manager.WantedIngredients;
        var allWeight = gameConfig.allWeight;
        var wantedWeight = gameConfig.wantedWeight;

        bool isInWanted = Random.Range(0, allWeight + wantedWeight) < wantedWeight;

        if (isInWanted)
        {
            config = wanted[Random.Range(0, wanted.Length)];
        }
        else
        {
            config = all[Random.Range(0, all.Length)];
        }

        var isWanted = wanted.Contains(config);
        var wantedVFX = gameConfig.wantedVFX;
        var unWantedVFX = gameConfig.unWantedVFX;
        var position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        caughtVFX = Instantiate(isWanted ? wantedVFX : unWantedVFX, position, Quaternion.identity, transform);
        caughtVFX.SetActive(false);

        if (config.sprite) {
            item.GetComponent<SpriteRenderer>().sprite = config.sprite;
        }
        item.SetActive(true);
    }

    void Caught()
    {
        caughtVFX.SetActive(true);
        item.SetActive(false);
    }
    public void Spawn()
    {

        Init();
        rb.AddForce(CalculateForceDirection() * config.speed, ForceMode.Impulse);
        rb.AddTorque(RandomTorque() * config.torque, ForceMode.Impulse);
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        var target = GameObject.FindWithTag("Target");
        targetPos = target.transform.position;
    }
    void Start()
    {
        manager = GameManagerController.Instance;
        gameConfig = manager.config;

        Spawn();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var cursor = manager.GetComponent<CursorController>();
            if (cursor.IsDrawing)
            {
                Caught();
            }
        }
    }
}
