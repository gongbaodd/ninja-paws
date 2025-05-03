using UnityEngine;

public class ButtonController : MonoBehaviour
{
    GameManagerController manager;
    GameSettings config;

    [SerializeField] float relativeZ = -2f;
    void Poof()
    {
        var vfx = Instantiate(config.buttonVFX, transform.position, Quaternion.identity);
        vfx.transform.localPosition = new Vector3(transform.position.x, transform.position.y, relativeZ);

        vfx.GetComponent<ParticleSystem>().Play();
        Destroy(gameObject, .8f);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var cursor = manager.GetComponent<CursorController>();

            if (cursor.IsDrawing)
            {
                Poof();
            }
        }
    }

    void Start()
    {
        manager = GameManagerController.Instance;
        config = manager.config;
    }
}
