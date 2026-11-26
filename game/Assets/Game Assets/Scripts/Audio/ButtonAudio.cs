using UnityEngine;

public class ButtonAudio : MonoBehaviour
{
    [Header("UI Events")]
    public AK.Wwise.Event UIClickButtonEvent;
    public AK.Wwise.Event UIScrollEvent;

    public void UIButtonEvent()
    {
        UIClickButtonEvent.Post(gameObject);
    }

    public void UIScrollingEvent()
    {
        UIScrollEvent.Post(gameObject);
    }
}
