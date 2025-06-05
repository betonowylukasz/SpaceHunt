using UnityEngine;

public class Shopkeeper : NPC
{
    public Dialogue dialogue;
    public GameObject shopUI;

    public override void Interact()
    {
        DialogueManager.Instance.StartDialogue(dialogue, OnDialogueFinished);
    }

    private void OnDialogueFinished()
    {
        //shopUI.SetActive(true);
        DialogueManager.Instance.SetIsActive(false);
        Time.timeScale = 1f;
    }
}
