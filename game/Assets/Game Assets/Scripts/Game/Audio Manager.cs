using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    GameManager _GameManagerInstance;

    [Header("Triggered Music")]
    [SerializeField] List<TriggeredAudio> _TriggeredMusic;

    [Header("Ambience")]
    [SerializeField] AK.Wwise.Event _InsideAmbience;
    [SerializeField] AK.Wwise.Event _OutsideAmbience;
    [SerializeField] Tower _TowerObject;

    [Header("Rain")]
    [SerializeField] AK.Wwise.Event _Rain;
    [SerializeField] AK.Wwise.Event _Clear;

    [Header("NPCs")]
    [SerializeField] AK.Wwise.Event _PeterDialogueAudio;
    [SerializeField] AK.Wwise.Event _MargDialogueAudio;
    [SerializeField] AK.Wwise.Event _ThomasDialogueAudio;
    [SerializeField] NPCDialogueManager _NPCDialogueManager;

    [Header("Animation")]
    [SerializeField] AK.Wwise.Event _PeterAnimationAudio;
    [SerializeField] AK.Wwise.Event _MargaretAnimationAudio;
    [SerializeField] AK.Wwise.Event _ThomasAnimationAudio;
    List<NPC> _NPCList;
    GameObject _Player;

    [Header("Shadow Ocean")]
    [SerializeField] AK.Wwise.Event _ShadowOceanAudio;
    [SerializeField] AK.Wwise.Event _FadeOceanAudio;
    [SerializeField] AK.Wwise.Event _StopOceanAudio;
    [SerializeField] AK.Wwise.Event _FallOceanAudio;
    [SerializeField] float _playerDistanceThreshold = 13.1f;
    bool _OceanAudioPlaying = false;

    [Header("Checkpoints")]
    public AK.Wwise.Event _CheckpointAudio;
    public AK.Wwise.Event _RestartAudio;

    [Header("Player")]
    public AK.Wwise.Event _JumpAudio;
    public AK.Wwise.Event _WalkAudio;

    //[Header("Doors")]
    //public AK.Wwise.Event DoorOpenEvent;

    void Start()
    {
        _GameManagerInstance = GameManager.Instance;
        _Player = GameManager.Instance.Player;
        _NPCList = GetComponents<NPC>().ToList();

        foreach (TriggeredAudio _tm in _TriggeredMusic)
        {
            _tm.TriggerObject.ActivateEvent.AddListener(_tm.PlayAudio);
        }

        foreach (NPC npc in _NPCList)
        {
            npc.AnimationAudioEvent.AddListener(PlayAnimationAudio);
        }

        _TowerObject.InsideEvent.AddListener(PlayAmbienceAudio);
        _NPCDialogueManager.NPCAudioEvent.AddListener(PlayDialogueAudio);
        
        _GameManagerInstance.FallOceanEvent.AddListener(PlayFallOceanAudio);
        _GameManagerInstance.CheckpointEvent.AddListener(PlayCheckpointAudio);
        _GameManagerInstance.RestartEvent.AddListener(PlayCheckpointAudio);
        _GameManagerInstance.RainEvent.AddListener(PlayRainAudio);

        _Player.GetComponent<PlayerMovement>().PlayerAudioEvent.AddListener(PlayPlayerAudio);

    }

    void Update()
    {
        UpdateOceanDistance();
        UpdateCharacterDistances();
    }

    void PlayAmbienceAudio(bool _inside)
    {
        if (_inside)
            _InsideAmbience.Post(gameObject);
        else
            _OutsideAmbience.Post(gameObject);
    }

    void PlayRainAudio(bool _start)
    {
        if (_start)
            _Rain.Post(gameObject);
        else
            _Clear.Post(gameObject);
    }

    void PlayDialogueAudio(GameObject _obj, CharacterNames _charName)
    {
        switch (_charName)
        {
            case CharacterNames.Peter:
                _PeterDialogueAudio.Post(_obj);
                break;
            case CharacterNames.Margaret:
                _MargDialogueAudio.Post(_obj);
                break;
            case CharacterNames.Thomas:
                _ThomasDialogueAudio.Post(_obj);
                break;
            default:
                break;
        }
    }

    void PlayAnimationAudio(GameObject _obj, CharacterNames _charName)
    {
        switch (_charName)
        {
            case CharacterNames.Peter:
                _PeterAnimationAudio.Post(_obj);
                break;
            case CharacterNames.Margaret:
                _MargaretAnimationAudio.Post(_obj);
                break;
            case CharacterNames.Thomas:
                _ThomasAnimationAudio.Post(_obj);
                break;
            default:
                break;
        }
    }

    void PlayFallOceanAudio()
    {
        _FallOceanAudio.Post(_Player);
    }

    void PlayCheckpointAudio(GameObject _obj)
    {
        _CheckpointAudio.Post(_obj);
    }

    void PlayCheckpointAudio()
    {
        _RestartAudio.Post(_Player);
    }

    void PlayPlayerAudio(GameObject _obj, PlayerEventType _type)
    {
        switch (_type)
        {
            case PlayerEventType.Walk:
                _WalkAudio.Post(_obj);
                break;
            case PlayerEventType.Jump:
                _JumpAudio.Post(_obj);
                break;
                //CONTINUE
            default:
                break;
        }
        
    }

    void UpdateOceanDistance()
    {
        float distance = _GameManagerInstance.ShadowOceanDistance();

        if (distance < _playerDistanceThreshold && !_OceanAudioPlaying)
        {
            _ShadowOceanAudio.Post(gameObject);
            _OceanAudioPlaying = true;
        }

        else if (distance > _playerDistanceThreshold)
        {
            _FadeOceanAudio.Post(gameObject);
            _OceanAudioPlaying = false;
        }

        /* do on fall in shadow
        else if (distance < 2)
        {
            StopShadows.Post(gameObject);
            _isPlaying = false;
        }
        */
    }

    void UpdateCharacterDistances()
    {
        float threshold = 1000f;
        float distance = threshold;
        foreach (NPC npc in _NPCList)
        {
            distance = Vector3.Distance(npc.transform.position, _Player.transform.position);
            if (distance < threshold)
            {
                threshold = distance;
            }
        }

        AkUnitySoundEngine.SetRTPCValue("RTPC_NPCDistance", distance);
    }
}

[Serializable]
public class TriggeredAudio
{
    public AK.Wwise.Event AudioEvent;
    public AudioTrigger TriggerObject;

    public void PlayAudio(GameObject _obj)
    {
        AudioEvent.Post(_obj);
    }
}
