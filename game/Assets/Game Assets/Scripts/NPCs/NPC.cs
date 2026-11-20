using UnityEngine;
using UnityEngine.Events;

public class NPC : MonoBehaviour
{
    public string[] DialogueLines;
    public string NPCName;
    public CharacterNames CharacterName;

    UnityEvent<GameObject, bool> _ActivateEvent = new();
    [HideInInspector] public UnityEvent<GameObject, CharacterNames> AnimationAudioEvent = new();


    public void PlayAnimationAudio()
    {
        AnimationAudioEvent?.Invoke(gameObject, CharacterName);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            _ActivateEvent?.Invoke(gameObject, true);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            _ActivateEvent?.Invoke(gameObject, false);
            /*
            StopCoroutine(_DialogueCoroutine);
            StopTalking();
            _InRange = false;
            */
        }
    }

    public void AddListener(UnityAction<GameObject, bool> call)
    {
        _ActivateEvent.AddListener(call);
    }
}