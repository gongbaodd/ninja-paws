using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Configs/setting")]
public class GameSettings : ScriptableObject
{
    [Header("start scene")]
    public bool useMotion = false;
    public AudioClip startAmbienceMusic;
    public AudioClip startNonDiegeticMusic;

    [Header("levels scene")]
    public Material EstoniaFlag;
    public Material LatviaFlag;
    public Material LithuaniaFlag;
    public AudioClip levelsAmbienceMusic;
    public AudioClip levelsNonDiegeticMusic;

    [Header("game scene")]
    public Material EstoniaBackground;
    public Material LatviaBackground;
    public Material LithuaniaBackground;
    public AudioClip gameAmbienceMusic;
    public AudioClip gameNonDiegeticMusic;
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
    [Header("game ending")]
    public AudioClip endingAmbienceMusic;
    public AudioClip endingNonDiegeticMusic;
    public AudioClip winningSFX;
    public AudioClip losingSFX;

    [Header("all")]
    public float vfxTime = 0.6f;

}

