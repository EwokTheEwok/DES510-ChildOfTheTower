using UnityEngine;

public class WindowScript : MonoBehaviour
{
    public float _WindowSize = 1f;
    public float _WindowFade = 0.5f;

    public GameObject _Player;
    public GameObject _Cylinder;
    public Material _WindowMaterial;
    public Material _BaseMaterial;

    private void Start()
    {
        Shader.SetGlobalFloat("_Window_Size", _WindowSize);
        Shader.SetGlobalFloat("_Window_Fade", _WindowFade);
    }

    void Update()
    {
        _Cylinder.transform.up = Camera.main.transform.forward;

        Shader.SetGlobalFloat("_Window_Size", _WindowSize);
        Shader.SetGlobalFloat("_Window_Fade", _WindowFade);
        Vector3 vp = Camera.main.WorldToViewportPoint(_Player.transform.position);
        Vector3 playerpos = _Player.transform.position;
        Shader.SetGlobalVector("_Player_Screen_Position", new Vector4(vp.x, vp.y, vp.z, 0f));
        Shader.SetGlobalVector("_Player_World_Position", new Vector4(playerpos.x, playerpos.y, playerpos.z, 0f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Floor"))
            other.gameObject.GetComponent<Renderer>().material = _WindowMaterial;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Floor"))
            other.gameObject.GetComponent<Renderer>().material = _BaseMaterial;
    }
}
