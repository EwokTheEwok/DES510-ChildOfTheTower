using UnityEngine;

public class CheckpointAudio : MonoBehaviour
{
    public AK.Wwise.Event CheckpointEvent;
    public AK.Wwise.Event DoorOpenEvent;

    public void PostCheckpointEvent(GameObject gameObject)
    {
        CheckpointEvent.Post(gameObject);
    }

    public void PostDoorOpenEvent(GameObject gameObject)
    {
        DoorOpenEvent.Post(gameObject);
    }
}
