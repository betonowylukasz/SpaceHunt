using System.Collections.Generic;
using UnityEngine;

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

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        sentences = new Queue<string>();
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
        Debug.Log(sentences.Count);
        dialogueText.text = sentences.Dequeue();
    }

    void EndDialogue()
    {
        Debug.Log(sentences.Count);
        dialogueBubble.SetActive(false);
        onFinishCallback?.Invoke();
    }

    void Update()
    {
        if (isActive && Input.GetKeyDown(KeyCode.X))
        {
            DisplayNextSentence();
        }
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
