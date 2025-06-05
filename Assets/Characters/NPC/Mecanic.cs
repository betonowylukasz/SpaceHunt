using UnityEngine;

public class Mecanic : NPC
{
    public Dialogue dialogue;
    public GameObject slotUI;

    public override void Interact()
    {
        DialogueManager.Instance.StartDialogue(dialogue, OnDialogueFinished);
    }

    private void OnDialogueFinished()
    {
        WeaponSelectManager.Instance.ShowSlots();
    }
}
