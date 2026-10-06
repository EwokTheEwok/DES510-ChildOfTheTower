using UnityEngine;

public class TriggerMusicLoop : MonoBehaviour
{
    public AK.Wwise.Event TriggerLevelMusicLoop;

    private void OnTriggerEnter(Collider other)
    {
        TriggerLevelMusicLoop.Post(gameObject);
    }
}
