using UnityEngine;

public class AmbSwitcher : MonoBehaviour
{
    public AK.Wwise.Event AmbSwitchInside;
    public AK.Wwise.Event AmbSwitchOutside;

    public AK.Wwise.Event RainSwitchOn;
    public AK.Wwise.Event RainSwitchOff;

    public AK.Wwise.RTPC AmbSwitchRTPC;
    public AK.Wwise.RTPC RainSwitchRTPC;

    public AK.Wwise.Event TriggerAmb2;

    bool HasTriggered = false;

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

    public void RainSwitch(bool israining)
    {
        if (israining)
        {
            RainSwitchOn.Post(gameObject);
        }

        else
        {
            RainSwitchOff.Post(gameObject);
        }
    }

    public void SwitchAmbLevel2()
    {
        if (!HasTriggered)
        {
            TriggerAmb2.Post(gameObject);
            HasTriggered = true;
        }
    }
}
