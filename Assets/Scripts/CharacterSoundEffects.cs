using UnityEngine;

public class CharacterSoundEffects : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip footstepsSound;

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator.GetBool("isMoving"))
        {
            audioSource.mute = false;
        }
        else audioSource.mute = true;
    }
}
