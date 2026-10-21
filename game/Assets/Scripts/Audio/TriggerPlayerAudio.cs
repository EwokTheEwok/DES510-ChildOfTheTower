using UnityEngine;

public class TriggerPlayerAudio : MonoBehaviour
{
    public AK.Wwise.Event FootstepsEvent;
    public AK.Wwise.Event JumpEvent;

    CharacterController CharControl;

    public void Start()
    {
        CharControl = GetComponent<CharacterController>();
    }
    public void PlayFootsteps()
    {
        if (CharControl.isGrounded)
        {
            FootstepsEvent.Post(gameObject);
        }
    }

    public void PlayJump()
    {
        JumpEvent.Post(gameObject);
    }
}
