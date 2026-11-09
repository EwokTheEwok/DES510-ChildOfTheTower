using UnityEngine;

public class CheckpointAudio : MonoBehaviour
{
    public AK.Wwise.Event CheckpointEvent;

    public void PostCheckpointEvent(GameObject gameObject)
    {
        CheckpointEvent.Post(gameObject);
    }
}
