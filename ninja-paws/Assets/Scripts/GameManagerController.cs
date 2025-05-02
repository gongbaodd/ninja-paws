using UnityEngine;

public class GameManagerController : MonoBehaviour
{
    public static GameManagerController Instance { get { return instance; } }
    static GameManagerController instance;
    [SerializeField] GameSettings config;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
}
