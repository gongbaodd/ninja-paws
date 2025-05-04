using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    struct CheckItem {
        public string itemName;
        public bool isChecked;
    }
    readonly List<CheckItem> checkItems = new();
    bool isCollectSfxPlayed = false;

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
            var itemName = dish.ingredients[i].itemName;
            var item = Items.transform.GetChild(i).gameObject;
            var label = item.transform.GetChild(0).GetComponent<TMP_Text>();
            label.text = itemName;
            item.name = itemName;

            checkItems.Add(new CheckItem { itemName = itemName, isChecked = false });
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
            checkItems[checkItemIndex] = checkItem;

            if (checkItems.TrueForAll(x => x.isChecked))
            {
                if (isCollectSfxPlayed) return;

                IEnumerator PlaySfxRoutine()
                {
                    yield return new WaitForSeconds(config.vfxTime);
                    sfx.PlayOneShot(config.allCollectedSFX);
                    isCollectSfxPlayed = true;
                }

                StartCoroutine(PlaySfxRoutine());
            }
        }
    }

    void Awake()
    {
        sfx = GetComponent<AudioSource>();
    }

    void Start()
    {
        manager = GameManagerController.Instance;
        config = manager.config;

        var dishes = config.dishes;
        dish = dishes[manager.dishIndex];

        InitItems();

        IngredientController.OnCaught += OnIngredientCaught;
    }

    void OnDestroy()
    {
        IngredientController.OnCaught -= OnIngredientCaught;
    }
}
