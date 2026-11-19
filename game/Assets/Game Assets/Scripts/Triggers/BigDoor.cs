using System.Collections;
using UnityEngine;

public class BigDoor : MonoBehaviour
{

    private int _BoltsUnlocked;
    private IEnumerator _OpenDoorCoroutine;
    private float _DoorWaitTime = 2f;

    public void OpenVault()
    {
        _BoltsUnlocked++;
        switch (_BoltsUnlocked)
        {
            case 1:
                
                break;
            case 2:
                
                break;
            case 3:
                _OpenDoorCoroutine = OpenDoor();
                StartCoroutine(_OpenDoorCoroutine);
                break;
            default:
                break;
        }
    }

    private IEnumerator OpenDoor()
    {
        yield return new WaitForSeconds(_DoorWaitTime);
        //tell door to open here
        yield return null;
    }
}

