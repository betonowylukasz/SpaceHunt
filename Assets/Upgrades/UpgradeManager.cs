using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Linq;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    private int currentSelectedIndex = 0;

    [Header("Upgrade UI")]
    public GameObject upgradePanel;
    public List<UpgradeCardUI> cardUIs;

    [Header("Available Upgrades")]
    public List<Upgrade> allUpgrades;

    private UpgradeAction inputActions;
    private Vector2 navigationInput;
    private bool inputLocked = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        inputActions = new UpgradeAction();

        inputActions.UI.Navigate.performed += ctx => OnNavigate(ctx.ReadValue<Vector2>());
        inputActions.UI.Select.performed += ctx => OnSelect();
    }

    private void OnEnable()
    {
        inputActions.UI.Enable();
    }

    private void OnDisable()
    {
        inputActions.UI.Disable();
    }

    private void OnNavigate(Vector2 direction)
    {
        if (!upgradePanel.activeInHierarchy || inputLocked)
            return;

        if (direction.x < -0.5f)
        {
            inputLocked = true;
            currentSelectedIndex = (currentSelectedIndex - 1 + cardUIs.Count) % cardUIs.Count;
            HighlightCard(currentSelectedIndex);
            Invoke(nameof(UnlockInput), 0.2f); // debounce
        }
        else if (direction.x > 0.5f)
        {
            inputLocked = true;
            currentSelectedIndex = (currentSelectedIndex + 1) % cardUIs.Count;
            HighlightCard(currentSelectedIndex);
            Invoke(nameof(UnlockInput), 0.2f); // debounce
        }
    }

    private void OnSelect()
    {
        if (!upgradePanel.activeInHierarchy)
            return;

        cardUIs[currentSelectedIndex].OnClick();
    }

    private void UnlockInput()
    {
        inputLocked = false;
    }

    public void ShowUpgrades()
    {
        Time.timeScale = 0f;
        upgradePanel.SetActive(true);
        List<Upgrade> selected = allUpgrades.OrderBy(x => Random.value).Take(3).ToList();

        for (int i = 0; i < cardUIs.Count; i++)
        {
            cardUIs[i].Setup(selected[i]);

            var button = cardUIs[i].GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(cardUIs[i].OnClick);
        }

        currentSelectedIndex = 0;
        HighlightCard(currentSelectedIndex);
    }

    public void SelectUpgrade(Upgrade data)
    {
        PlayerUpgrades.Instance.ApplyUpgrade(data);
        upgradePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void HighlightCard(int index)
    {
        for (int i = 0; i < cardUIs.Count; i++)
        {
            cardUIs[i].transform.localScale = (i == index) ? Vector3.one * 1.1f : Vector3.one;
        }
    }
}
