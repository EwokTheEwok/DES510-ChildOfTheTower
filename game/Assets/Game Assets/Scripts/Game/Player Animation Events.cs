using UnityEngine;
using UnityEngine.Events;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField] UnityEvent WakeUpEvent = new();

    public void WakeUp()
    {
        WakeUpEvent?.Invoke();
    }
}
