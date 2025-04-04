using UnityEngine;

namespace Assets.Scenes.FruitNinja.Scripts
{
    [CreateAssetMenu(fileName = "Spawnables", menuName = "FruitNinja/Spawnables")]
    public class Spawnables : ScriptableObject
    {
        public float speed = 15f;
        public float torque = 0.1f;
        public bool isBad = false;
    }
}


