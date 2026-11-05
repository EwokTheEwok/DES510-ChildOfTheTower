using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    public AK.Wwise.Event PeterDialogueAudio;
    public AK.Wwise.Event MargDialogueAudio;
    public AK.Wwise.Event ThomasDialogueAudio;

    public void PlayPeterAudio(GameObject gameObject)
    {
        PeterDialogueAudio.Post(gameObject);
    }

    public void PlayMargAudio(GameObject gameObject)
    {
        MargDialogueAudio.Post(gameObject);
    }

    public void PlayThomasAudio(GameObject gameObject)
    {
        ThomasDialogueAudio.Post(gameObject);
    }
}
