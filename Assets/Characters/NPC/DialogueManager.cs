using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class Dialogue
{
    public string[] sentences;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    private Queue<string> sentences;
    public GameObject dialogueBubble;
    public TMPro.TextMeshProUGUI dialogueText;
    private System.Action onFinishCallback;
    private bool isActive = false;

    [Header("Input")]
    public InputActionAsset inputActions;

    private InputAction dialogAction;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        sentences = new Queue<string>();
    }

    private void OnEnable()
    {
        if (inputActions != null)
        {
            var uiMap = inputActions.FindActionMap("UI");
            dialogAction = uiMap?.FindAction("Dialog");

            if (dialogAction != null)
            {
                dialogAction.performed += OnDialogInput;
                dialogAction.Enable();
            }
        }
    }

    private void OnDisable()
    {
        if (dialogAction != null)
        {
            dialogAction.performed -= OnDialogInput;
            dialogAction.Disable();
        }
    }

    private void OnDialogInput(InputAction.CallbackContext context)
    {
        if (isActive)
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogue(Dialogue dialogue, System.Action onFinish = null)
    {
        Time.timeScale = 0f;
        isActive = true;
        onFinishCallback = onFinish;
        sentences.Clear();
        foreach (var sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        dialogueBubble.SetActive(true);
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        dialogueText.text = sentences.Dequeue();
    }

    void EndDialogue()
    {
        if (dialogueBubble.activeInHierarchy) StartCoroutine(DelayedFinish());
        dialogueBubble.SetActive(false);
    }

    IEnumerator DelayedFinish()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1f;
        onFinishCallback?.Invoke();
    }

    public bool GetIsActive()
    {
        return isActive;
    }

    public void SetIsActive(bool active)
    {
        isActive = active;
    }
}
