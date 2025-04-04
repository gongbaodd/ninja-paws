using UnityEngine;

namespace Assets.Scenes.FruitNinja.Scripts
{
    public class RemovalController : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Destroy(other.gameObject);
        }
    }
}