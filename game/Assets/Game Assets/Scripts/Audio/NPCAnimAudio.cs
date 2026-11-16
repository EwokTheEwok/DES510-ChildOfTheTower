using DG.Tweening;
using UnityEngine;

public class NPCAnimAudio : MonoBehaviour
{
    public AK.Wwise.Event MargIdleAnim;

    public AK.Wwise.RTPC NPCDistance;

    private GameObject _player;

    void Start()
    {
        _player = GameManager.Instance.Player;
    }

    public void PlayMargIdle()
    {
        MargIdleAnim.Post(gameObject);
    }

    public void CalculateNPCDistance()
    {
        if (_player == null) { return; }

        Vector2 player = (_player.transform.position);

        Vector2 NPC = (gameObject.transform.position);

        float distance = Vector2.Distance(player, NPC);

        AkUnitySoundEngine.SetRTPCValue("RTPC_NPCDistance", distance);

        //Debug.Log(distance);
    }

    private void Update()
    {
        CalculateNPCDistance();
    }
}
