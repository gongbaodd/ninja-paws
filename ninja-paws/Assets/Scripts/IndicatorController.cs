using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class IndicatorController : MonoBehaviour
{
    GameManagerController manager;
    GameSettings config;
    DishConfig dish;
    GameObject Items;
    AudioSource sfx;
    [SerializeField] GameObject Items3;
    [SerializeField] GameObject Items4;
    [SerializeField] GameObject Items5;
    [SerializeField] GameObject plus1;

    public struct CheckItem {
        public string itemName;
        public bool isChecked;
        public int count;
    }
    readonly List<CheckItem> checkItems = new();
    GameObject redoVFX;
    int redoCount = 0;
    int finishedCount = 0;
    void InitItems()
    {
        Items3.SetActive(dish.ingredients.Length == 3);
        Items4.SetActive(dish.ingredients.Length == 4);
        Items5.SetActive(dish.ingredients.Length == 5);

        switch (dish.ingredients.Length)
        {
            case 3:
                Items = Items3;
                break;
            case 4:
                Items = Items4;
                break;
            case 5:
                Items = Items5;
                break;
        }

        for (int i = 0; i < dish.ingredients.Length; i++)
        {
            var ingredient = dish.ingredients[i];
            var itemName = ingredient.itemName;
            var item = Items.transform.GetChild(i).gameObject;
            var label = item.transform.GetChild(0).GetComponent<TMP_Text>();
            label.text = itemName;
            item.name = itemName;

            item.GetComponent<Image>().sprite = ingredient.sprite;

            checkItems.Add(new CheckItem { itemName = itemName, isChecked = false, count = 0 });
        }
    }

    public struct State {
        public List<CheckItem> checkItems;
        public int redoCount;
        public int finishedCount;
    }
    public static event System.Action<State> OnIndicatorStateUpdate;

    void RemoveOneDish()
    {
        for (int i = 0; i < checkItems.Count; i++)
        {
            var checkItem = checkItems[i];

            checkItem.count--;
            if (checkItem.count < 0) {
                checkItem.count = 0;
            }

            checkItem.isChecked = checkItem.count > 0;
            checkItems[i] = checkItem;

            var indicateItem = Items.transform.GetChild(i).gameObject;
            var check = indicateItem.transform.Find("check").gameObject;
            check.SetActive(checkItem.isChecked);
        }
    }

    void OnIngredientCaught(IngredientConfig itemConfig)
    {
        var item = Items.transform.Find(itemConfig.itemName);

        if (item)
        {
            var obj = item.gameObject;
            var check = obj.transform.Find("check").gameObject;
            check.SetActive(true);

            var checkItemIndex = checkItems.FindIndex(x => x.itemName == itemConfig.itemName);
            var checkItem = checkItems[checkItemIndex];
            checkItem.isChecked = true;
            checkItem.count++;
            checkItems[checkItemIndex] = checkItem;

            if (checkItems.TrueForAll(x => x.isChecked))
            {
                finishedCount++;
                plus1.SetActive(true);
                RemoveOneDish();

                IEnumerator PlaySfxRoutine()
                {
                    yield return new WaitForSeconds(config.vfxTime);
                    sfx.PlayOneShot(config.allCollectedSFX);
                    yield return new WaitForSeconds(config.vfxTime);
                    plus1.SetActive(false);
                }

                StartCoroutine(PlaySfxRoutine());
            }
        } else {
            IEnumerator PlaySfxRoutine()
            {
                yield return new WaitForSeconds(config.vfxTime);
                sfx.PlayOneShot(config.redoSFX);
                redoVFX.SetActive(true);
                redoVFX.GetComponent<ParticleSystem>().Play();
                yield return DeactiveRedoVFXRoutine();
            }

            IEnumerator DeactiveRedoVFXRoutine() {
                yield return new WaitForSeconds(config.vfxTime);
                redoVFX.SetActive(false);
            }

            RemoveOneDish();
        
            redoCount++;

            StartCoroutine(PlaySfxRoutine());
        }

        OnIndicatorStateUpdate?.Invoke(new State { checkItems = checkItems, redoCount = redoCount, finishedCount = finishedCount });
    }

    void Awake()
    {
        sfx = GetComponent<AudioSource>();
        plus1.SetActive(false);
    }

    void Start()
    {
        manager = GameManagerController.Instance;
        config = manager.config;

        var dishes = config.dishes;
        dish = dishes[manager.dishIndex];

        InitItems();

        var vfxPos = new Vector3(Items.transform.position.x, Items.transform.position.y, 0f);
        redoVFX = Instantiate(config.redoVFX, vfxPos, Quaternion.identity);
        redoVFX.SetActive(false);

        IngredientController.OnCaught += OnIngredientCaught;
    }

    void OnDestroy()
    {
        IngredientController.OnCaught -= OnIngredientCaught;
    }
}
