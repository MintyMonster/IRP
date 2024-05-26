using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inworld.Interactions;
using Inworld;
using Inworld.Sample.Innequin;


public class ReceiveQuest : MonoBehaviour
{
    private InworldCharacter character3D;
    [SerializeField] private Canvas questCanvas;
    public bool inQuest { get; private set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        character3D = GetComponent<InworldCharacter>();

        Debug.Log(character3D.Name);
        Debug.Log(character3D.Event.ToString());

        
        if(character3D != null && character3D.Event != null)
        {
            character3D.Event.onGoalCompleted.AddListener((goalName, additionalInfo) => HandleInteraction(goalName, additionalInfo));
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandleInteraction(string goalName, string additionalInfo)
    {
        Debug.Log("Current goal: " + goalName + ", " + additionalInfo);

        if (additionalInfo == "ask_about_quests")
            GiveQuest();

    }

    private void GiveQuest()
    {
        Debug.Log("Canvas enabled" + "InQuest? " + inQuest);
        questCanvas.gameObject.SetActive(true);
        inQuest = true;
    }
}
