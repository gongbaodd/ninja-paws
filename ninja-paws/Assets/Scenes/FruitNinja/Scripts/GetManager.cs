using UnityEngine;

namespace Assets.Scenes.FruitNinja.Scripts
{
    public class GetManager : MonoBehaviour
    {
        private GameObject manager;

        public GameObject GameManager => manager;

        void Awake()
        {
            manager = GameObject.FindWithTag("GameController");

            if (manager == null)
            {
                throw new System.Exception("GameManager not found in the scene. Please add a GameManager object with the 'GameController' tag.");
            }
        }

    }
}