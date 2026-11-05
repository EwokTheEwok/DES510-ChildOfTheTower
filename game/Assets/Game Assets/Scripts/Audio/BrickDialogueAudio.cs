using UnityEngine;

public class BrickDialogueAudio : MonoBehaviour
{
    public AK.Wwise.Event RandomBrickDialogueAudio;
    public AK.Wwise.Event[] TargetedBrickDialogueAudio;

    public void PlayRandomBrickDialogue(GameObject gameObject)
    {
        RandomBrickDialogueAudio.Post(gameObject);
    }
    public void PlayTargetedBrickDialogue(int index, GameObject gameObject)
    {
        TargetedBrickDialogueAudio[index].Post(gameObject);
    }
}
