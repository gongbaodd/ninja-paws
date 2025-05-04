using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scenes.FruitNinja.Scripts
{
    public class SpawnFruitController : MonoBehaviour
    {
        [SerializeField] private GameObject target;

        [SerializeField] private GameConfig config;

        public GameConfig Config => config;

        private bool keepSpawning = true;

        private IEnumerator spawnHandler;

        public Vector2 CalculateForceDirection(Vector2 fruitPos)
        {
            Vector2 targetPos = target.transform.position;
            Vector2 direction = (targetPos - fruitPos).normalized;
            return direction;
        }

        private IEnumerator SpawnFruitRoutine()
        {
            while (keepSpawning)
            {
                yield return new WaitForSeconds(config.spawnDelay);
                SpawnFruit();
            }
        }

        private void SpawnFruit()
        {
            var fruits = config.fruits;
            var bombs = config.bombs;

            var bombWeight = config.bombWeight;
            var fruitWeight = config.fruitWeight;

            var spawnWidth = config.spawnWidth;
            var spawnHeight = config.spawnHeight;

            bool isSpawnBomb = Random.Range(0, bombWeight + fruitWeight) < bombWeight;

            if (isSpawnBomb)
            {
                int index = Random.Range(0, bombs.Count);
                var spawnable = Instantiate(bombs[index], new Vector3(Random.Range(-spawnWidth, spawnWidth), spawnHeight, 0), Quaternion.identity);

                spawnable.GetComponent<FruitController>().OnFruitDestroyed += Boom;
            }
            else
            {
                int index = Random.Range(0, fruits.Count);
                var spawnable = Instantiate(fruits[index], new Vector3(Random.Range(-spawnWidth, spawnWidth), spawnHeight, 0), Quaternion.identity);

                spawnable.GetComponent<FruitController>().OnFruitDestroyed += Poof;
            }
        }

        private void Poof(Vector2 fruitPos)
        {
            var poofPrefab = config.poofPrefab;
            var poof = Instantiate(poofPrefab, new Vector3(fruitPos.x, fruitPos.y, 0), Quaternion.identity);

            poof.GetComponent<ParticleSystem>().Play();

            Destroy(poof, 1f);
        }

        private void Boom(Vector2 boomPos) {
            var boomPrefab = config.boomPrefab;
            var boom = Instantiate(boomPrefab, new Vector3(boomPos.x, boomPos.y, 0), Quaternion.identity);

            boom.GetComponent<ParticleSystem>().Play();
            Destroy(boom, 1f);
        }

        private void StopSpawning()
        {
            keepSpawning = false;
            StopCoroutine(spawnHandler);
        }

        void Start()
        {
            spawnHandler = SpawnFruitRoutine();
            StartCoroutine(spawnHandler);

            TimerController.OnTimerEnd += StopSpawning;
        }

        void OnDestroy()
        {
            TimerController.OnTimerEnd -= StopSpawning;
        }
    }
}
