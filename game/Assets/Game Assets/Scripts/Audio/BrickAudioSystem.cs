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
        Debug.Log("sound: " + count + " " + gameObject.transform.position);
        if (count == 0)
            BrickReset.Post(gameObject);

        if (count < BrickPlaceEvents.Length)
        {
            BrickPlaceEvents[count].Post(gameObject);
            Debug.Log("post: " + count);
        }
        else
        {
            BrickPlaceEvents[^1].Post(gameObject);
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
