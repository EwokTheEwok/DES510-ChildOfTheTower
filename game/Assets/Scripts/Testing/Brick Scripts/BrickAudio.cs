using UnityEngine;

public class BrickAudio : MonoBehaviour
{

    public AK.Wwise.Event[] _brickEvents;

    public AK.Wwise.Event _brickReverseEvent;

    public void PostBrickEvent()
    {
        //if (_brickBehaviorTest == null || _brickEvents.Length == 0) return;

        int i = 5;

        _brickEvents[i].Post(gameObject);
    }

    public void BrickReverse()
    {
        _brickReverseEvent.Post(gameObject);
    }
}
