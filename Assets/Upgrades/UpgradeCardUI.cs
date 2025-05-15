using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeCardUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    private Upgrade currentData;

    public void Setup(Upgrade data)
    {
        currentData = data;
        icon.sprite = data.icon;
        nameText.text = data.upgradeName;
        descriptionText.text = data.description;
    }

    public void OnClick()
    {
        UpgradeManager.Instance.SelectUpgrade(currentData);
    }
}
