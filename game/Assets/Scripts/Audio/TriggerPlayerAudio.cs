using UnityEngine;

public class TriggerPlayerAudio : MonoBehaviour
{
    public AK.Wwise.Event FootstepsEvent;
    public AK.Wwise.Event JumpEvent;

    public CharacterController CharControl;

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
