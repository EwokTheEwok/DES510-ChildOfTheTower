using UnityEngine;

public class CheckpointAudio : MonoBehaviour
{
    public AK.Wwise.Event CheckpointEvent;
    public AK.Wwise.Event DoorOpenEvent;
    public AK.Wwise.Event FallInShadowsEvent;
    public AK.Wwise.Event RespawnEvent;

    public void PostCheckpointEvent(GameObject gameObject)
    {
        CheckpointEvent.Post(gameObject);
    }

    public void PostDoorOpenEvent(GameObject gameObject)
    {
        DoorOpenEvent.Post(gameObject);
    }

    public void FallInShadows()
    {
        FallInShadowsEvent.Post(gameObject);
    }

    public void Respawn()
    {
        RespawnEvent.Post(gameObject);
    }
}
