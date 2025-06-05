using UnityEngine;
using UnityEngine.UI;

public abstract class NPC : MonoBehaviour
{
    public GameObject interactionPromptUI;

    protected bool isPlayerNearby = false;

    protected virtual void Update()
    {
        if (isPlayerNearby && !DialogueManager.Instance.GetIsActive() && (Input.GetKeyDown(KeyCode.X)))
        {
            HidePrompt();
            Interact();
        }
    }

    public void ShowPrompt()
    {
        if (interactionPromptUI != null)
            interactionPromptUI.SetActive(true);
    }

    public void HidePrompt()
    {
        if (interactionPromptUI != null)
            interactionPromptUI.SetActive(false);
    }

    public abstract void Interact();

    public virtual void Highlight(bool on)
    {
        // np. zmiana koloru/shadera
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            ShowPrompt();
            Highlight(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            HidePrompt();
            Highlight(false);
        }
    }
}
