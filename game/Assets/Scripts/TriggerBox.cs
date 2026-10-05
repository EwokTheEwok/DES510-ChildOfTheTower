using UnityEngine;
using UnityEngine.Events;
public class TriggerBox : MonoBehaviour
{

    [SerializeField] UnityEvent onTriggerEnter;
    [SerializeField] UnityEvent onTriggerExit;


    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter.Invoke();
    }


    private void OnTriggerExit(Collider other)
    {
        onTriggerExit.Invoke();
    }
}
