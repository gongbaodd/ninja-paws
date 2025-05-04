using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class SpawnerController : MonoBehaviour
{
    LinkedPool<GameObject> pool;
    GameManagerController manager;
    GameSettings config;
    int poolSize;
    [SerializeField] GameObject ingredient;
    
    GameObject CreatePooledItem() {
        var obj = Instantiate(ingredient);
        obj.SetActive(false);
        return obj;
    }

    void OnTakeFromPool(GameObject obj) {
        obj.SetActive(true);
        var controller = obj.GetComponent<IngredientController>();
        controller.Spawn();
    }

    void OnReturnedToPool(GameObject obj) {
        obj.SetActive(false);
    }

    void OnDestroyPoolObject(GameObject obj) {
        Destroy(obj);
    }

    void InitPool()
    {
        bool collectionChecks = true;
        pool = new LinkedPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, poolSize);
    }

    IEnumerator SpawnRoutine() {
        while(true) {
            yield return new WaitForSeconds(config.spawnDelay);
            if (manager.KeepSpawnIngredients) {
                pool.Get();
            }
        }
    }

    void OnItemDeactived(GameObject obj) {
        pool.Release(obj);
    }
    void Start()
    {
        manager = GameManagerController.Instance;
        config = manager.config;
        poolSize = config.ingredientsPoolSize;

        InitPool();
        IngredientController.OnDeactived += OnItemDeactived;

        manager.KeepSpawnIngredients = true;
        StartCoroutine(SpawnRoutine());
    }

    void OnDestroy()
    {
        IngredientController.OnDeactived -= OnItemDeactived;
        pool.Dispose();
    }
}
