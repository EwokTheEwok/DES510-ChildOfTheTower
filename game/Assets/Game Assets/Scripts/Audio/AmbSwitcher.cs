using UnityEngine;

public class AmbSwitcher : MonoBehaviour
{
    public AK.Wwise.Event AmbSwitchInside;
    public AK.Wwise.Event AmbSwitchOutside;
    public AK.Wwise.RTPC AmbSwitchRTPC;

    private void OnTriggerEnter(Collider other)
    {
        AmbSwitch();
    }

    public void AmbSwitch()
    {
        float RTPCValue = AmbSwitchRTPC.GetGlobalValue();

        if (RTPCValue == 0)
        {
            AmbSwitchInside.Post(gameObject);
        }

        else if (RTPCValue == 1)
        {
            AmbSwitchOutside.Post(gameObject);
        }
    }
}
