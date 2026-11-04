using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class NPC : MonoBehaviour
{
    public string[] DialogueLines;
    public string NPCName;

    UnityEvent<GameObject, bool> _ActivateEvent = new();


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            _ActivateEvent?.Invoke(gameObject, true);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            _ActivateEvent?.Invoke(gameObject, false);
            /*
            StopCoroutine(_DialogueCoroutine);
            StopTalking();
            _InRange = false;
            */
        }
    }

    public void AddListener(UnityAction<GameObject, bool> call)
    {
        _ActivateEvent.AddListener(call);
    }
}