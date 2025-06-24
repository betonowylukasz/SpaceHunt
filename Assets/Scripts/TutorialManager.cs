using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TutorialManager : MonoBehaviour
{
    public enum TutorialStep { Move, Dodge, Look, Shoot, Reload, Weapon2, Weapon1, Fixer, Complete }
    public TutorialUIController uiController;
    private TutorialStep currentStep = TutorialStep.Move;

    private bool isCorutineStarted = false;
    private PlayerController player;

    void Start()
    {
        if(SaveManager.Instance.CurrentSaveData.isTutorialCompleted)
        {
            return;
        }

        PlayerController.Instance.OnMoveAction += HandleMove;
        PlayerController.Instance.OnDodgeAction += HandleDodge;
        PlayerController.Instance.OnLookAction += HandleLook;
        PlayerController.Instance.OnShootAction += HandleShoot;
        WeaponManager.Instance.OnReloadAction += HandleReload;
        PlayerController.Instance.OnWeapon2Action += HandleWeapon2;
        PlayerController.Instance.OnWeapon1Action += HandleWeapon1;
        Fixer.Instance.OnDialogFinishedAction += HandleFixer;


        uiController.ShowMove();
    }

    void HandleMove()
    {
        if (currentStep != TutorialStep.Move) return;

        AdvanceStep(TutorialStep.Dodge);
        uiController.ShowDodge();
    }

    void HandleDodge()
    {
        if (currentStep != TutorialStep.Dodge) return;

        uiController.ShowDodgeInfo(); // np. "Unik blokuje pociski!"

        // Odczekaj 3 sekundy, potem przejdŸ do nastêpnego kroku
        if (!isCorutineStarted)
        {
            StartCoroutine(DelayedLookStep());
            isCorutineStarted = true;
        }
    }

    private IEnumerator DelayedLookStep()
    {
        yield return new WaitForSeconds(3f);
        isCorutineStarted = false;
        AdvanceStep(TutorialStep.Look);
        uiController.ShowLook(); // np. "Rozejrzyj siê myszk¹/padem"
    }

    void HandleLook()
    {
        if (currentStep != TutorialStep.Look) return;

        AdvanceStep(TutorialStep.Shoot);
        uiController.ShowShoot();
    }

    void HandleShoot()
    {
        if (currentStep != TutorialStep.Shoot) return;

        AdvanceStep(TutorialStep.Reload);
        uiController.ShowReload();
    }

    void HandleReload()
    {
        if (currentStep != TutorialStep.Reload) return;

        AdvanceStep(TutorialStep.Weapon2);
        uiController.ShowWeapon();
    }

    void HandleWeapon2()
    {
        if (currentStep != TutorialStep.Weapon2) return;

        AdvanceStep(TutorialStep.Weapon1);
    }

    void HandleWeapon1()
    {
        if (currentStep != TutorialStep.Weapon1) return;

        AdvanceStep(TutorialStep.Fixer);
        uiController.ShowFixer();
    }

    void HandleFixer()
    {
        if (currentStep != TutorialStep.Fixer) return;

        AdvanceStep(TutorialStep.Complete);
        uiController.ShowComplete();
        SaveManager.Instance.CurrentSaveData.isTutorialCompleted = true;
        SaveManager.Instance.Save();
    }

    void AdvanceStep(TutorialStep nextStep)
    {
        currentStep = nextStep;
    }
}
