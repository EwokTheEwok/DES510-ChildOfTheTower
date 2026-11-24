using UnityEngine;
using UnityEngine.Events;

public class NPCAnimationAudio : MonoBehaviour
{
    public CharacterNames CharacterName;
    [HideInInspector] public UnityEvent<GameObject, CharacterNames> AnimationAudioEvent = new();

    public void PlayAnimationAudio()
    {
        AnimationAudioEvent?.Invoke(gameObject, CharacterName);
    }
}
