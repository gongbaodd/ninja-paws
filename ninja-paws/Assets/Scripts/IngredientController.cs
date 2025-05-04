using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class IngredientController : MonoBehaviour
{
    Rigidbody rb;
    GameManagerController manager;
    GameSettings gameConfig;
    Vector2 targetPos;
    IngredientConfig config;

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

        bool isWanted = Random.Range(0, allWeight + wantedWeight) < wantedWeight;

        if (isWanted)
        {
            config = wanted[Random.Range(0, wanted.Length)];
        }
        else
        {
            config = all[Random.Range(0, all.Length)];
        }
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

}
