using UnityEngine;

public class BrickDialogueAudio : MonoBehaviour
{
    public AK.Wwise.Event[] RandomBrickDialogueAudio;
    public AK.Wwise.Event[] TargetedBrickDialogueAudio;

    public void PlayRandomBrickDialogue(int index, GameObject gameObject)
    {
        RandomBrickDialogueAudio[index].Post(gameObject);
    }
    public void PlayTargetedBrickDialogue(int index, GameObject gameObject)
    {
        TargetedBrickDialogueAudio[index].Post(gameObject);
    }
}
