using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    GameManager _GameManagerInstance;
    NPCDialogueManager _NPCDialogueManagerInstance;
    Tower _Tower;

    [Header("Triggered Music")]
    [SerializeField] TriggeredAudio[] _TriggeredMusic;

    [Header("Ambience")]
    [SerializeField] AK.Wwise.Event _InsideAmbience;
    [SerializeField] AK.Wwise.Event _OutsideAmbience;

    [Header("Rain")]
    [SerializeField] AK.Wwise.Event _Rain;
    [SerializeField] AK.Wwise.Event _Clear;

    [Header("NPCs")]
    [SerializeField] AK.Wwise.Event _PeterDialogue;
    [SerializeField] AK.Wwise.Event _MargDialogue;
    [SerializeField] AK.Wwise.Event _ThomasDialogue;

    [Header("Animation")]
    [SerializeField] AK.Wwise.Event _PeterIdle;
    [SerializeField] AK.Wwise.Event _MargaretIdle;
    [SerializeField] AK.Wwise.Event _ThomasIdle;
    List<NPCAnimationAudio> _NPCAudioList;
    GameObject _Player;

    [Header("Shadow Ocean")]
    [SerializeField] AK.Wwise.Event _ShadowOcean;
    [SerializeField] AK.Wwise.Event _FadeOcean;
    [SerializeField] AK.Wwise.Event _StopOcean;
    [SerializeField] AK.Wwise.Event _FallOcean;
    [SerializeField] float _playerDistanceThreshold = 13.1f;
    bool _OceanAudioPlaying = false;

    [Header("Checkpoints")]
    [SerializeField] AK.Wwise.Event _Checkpoint;
    [SerializeField] AK.Wwise.Event _Restart;

    [Header("Block Dialogue")]
    [SerializeField] AK.Wwise.Event[] _RandomBlock;
    [SerializeField] AK.Wwise.Event[] _TargetedBlock;

    [Header("Bubble")]
    [SerializeField] AK.Wwise.Event _MissingBubble;

    [Header("Block")]
    [SerializeField] AK.Wwise.Event[] _BlockMove;
    [SerializeField] AK.Wwise.Event _BlockStart;
    [SerializeField] AK.Wwise.Event _BlockLimit;
    [SerializeField] AK.Wwise.Event _BlockUndo;

    //[Header("Doors")]
    //public AK.Wwise.Event DoorOpenEvent;

    void Start()
    {
        _GameManagerInstance = GameManager.Instance;
        _NPCDialogueManagerInstance = NPCDialogueManager.Instance;
        _Tower = _GameManagerInstance.Tower;
        _Player = GameManager.Instance.Player;
        _NPCAudioList = FindObjectsByType<NPCAnimationAudio>().ToList();

        foreach (TriggeredAudio _tm in _TriggeredMusic)
        {
            AudioTrigger at = _tm.TriggerObject.gameObject.AddComponent<AudioTrigger>();
            at.ActivateEvent.AddListener(_tm.PlayAudio);
        }

        foreach (NPCAnimationAudio _npc in _NPCAudioList)
        {
            _npc.AnimationAudioEvent.AddListener(PlayNPCIdleAudio);
        }

        _Tower.InsideEvent.AddListener(PlayAmbienceAudio);
        _NPCDialogueManagerInstance.NPCDialogueEvent.AddListener(PlayNPCDialogueAudio);
        _GameManagerInstance.GameEvent.AddListener(PlayEventAudio);
        _GameManagerInstance.UI.GetComponentInChildren<BrickDialogue>().BlockDialogueEvent.AddListener(PlayBlockDialogueAudio);
        BlockController2 _bc = _Player.GetComponentInChildren<BlockController2>();
        _bc.BubbleEvent.AddListener(PlayBubbleAudio);
        _bc.BlockEvent.AddListener(PlayBlockAudio);
        _bc.UndoEvent.AddListener(PlayBlockUndoAudio);
        _bc.LimitEvent.AddListener(PlayBlockLimitAudio);
    }

    void Update()
    {
        UpdateOceanDistance();
        UpdateNPCDistances();
    }

    void PlayAmbienceAudio(bool _inside)
    {
        if (_inside)
            _InsideAmbience.Post(gameObject);
        else
            _OutsideAmbience.Post(gameObject);
    }

    void PlayNPCDialogueAudio(GameObject _obj, CharacterNames _charName)
    {
        switch (_charName)
        {
            case CharacterNames.Peter:
                _PeterDialogue.Post(_obj);
                break;
            case CharacterNames.Margaret:
                _MargDialogue.Post(_obj);
                break;
            case CharacterNames.Thomas:
                _ThomasDialogue.Post(_obj);
                break;
            default:
                break;
        }
    }

    void PlayNPCIdleAudio(GameObject _obj, CharacterNames _charName)
    {
        switch (_charName)
        {
            case CharacterNames.Peter:
                _PeterIdle.Post(_obj);
                break;
            case CharacterNames.Margaret:
                _MargaretIdle.Post(_obj);
                break;
            case CharacterNames.Thomas:
                _ThomasIdle.Post(_obj);
                break;
            default:
                break;
        }
    }

    void PlayEventAudio(GameEventTypes _type)
    {
        switch (_type)
        {
            case GameEventTypes.Fall:
                _FallOcean.Post(_Player);
                _StopOcean.Post(gameObject);
                _OceanAudioPlaying = false;
                break;
            case GameEventTypes.Checkpoint:
                _Checkpoint.Post(_Player);
                break;
            case GameEventTypes.Restart:
                _Restart.Post(_Player);
                break;
            case GameEventTypes.RainStart:
                _Rain.Post(_Player);
                break;
            case GameEventTypes.RainStop:
                _Clear.Post(_Player);
                break;
            default:
                break;
        }
    }

    void PlayBlockDialogueAudio(GameObject _obj, BlockDialogueType _type)
    {
        switch (_type)
        {
            case BlockDialogueType.Random:
                int randomIndex = UnityEngine.Random.Range(0, _RandomBlock.Length);
                _RandomBlock[randomIndex].Post(_obj);
                break;
            case BlockDialogueType.Targeted:
                int targetIndex = UnityEngine.Random.Range(0, _RandomBlock.Length);
                _RandomBlock[targetIndex].Post(_obj);
                break;
            default:
                break;
        }
    }

    void PlayBubbleAudio()
    {
        _MissingBubble.Post(_Player);
    }

    void PlayBlockAudio(int _index, GameObject _obj)
    {
        if (_index == 0)
        {
            _BlockStart.Post(_obj);
            _BlockMove[0].Post(_obj);
        }
        else if (_index < _BlockMove.Length)
            _BlockMove[_index].Post(_obj);
    }

    void PlayBlockUndoAudio(GameObject _obj)
    {
        _BlockUndo.Post(_obj);
    }

    void PlayBlockLimitAudio(GameObject _obj)
    {
        _BlockLimit.Post(_obj);
    }

    void UpdateOceanDistance()
    {
        float distance = _GameManagerInstance.ShadowOceanDistance();
        
        if (distance < _playerDistanceThreshold && !_OceanAudioPlaying)
        {
            //Debug.Log("playing "+distance);
            _ShadowOcean.Post(gameObject);
            _OceanAudioPlaying = true;
        }
        else if (distance > _playerDistanceThreshold && _OceanAudioPlaying)
        {
            //Debug.Log("fade " + distance);
            _FadeOcean.Post(gameObject);
            _OceanAudioPlaying = false;
        }

        AkUnitySoundEngine.SetRTPCValue("RTPC_ShadowDistance", distance);
    }

    void UpdateNPCDistances()
    {
        float threshold = 1000f;
        float distance = threshold;
        foreach (NPCAnimationAudio _npc in _NPCAudioList)
        {
            distance = Vector3.Distance(_npc.transform.position, _Player.transform.position);
            if (distance < threshold)
            {
                threshold = distance;
            }
        }
        distance = threshold;
        //Debug.Log(distance);

        AkUnitySoundEngine.SetRTPCValue("RTPC_NPCDistance", distance);
    }
}

[Serializable]
public class TriggeredAudio
{
    public AK.Wwise.Event AudioEvent;
    public BoxCollider TriggerObject;

    public void PlayAudio(GameObject _obj)
    {
        AudioEvent.Post(_obj);
    }
}

public enum BlockDialogueType
{
    Random,
    Targeted
}