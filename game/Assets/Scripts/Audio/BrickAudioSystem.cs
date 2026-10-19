using UnityEngine;

public class BrickAudioSystem : MonoBehaviour
{
    // Wwise Events
    public AK.Wwise.Event[] BrickPlaceEvents;
    public AK.Wwise.Event RemoveEvent;
    public AK.Wwise.Event BrickReset;

    // Block Controller Reference
    public BlockController BlockControllerScript;

    public void PlaceBrick()
    {
        int i = BlockControllerScript._BlockList.Count;

        GameObject gameobject = BlockControllerScript._BlockPrefab;

        if (i == 0)
           BrickReset.Post(gameObject);

        BrickPlaceEvents[i].Post(gameObject);
        Debug.Log(i);
    }

    public void RemoveBrick()
    {
        GameObject gameobject = BlockControllerScript._BlockPrefab;

        RemoveEvent.Post(gameObject);
    }
}
