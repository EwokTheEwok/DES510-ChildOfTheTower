using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationAudio : MonoBehaviour
{
    [Header("Animation Events")]
    public AK.Wwise.Event FootstepsEvent;
    public AK.Wwise.Event JumpEvent;
    public AK.Wwise.Event LandEvent;
    public AK.Wwise.Event FallEvent;
    public AK.Wwise.Event PlaceEvent;
    public AK.Wwise.Event WakeEvent;

    [Header("Footstep Events")]
    [SerializeField] List<FootstepEvent> _FootstepsEvents;

    GameObject _Player;
    CharacterController _Char;
    float _WalkRayDistance = 0f;
    bool _Falling = false;
    
    private void Start()
    {
        _Player = GameManager.Instance.Player;
        _Char = _Player.GetComponent<CharacterController>();
        _WalkRayDistance = _Char.height;
    }

    private void FixedUpdate()
    {
        FootstepSwitcher();
    }

    public void PlayFootsteps()
    {
        if (_Char.isGrounded) {
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
        _Falling = false;
    }

    public void PlayFalling()
    {
        if (!_Falling)
        {
            FallEvent.Post(gameObject);
            _Falling = true;
        }
    }

    public void PlayPlace()
    {
        PlaceEvent.Post(gameObject);
    }

    public void PlayWake()
    {
        WakeEvent.Post(gameObject);
    }

    public void FootstepSwitcher()
    {
        if (Physics.Raycast(_Player.transform.position, Vector3.down, out RaycastHit hitinfo, _WalkRayDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            foreach (FootstepEvent _fe in _FootstepsEvents)
            {
                if (hitinfo.transform.gameObject.CompareTag(_fe.TagName))
                    _fe.AudioEvent.Post(gameObject);
            }
        }
    }
}

[Serializable]
public class FootstepEvent
{
    public AK.Wwise.Event AudioEvent;
    public string TagName;
}