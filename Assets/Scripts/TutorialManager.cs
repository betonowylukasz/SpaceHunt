using UnityEngine;
using UnityEngine.Events;

public class TutorialManager : MonoBehaviour
{
    public enum TutorialStep { Move, Dodge, Look, Shoot, Weapon2, Weapon1, Complete }
    public TutorialUIController uiController;
    private TutorialStep currentStep = TutorialStep.Move;

    private PlayerController player;

    void Start()
    {
        PlayerController.Instance.OnMoveAction += HandleMove;
        PlayerController.Instance.OnDodgeAction += HandleDodge;
        PlayerController.Instance.OnLookAction += HandleLook;
        PlayerController.Instance.OnShootAction += HandleShoot;
        PlayerController.Instance.OnWeapon2Action += HandleWeapon2;
        PlayerController.Instance.OnWeapon1Action += HandleWeapon1;

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

        AdvanceStep(TutorialStep.Look);
        uiController.ShowLook();
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

        AdvanceStep(TutorialStep.Complete);
        uiController.ShowComplete();
    }

    void AdvanceStep(TutorialStep nextStep)
    {
        currentStep = nextStep;
    }
}
