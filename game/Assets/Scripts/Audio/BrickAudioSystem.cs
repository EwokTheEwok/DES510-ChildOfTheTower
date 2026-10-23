using UnityEngine;

public class BrickAudioSystem : MonoBehaviour
{
    // Wwise Events
    public AK.Wwise.Event[] BrickPlaceEvents;
    public AK.Wwise.Event RemoveEvent;
    public AK.Wwise.Event BrickReset;

    public void PlaceBrick(int count, GameObject gameobject)
    {
        Debug.Log("sound: " + count);
        if (count == 0)
           BrickReset.Post(gameObject);

        if (count < BrickPlaceEvents.Length)
            BrickPlaceEvents[count].Post(gameObject);
    }

    public void RemoveBrick(GameObject gameobject)
    {
        RemoveEvent.Post(gameObject);
    }
}
