using UnityEngine;

public class Shopkeeper : NPC
{
    public Dialogue[] possibleDialogues;

    public override void Interact()
    {
        if (possibleDialogues.Length == 0) return;

        Dialogue randomDialogue = possibleDialogues[Random.Range(0, possibleDialogues.Length)];
        DialogueManager.Instance.StartDialogue(randomDialogue, OnDialogueFinished);
    }

    private void OnDialogueFinished()
    {
        DialogueManager.Instance.SetIsActive(false);
        Time.timeScale = 1f;
    }
}
