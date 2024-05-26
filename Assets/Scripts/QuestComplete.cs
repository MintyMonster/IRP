using Inworld;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class QuestComplete : MonoBehaviour
{

    private InworldCharacter character3D;
    [SerializeField] private Canvas questCanvas;
    [SerializeField] private Canvas completeCanvas;
    [SerializeField] private GameObject questText;

    // Start is called before the first frame update
    void Start()
    {
        character3D = GetComponent<InworldCharacter>();

        Debug.Log(character3D.Name);
        Debug.Log(character3D.Event.ToString());


        if (character3D != null && character3D.Event != null)
        {
            character3D.Event.onGoalCompleted.AddListener((goalName, additionalInfo) => HandleInteraction(goalName, additionalInfo));
        }

    }

    private void HandleInteraction(string goalName, string additionalInfo)
    {
        Debug.Log("Current goal: " + goalName + ", " + additionalInfo);

        if (additionalInfo == "listen_to_old_jack_stories")
            CompleteQuest();
    }

    private void CompleteQuest()
    {
        questCanvas.gameObject.SetActive(false);
        StartCoroutine(ShowCompleteScreen());
    }

    private IEnumerator ShowCompleteScreen()
    {
        completeCanvas.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        completeCanvas.gameObject.SetActive(false);
    }
}
