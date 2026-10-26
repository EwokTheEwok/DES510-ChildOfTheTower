using UnityEngine;

public class ShadowRTPCTest : MonoBehaviour
{
    public int CubeSelect;

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("RTPC Value updated");
        AkUnitySoundEngine.SetRTPCValue("RTPC_ShadowDistance", CubeSelect);
    }
}
