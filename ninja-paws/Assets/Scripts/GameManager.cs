using UnityEngine;
using System.Collections;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public GameObject questDialoguebox;
    public TMP_Text QuestDialogueText;

    private static String playerTag = "Player";


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Quest started");
            questDialoguebox.SetActive(true);
            QuestDialogueText.text = "Hello Player";
        }

    }
}
