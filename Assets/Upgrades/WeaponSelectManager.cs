using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class WeaponSelectManager : MonoBehaviour
{
    public static WeaponSelectManager Instance;

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

    private void Update()
    {
        // Nie reagujemy, jeœli panel nieaktywny
        if (slotPanel.activeInHierarchy)
        {
            UpdatePanel(slotUIs);
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton2)) // X na padzie
            {
                slotPanel.SetActive(false);
                slotIndex = currentSelectedIndex;
                ShowWeapons();
            }
        }
        else if (weaponPanel.activeInHierarchy)
        {
            UpdatePanel(cardUIs);
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton2)) // X na padzie
            {
                SelectWeapon(currentSelectedIndex);
            }
        }
    }

    public void UpdatePanel(List<UpgradeCardUI> cardUIs)
    {
        // Nawigacja w lewo
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentSelectedIndex = (currentSelectedIndex - 1 + cardUIs.Count) % cardUIs.Count;
            HighlightCard(currentSelectedIndex, cardUIs);
        }

        // Nawigacja w prawo
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentSelectedIndex = (currentSelectedIndex + 1) % cardUIs.Count;
            HighlightCard(currentSelectedIndex, cardUIs);
        }
    }

    public void ShowSlots()
    {
        Time.timeScale = 0f;
        for (int i=0;i<slotUIs.Count;i++)
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
        DialogueManager.Instance.SetIsActive(false);
        Time.timeScale = 1f;
    }

    private void HighlightCard(int index, List<UpgradeCardUI> cardUIs)
    {
        for (int i = 0; i < cardUIs.Count; i++)
        {
            if (i == index)
            {
                cardUIs[i].transform.localScale = Vector3.one * 1.1f;
            }
            else
            {
                cardUIs[i].transform.localScale = Vector3.one;
            }
        }
    }
}
