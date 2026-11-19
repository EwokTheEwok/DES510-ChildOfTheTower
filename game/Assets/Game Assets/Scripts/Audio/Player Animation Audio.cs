using UnityEngine;

public class PlayerAnimationAudio : MonoBehaviour
{
    [Header("Animation Audio Events")]
    public AK.Wwise.Event FootstepsEvent;
    public AK.Wwise.Event JumpEvent;
    public AK.Wwise.Event LandEvent;
    public AK.Wwise.Event FallEvent;
    public AK.Wwise.Event PlaceEvent;
    public AK.Wwise.Event WakeEvent;

    [Header("Footstep Switch Events")]
    public AK.Wwise.Event StoneFootstepsState;
    public AK.Wwise.Event BlockFootstepsState;
    public AK.Wwise.Event WoodFootstepsState;
    public AK.Wwise.Event SandFootstepsState;


    bool isFalling = false;

    public CharacterController CharControl;

    private float rayDistance = 1.5f;

    private GameObject player;

    private void Start()
    {
        player = GameManager.Instance.Player;
    }

    private void FixedUpdate()
    {
        FootstepSwitcher();
    }

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

    public void PlayPlaceAnim()
    {
        PlaceEvent.Post(gameObject);
    }

    public void PlayWakeAnim()
    {
        WakeEvent.Post(gameObject);
    }

    public void FootstepSwitcher()
    {
        if (Physics.Raycast(player.transform.position, Vector3.down, out RaycastHit hitinfo, rayDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            if (hitinfo.transform.gameObject.CompareTag("Block"))
            {
                BlockFootstepsState.Post(gameObject);
                //Debug.Log("She Walkin on block");
            }

            else if (hitinfo.transform.gameObject.CompareTag("Sand"))
            {
                SandFootstepsState.Post(gameObject);
                //Debug.Log("She Walkin on sand");
            }

            else if (hitinfo.transform.gameObject.CompareTag("Stone"))
            {
                StoneFootstepsState.Post(gameObject);
                //Debug.Log("She Walkin on stone");
            }

            else if (hitinfo.transform.gameObject.CompareTag("Wood"))
            {
                WoodFootstepsState.Post(gameObject);
                //Debug.Log("She walkin on wood");
            }

            else
            {
                StoneFootstepsState.Post(gameObject);
            }
        }
    }
}
