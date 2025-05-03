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

    bool isSlicable = true;

    public void SetSlicable(bool isSlicable) {
        this.isSlicable = isSlicable;
    }

    public void StartLoading() {
        manager.GetComponent<SceneManagerController>().BeforeLoadScene();
    }
    public void GotoLevelsScene() {
        manager.GetComponent<SceneManagerController>().GotoLevelMenuScene();
    }

    public void GotoStartScene() {
        manager.GetComponent<SceneManagerController>().GotoStartScene();
    }

    public void PlayVFX()
    {
        var vfx = Instantiate(config.buttonVFX, transform.position, Quaternion.identity);
        vfx.transform.localPosition = new Vector3(transform.position.x, transform.position.y, relativeZ);
        vfx.GetComponent<ParticleSystem>().Play();

        IEnumerator DestroyVFXRoutine()
        {
            yield return new WaitForSeconds(.6f);
            onSliced.Invoke();
            Destroy(vfx);
        }
        StartCoroutine(DestroyVFXRoutine());
    }

    void Poof()
    {
        var sceneController = manager.GetComponent<SceneManagerController>();
        if (sceneController.isLoading) return;

        if (!isSlicable) return;

        PlayVFX();

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
