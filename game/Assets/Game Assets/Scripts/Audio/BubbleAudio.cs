using UnityEngine;

public class BubbleAudio : MonoBehaviour
{
    public AK.Wwise.Event BubbleEvent;

    public void PlayBubbleEvent()
    {
        BubbleEvent.Post(gameObject);
    }
}
