using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.InputSystem;
using System.Collections;

public class WeaponSelectManager : MonoBehaviour
{
    public static WeaponSelectManager Instance;

    public InputActionAsset inputActions; // przeci¹gasz ca³y Input Actions asset

    private InputAction navigateAction;
    private InputAction selectAction;

    private int currentSelectedIndex = 0;

    [Header("Upgrade UI")]
    public GameObject weaponPanel;
    public GameObject slotPanel;
    public List<UpgradeCardUI> cardUIs;
    public List<UpgradeCardUI> slotUIs;

    private int slotIndex = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void OnEnable()
    {
        if (inputActions == null)
        {
            Debug.LogWarning("InputActions Asset nie jest przypisany!");
            return;
        }

        var uiMap = inputActions.FindActionMap("UI");
        if (uiMap == null)
        {
            Debug.LogError("Nie znaleziono ActionMap o nazwie 'UI'");
            return;
        }

        navigateAction = uiMap.FindAction("Navigate");
        selectAction = uiMap.FindAction("Select");

        if (navigateAction == null || selectAction == null)
        {
            Debug.LogError("Nie znaleziono akcji 'Navigate' lub 'Select' w mapie UI");
            return;
        }

        navigateAction.Enable();
        selectAction.Enable();

        navigateAction.performed += OnNavigate;
        selectAction.performed += OnSelect;
    }

    private void OnDisable()
    {
        if (navigateAction != null)
        {
            navigateAction.performed -= OnNavigate;
            navigateAction.Disable();
        }
        if (selectAction != null)
        {
            selectAction.performed -= OnSelect;
            selectAction.Disable();
        }
    }

    private void OnNavigate(InputAction.CallbackContext context)
    {
        if (slotPanel.activeInHierarchy)
        {
            Vector2 dir = context.ReadValue<Vector2>();
            if (dir.x < 0) // w lewo
            {
                currentSelectedIndex = (currentSelectedIndex - 1 + slotUIs.Count) % slotUIs.Count;
                HighlightCard(currentSelectedIndex, slotUIs);
            }
            else if (dir.x > 0) // w prawo
            {
                currentSelectedIndex = (currentSelectedIndex + 1) % slotUIs.Count;
                HighlightCard(currentSelectedIndex, slotUIs);
            }
        }
        else if (weaponPanel.activeInHierarchy)
        {
            Vector2 dir = context.ReadValue<Vector2>();
            if (dir.x < 0) // w lewo
            {
                currentSelectedIndex = (currentSelectedIndex - 1 + cardUIs.Count) % cardUIs.Count;
                HighlightCard(currentSelectedIndex, cardUIs);
            }
            else if (dir.x > 0) // w prawo
            {
                currentSelectedIndex = (currentSelectedIndex + 1) % cardUIs.Count;
                HighlightCard(currentSelectedIndex, cardUIs);
            }
        }
    }

    private void OnSelect(InputAction.CallbackContext context)
    {
        if (slotPanel.activeInHierarchy)
        {
            slotPanel.SetActive(false);
            slotIndex = currentSelectedIndex;
            ShowWeapons();
        }
        else if (weaponPanel.activeInHierarchy)
        {
            SelectWeapon(currentSelectedIndex);
        }
    }

    public void ShowSlots()
    {
        Time.timeScale = 0f;
        for (int i = 0; i < slotUIs.Count; i++)
        {
            slotUIs[i].nameText.text = WeaponManager.Instance.EquipedWeaponName(i);
            slotUIs[i].icon.sprite = WeaponManager.Instance.EquipedWeaponSprite(i);
        }
        slotPanel.SetActive(true);
        currentSelectedIndex = 0;
        HighlightCard(currentSelectedIndex, slotUIs);
    }

    public void ShowWeapons()
    {
        Time.timeScale = 0f;
        weaponPanel.SetActive(true);
        currentSelectedIndex = 0;
        HighlightCard(currentSelectedIndex, cardUIs);
    }

    public void SelectWeapon(int selectedWeapon)
    {
        WeaponManager.Instance.SelectWeapon(slotIndex, selectedWeapon);
        weaponPanel.SetActive(false);
        StartCoroutine(DelayedFinish());
    }

    IEnumerator DelayedFinish()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        DialogueManager.Instance.SetIsActive(false);
        Time.timeScale = 1f;
    }

    private void HighlightCard(int index, List<UpgradeCardUI> cardUIs)
    {
        for (int i = 0; i < cardUIs.Count; i++)
        {
            cardUIs[i].transform.localScale = (i == index) ? Vector3.one * 1.1f : Vector3.one;
        }
    }
}
