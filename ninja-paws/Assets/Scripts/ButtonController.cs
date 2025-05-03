using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ButtonController : MonoBehaviour
{
    GameManagerController manager;
    GameSettings config;
    [SerializeField] float relativeZ = -2f;
    [SerializeField] UnityEvent onSlice;
    [SerializeField] UnityEvent onSliced;
    AudioSource sfx;
    void Poof()
    {
        var vfx = Instantiate(config.buttonVFX, transform.position, Quaternion.identity);
        vfx.transform.localPosition = new Vector3(transform.position.x, transform.position.y, relativeZ);
        vfx.GetComponent<ParticleSystem>().Play();
        IEnumerator DestroyVFXRoutine()
        {
            yield return new WaitForSeconds(1f);
            onSliced.Invoke();
            Destroy(vfx);
        }
        StartCoroutine(DestroyVFXRoutine());
        
        onSlice.Invoke();

        sfx.PlayOneShot(sfx.clip);
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
        sfx = GetComponent<AudioSource>();
    }
}
