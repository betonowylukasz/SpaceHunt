using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialUIController : MonoBehaviour
{
    public GameObject moveText;
    public GameObject dodgeText;
    public GameObject lookText;
    public GameObject shootText;
    public GameObject swapWeaponText;
    public GameObject completeText;

    public void ShowMove()
    {
        HideAll();
        moveText.SetActive(true);
    }

    public void ShowDodge()
    {
        HideAll();
        dodgeText.SetActive(true);
    }

    public void ShowLook()
    {
        HideAll();
        lookText.SetActive(true);
    }

    public void ShowShoot()
    {
        HideAll();
        shootText.SetActive(true);
    }

    public void ShowWeapon()
    {
        HideAll();
        swapWeaponText.SetActive(true);
    }

    public void ShowComplete()
    {
        HideAll();
        completeText.SetActive(true);
    }

    private void HideAll()
    {
        moveText.SetActive(false);
        dodgeText.SetActive(false);
        lookText.SetActive(false);
        shootText.SetActive(false);
        swapWeaponText.SetActive(false);
        completeText.SetActive(false);
    }
}
