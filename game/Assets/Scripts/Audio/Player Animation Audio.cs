using UnityEngine;

public class PlayerAnimationAudio : MonoBehaviour
{
    public AK.Wwise.Event FootstepsEvent;
    public AK.Wwise.Event JumpEvent;
    public AK.Wwise.Event LandEvent;

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

    public void PlayLanding()
    {
        LandEvent.Post(gameObject);
    }
}
