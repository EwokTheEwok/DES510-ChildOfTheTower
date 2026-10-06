using UnityEngine;

public class BrickAudio : MonoBehaviour
{
    public BrickBehaviorTest _brickBehaviorTest;

    public AK.Wwise.Event[] _brickEvents;

    public void PostBrickEvent()
    {
        //if (_brickBehaviorTest == null || _brickEvents.Length == 0) return;

        int i = _brickBehaviorTest._spawnCount;

        _brickEvents[i].Post(gameObject);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
