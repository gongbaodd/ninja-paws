using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(AudioSource))]
public class IngredientController : MonoBehaviour
{
    Rigidbody rb;
    SphereCollider sphereCollider;
    AudioSource sfx;
    AudioClip caughtClip;
    GameObject caughtVFX;
    Vector2 targetPos;
    float removeY;
    IngredientConfig config;
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

    GameManagerController manager;
    GameSettings gameConfig;
    void InitItems()
    {
        manager = GameManagerController.Instance;
        var all = manager.ingredients.ToArray();
        var wanted = manager.WantedIngredients;
        
        gameConfig = manager.config;
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

        if (config.sprite)
        {
            item.GetComponent<SpriteRenderer>().sprite = config.sprite;
        }
        
        var spawnWidth = gameConfig.spawnWidth;
        var spawnHeight = gameConfig.spawnHeight;
        transform.position = new Vector3(Random.Range(-spawnWidth, spawnWidth), spawnHeight, transform.position.z);

        sphereCollider.enabled = true;

        item.SetActive(true);

        var isWanted = wanted.Contains(config);
        var wantedVFX = gameConfig.wantedVFX;
        var unWantedVFX = gameConfig.unWantedVFX;
        var position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        if (caughtVFX)
        {
            Destroy(caughtVFX);
        }

        caughtVFX = Instantiate(isWanted ? wantedVFX : unWantedVFX, position, Quaternion.identity, transform);
        caughtVFX.SetActive(false);

        caughtClip = isWanted ? gameConfig.wantedSFX : gameConfig.unWantedSFX;
    }

    public static event System.Action<GameObject> OnDeactived;

    public static event System.Action<IngredientConfig> OnCaught;

    void Caught()
    {
        sphereCollider.enabled = false;
        caughtVFX.SetActive(true);
        item.SetActive(false);
        sfx.PlayOneShot(caughtClip);

        OnCaught?.Invoke(config);

        IEnumerator WaitForDeactivedRoutine() {
            yield return new WaitForSeconds(gameConfig.vfxTime);
            OnDeactived?.Invoke(gameObject);
            gameObject.SetActive(false);
        }

        StartCoroutine(WaitForDeactivedRoutine());
    }
    public void Spawn()
    {

        InitItems();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.rotation = Quaternion.identity;
        
        rb.AddForce(CalculateForceDirection() * config.speed, ForceMode.Impulse);
        rb.AddTorque(RandomTorque() * config.torque, ForceMode.Impulse);

        sfx.PlayOneShot(gameConfig.spawnSFX);
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();

        var target = GameObject.FindWithTag("Target");
        targetPos = target.transform.position;

        var removeBar = GameObject.FindWithTag("RemoveBar");
        removeY = removeBar.transform.position.y;

        sfx = GetComponent<AudioSource>();
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
    void Update()
    {
        if (transform.position.y < removeY) {
            OnDeactived?.Invoke(gameObject);
            gameObject.SetActive(false);
        }
    }
}
