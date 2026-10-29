using UnityEngine;

public class PlayerAnimationAudio : MonoBehaviour
{
    public AK.Wwise.Event FootstepsEvent;
    public AK.Wwise.Event JumpEvent;
    public AK.Wwise.Event LandEvent;
    public AK.Wwise.Event FallEvent;

    bool isFalling = false;

    public CharacterController CharControl;

    public void PlayFootsteps()
    {
        if (CharControl.isGrounded)
        {
            FootstepsEvent.Post(gameObject);
            isFalling = false;
        }
    }

    public void PlayJump()
    {
        JumpEvent.Post(gameObject);
    }

    public void PlayLanding()
    {
        LandEvent.Post(gameObject);
        isFalling = false;
    }

    public void PlayFalling()
    {
        if (!isFalling)
        {
            FallEvent.Post(gameObject);
            isFalling = true;
        }
    }
}
