using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Configs/setting")]
public class GameSettings : ScriptableObject
{
    public bool useMotion = false;

    public GameObject buttonVFX;

    public DishConfig[] dishes;
    public float spawnHeight = -5f;
    public float spawnWidth = 10f;
    public float spawnDelay = 1f;
    public float wantedWeight = 50;
    public float allWeight = 50;
    public float timeInSeconds = 60f;
    public float showCountDownTime = 5f;

}

