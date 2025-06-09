using UnityEngine;
using UnityEngine.SceneManagement;

public class Machine : NPC
{
    public Dialogue dialogue;

    public override void Interact()
    {
        DialogueManager.Instance.StartDialogue(dialogue, OnDialogueFinished);
    }

    private void OnDialogueFinished()
    {
        DialogueManager.Instance.SetIsActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MinigameScene");
    }
}
