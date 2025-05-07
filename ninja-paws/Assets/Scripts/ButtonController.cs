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

    public void SetSlicable(bool isSlicable)
    {
        this.isSlicable = isSlicable;
    }

    public void StartLoading()
    {
        manager.GetComponent<SceneManagerController>().BeforeLoadScene();
    }

    public void GotoIntroScene()
    {
        _ = manager.GetComponent<SceneManagerController>().GotoIntroScene();
    }
    public void GotoLevelsScene()
    {
        _ = manager.GetComponent<SceneManagerController>().GotoLevelMenuScene();
    }

    public void GotoStartScene()
    {
        _ = manager.GetComponent<SceneManagerController>().GotoStartScene();
    }

    public void GotoGameScene()
    {
        _ = manager.GetComponent<SceneManagerController>().GotoGameScene();
    }

    public void GotoWinScene()
    {
        _ = manager.GetComponent<SceneManagerController>().GotoWinScene();
    }

    public void PlayVFX()
    {
        var vfx = Instantiate(config.buttonVFX, transform.position, Quaternion.identity);
        vfx.transform.localPosition = new Vector3(transform.position.x, transform.position.y, relativeZ);
        vfx.GetComponent<ParticleSystem>().Play();

        IEnumerator DestroyVFXRoutine()
        {
            yield return new WaitForSeconds(config.vfxTime);
            onSliced.Invoke();
            Destroy(vfx);
        }
        StartCoroutine(DestroyVFXRoutine());
    }

    public void PlaySFX()
    {
        var clip = config.buttonSFX;
        sfx.PlayOneShot(clip);
    }

    void Poof()
    {
        var sceneController = manager.GetComponent<SceneManagerController>();
        if (sceneController.isLoading) return;
        if (!isSlicable) return;

        IEnumerator DisableRoutine() {
            yield return new WaitForSeconds(config.vfxTime * 2);
            SetSlicable(true);
        }

        PlayVFX();
        PlaySFX();

        onSlice.Invoke();
        SetSlicable(false);
        StartCoroutine(DisableRoutine());
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
