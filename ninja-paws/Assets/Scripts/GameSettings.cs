using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Configs/setting")]
public class GameSettings : ScriptableObject
{
    [Header("start scene")]
    public bool useMotion = false;

    [Header("levels scene")]
    public Material EstoniaFlag;
    public Material LatviaFlag;
    public Material LithuaniaFlag;

    [Header("game scene")]
    public GameObject buttonVFX;
    public AudioClip buttonSFX;
    public AudioClip spawnSFX;
    public GameObject wantedVFX;
    public AudioClip wantedSFX;
    public GameObject unWantedVFX;
    public AudioClip unWantedSFX;
    public AudioClip allCollectedSFX;
    public AudioClip redoSFX;
    public GameObject redoVFX;
    public AudioClip timesUpSFX;
    public DishConfig[] dishes;
    public float spawnHeight = -5f;
    public float spawnWidth = 10f;
    public float spawnDelay = 1f;
    public float wantedWeight = 50;
    public float allWeight = 50;
    public float timeInSeconds = 60f;
    public float showCountDownTime = 5f;
    public int ingredientsPoolSize = 50;
    [Header("all")]
    public float vfxTime = 0.6f;

}

