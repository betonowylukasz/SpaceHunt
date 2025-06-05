using UnityEngine;

public class Fixer : NPC
{
    public Dialogue dialogue;
    public GameObject orderUI;

    public override void Interact()
    {
        DialogueManager.Instance.StartDialogue(dialogue, OnDialogueFinished);
    }

    private void OnDialogueFinished()
    {
        //orderUI.SetActive(true);
        DialogueManager.Instance.SetIsActive(false);
        Time.timeScale = 1f;
    }
}
