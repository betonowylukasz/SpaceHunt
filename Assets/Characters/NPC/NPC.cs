using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public abstract class NPC : MonoBehaviour
{
    public GameObject interactionPromptUI;
    public InputActionAsset inputActions; // <- przeci¹gasz Input Asset

    private InputAction dialogAction;
    protected bool isPlayerNearby = false;

    protected virtual void OnEnable()
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

    protected virtual void OnDisable()
    {
        if (dialogAction != null)
        {
            dialogAction.performed -= OnDialogInput;
            dialogAction.Disable();
        }
    }

    private void OnDialogInput(InputAction.CallbackContext context)
    {
        if (isPlayerNearby &&
            !DialogueManager.Instance.GetIsActive())
        {
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
