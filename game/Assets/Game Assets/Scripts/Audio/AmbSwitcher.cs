using UnityEngine;

public class AmbSwitcher : MonoBehaviour
{
    public AK.Wwise.Event AmbSwitchInside;
    public AK.Wwise.Event AmbSwitchOutside;
    public AK.Wwise.RTPC AmbSwitchRTPC;

    public void AmbSwitch(bool inside)
    {
        //float RTPCValue = AmbSwitchRTPC.GetGlobalValue();

        if (inside)
        {
            AmbSwitchInside.Post(gameObject);
        }

        else
        {
            AmbSwitchOutside.Post(gameObject);
        }
    }
}
