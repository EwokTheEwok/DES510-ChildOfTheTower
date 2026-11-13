using UnityEngine;

public class TriggerMusicLoop : MonoBehaviour
{
    public AK.Wwise.Event TriggerLevelMusicLoop;

    bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered)
        TriggerLevelMusicLoop.Post(gameObject);

        hasTriggered = true;
    }
}
