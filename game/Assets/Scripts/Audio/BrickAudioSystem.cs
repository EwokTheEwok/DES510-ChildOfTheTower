using UnityEngine;

public class BrickAudioSystem : MonoBehaviour
{
    // Wwise Events
    public AK.Wwise.Event[] BrickPlaceEvents;
    public AK.Wwise.Event RemoveEvent;
    public AK.Wwise.Event BrickReset;

    public void PlaceBrick(int count, GameObject gameobject)
    {
        if (count == 0)
           BrickReset.Post(gameObject);

        BrickPlaceEvents[count].Post(gameObject);
    }

    public void RemoveBrick()
    {
        RemoveEvent.Post(gameObject);
    }
}
