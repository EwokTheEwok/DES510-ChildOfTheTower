using DG.Tweening;
using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using UnityEngine;

public class WindowScript : MonoBehaviour
{
    public float _WindowSize = 1f;
    public float _WindowFade = 0.5f;
    public GameObject _Player;
    public GameObject _CCamera;
    public GameObject _Tri;

    public float _Time = 0.5f;
    public float _Size = 0f;
    public float _MaxSize = 1f;
    public Ease _Ease = Ease.InOutSine;


    private void Start()
    {
        Shader.SetGlobalFloat("_Window_Size", _WindowSize);
        Shader.SetGlobalFloat("_Window_Fade", _WindowFade);
    }

    void Update()
    {
        _Tri.transform.eulerAngles = new Vector3(0f, _CCamera.transform.eulerAngles.y + 90f, 0f);
        Shader.SetGlobalFloat("_Window_Size", _WindowSize);
        Shader.SetGlobalFloat("_Window_Fade", _WindowFade);
        Shader.SetGlobalFloat("_Size", _Size);
        Vector3 vp = Camera.main.WorldToViewportPoint(_Player.transform.position);
        Vector3 playerpos = _Player.transform.position;
        Shader.SetGlobalVector("_Player_Screen_Position", new Vector4(vp.x, vp.y, vp.z, 0f));
        Shader.SetGlobalVector("_Player_World_Position", new Vector4(playerpos.x, playerpos.y, playerpos.z, 0f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") /*&& !other.CompareTag("Floor")*/) {
            other.GetComponent<Renderer>().material.SetFloat("_Hide", 1);
            Smooth(other, _MaxSize);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") /*&& !other.CompareTag("Floor")*/) {
            other.GetComponent<Renderer>().material.SetFloat("_Hide", 0);
            Smooth(other, 0);
        }
    }

    private void Smooth(Collider other, float end)
    {
        float size = other.GetComponent<Renderer>().material.GetFloat("_Size");
        DOTween.To(
        () => size,
        lambaRotation => other.GetComponent<Renderer>().material.SetFloat("_Size", lambaRotation),
        end,
        _Time).SetEase(_Ease);
    }
} 