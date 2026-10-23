using DG.Tweening;
using UnityEngine;

public class WindowScripta : MonoBehaviour
{
    public GameObject _Player;
    public GameObject _CCamera;
    public GameObject _WindowTri;

    public float _Time = 0.5f;
    public float _MaxSize = 1f;
    public float _MaxWallWindowSize = 2f;
    public float _WindowEdgeFade = 0.5f;
    public Ease _Ease = Ease.InOutSine;

    void Update()
    {
        _WindowTri.transform.eulerAngles = new Vector3(0f, _CCamera.transform.eulerAngles.y + 90f, 0f);
        Vector3 vp = Camera.main.WorldToViewportPoint(_Player.transform.position);
        Vector3 playerpos = _Player.transform.position;

        Shader.SetGlobalFloat("_Window_Fade", _WindowEdgeFade);
        Shader.SetGlobalVector("_Player_Screen_Position", new Vector4(vp.x, vp.y, vp.z, 0f));
        Shader.SetGlobalVector("_Player_World_Position", new Vector4(playerpos.x, playerpos.y, playerpos.z, 0f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Wall"))
            Smooth(other, _MaxSize);
        else if (!other.CompareTag("Player") && other.CompareTag("Wall"))
            Smooth(other, _MaxWallWindowSize);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            Smooth(other, 0);
    }

    private void Smooth(Collider other, float end)
    {
        if (other.GetComponent<Renderer>().material.HasFloat("_Window_Size"))
        {
            float size = other.GetComponent<Renderer>().material.GetFloat("_Window_Size");
            DOTween.To(
            () => size,
            lambaRotation => other.GetComponent<Renderer>().material.SetFloat("_Window_Size", lambaRotation),
            end,
            _Time).SetEase(_Ease);
        }
    }
} 