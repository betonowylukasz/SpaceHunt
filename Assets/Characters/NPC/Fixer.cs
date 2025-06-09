using System;
using UnityEngine;

public class Fixer : NPC
{
    public static Fixer Instance { get; private set; }

    public Dialogue dialogue;
    public GameObject orderUI;

    public event Action OnDialogFinishedAction;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public override void Interact()
    {
        DialogueManager.Instance.StartDialogue(dialogue, OnDialogueFinished);
    }

    private void OnDialogueFinished()
    {
        //orderUI.SetActive(true);
        DialogueManager.Instance.SetIsActive(false);
        Time.timeScale = 1f;
        OnDialogFinishedAction?.Invoke();
    }
}
