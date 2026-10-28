using UnityEngine;

public class BrickAudioSystem : MonoBehaviour
{
    // Wwise Events
    public AK.Wwise.Event[] BrickPlaceEvents;
    public AK.Wwise.Event RemoveEvent;
    public AK.Wwise.Event BrickReset;
    public AK.Wwise.Event LimitEvent;

    public void PlaceBrick(int count, GameObject gameObject)
    {
        Debug.Log("sound: " + count);
        if (count == 0)
           BrickReset.Post(gameObject);

        if (count < BrickPlaceEvents.Length)
            BrickPlaceEvents[count].Post(gameObject);
        
        else
        {
            BrickPlaceEvents[7].Post(gameObject);
        }
    }

    public void RemoveBrick(GameObject gameObject)
    {
        RemoveEvent.Post(gameObject);
    }

    public void BrickLimit(GameObject gameObject)
    {
        LimitEvent.Post(gameObject);
    }
}
