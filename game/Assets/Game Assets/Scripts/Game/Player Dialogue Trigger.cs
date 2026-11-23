using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PlayerDialogueTrigger : MonoBehaviour
{
    [SerializeField] string _NPCDialogueTag = "NPC Dialogue";
    [SerializeField] string _BlockDialogueTag = "Block Dialogue";
    [HideInInspector] public UnityEvent<GameObject, bool, int> NPCDialogueEvent = new();
    [HideInInspector] public UnityEvent<GameObject, bool> BlockDialogueEvent = new();

    private void OnTriggerEnter(Collider other)
    {
        int tagIndex = (int)char.GetNumericValue(other.tag[^1]);

        if (other.tag.StartsWith(_NPCDialogueTag))
        {
            NPCDialogueEvent?.Invoke(other.gameObject, true, tagIndex);
        }
        else if (other.tag.StartsWith(_BlockDialogueTag))
        {
            BlockDialogueEvent?.Invoke(other.gameObject, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.StartsWith(_NPCDialogueTag))
        {
            NPCDialogueEvent?.Invoke(other.gameObject, false, 0);
        }
        else if (other.tag.StartsWith(_BlockDialogueTag))
        {
            BlockDialogueEvent?.Invoke(other.gameObject, false);
        }
    }
}
