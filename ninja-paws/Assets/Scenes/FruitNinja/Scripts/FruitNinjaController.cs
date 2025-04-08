using UnityEngine;

namespace Assets.Scenes.FruitNinja.Scripts
{

    [RequireComponent(
        typeof(SpawnFruitController),
        typeof(CursorController)
    )]
    public class FruitNinjaController : MonoBehaviour
    {
        public void Win()
        {
            throw new System.NotImplementedException("Need to addItem to Inventory! Wait the Inventory to be implemented!");
        }
    }

}
