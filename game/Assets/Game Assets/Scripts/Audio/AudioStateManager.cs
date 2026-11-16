using UnityEngine;

public class AudioStateManager : MonoBehaviour
{
    [Header("State Events")]
    public AK.Wwise.Event PauseEvent;
    public AK.Wwise.Event ResumeEvent;

    [Header("UI Events")]
    public AK.Wwise.Event UIClickButtonEvent;
    public AK.Wwise.Event UIScrollEvent;

    public void PauseGame()
    {
        PauseEvent.Post(gameObject);
    }

    public void ResumeGame()
    {
        ResumeEvent.Post(gameObject);
    }

    public void UIButtonEvent()
    {
        UIClickButtonEvent.Post(gameObject);
    }

    public void UIScrollingEvent()
    {
        UIScrollEvent.Post(gameObject);
    }
}
