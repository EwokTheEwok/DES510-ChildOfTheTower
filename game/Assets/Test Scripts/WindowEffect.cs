using UnityEngine;

public class WindowEffect : MonoBehaviour
{
    public float m_Size = 0;
    //Renderer m_Renderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //m_Renderer = GetComponent<Renderer>();
        //m_Renderer.material.shader = Shader.Find("WindowShader");
        
    }

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalFloat("_WindowSize", m_Size);
        //m_Renderer.sharedMaterial.SetFloat("_WindowSize", m_Size);
    }
}
