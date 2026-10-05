using UnityEngine;

public class BrickSpawnIntro : MonoBehaviour
{
    public GameObject _introBrick;

    public Transform _introBrickSpawn;

    public void DropIntroBrick()
    {
        Instantiate(_introBrick, _introBrickSpawn);
        Debug.Log("Brick Spawned");
    }
}
