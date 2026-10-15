using UnityEngine;

public class WindowScript : MonoBehaviour
{
    public float _WindowSize = 1f;
    public float _WindowFade = 0.5f;

    //public int _PlayerScreenPosID = Shader.PropertyToID("_Player_Screen_Position");
    //public int _WindowSizeID = Shader.PropertyToID("_Window_Size");
    //public int _WindowFadeID = Shader.PropertyToID("_Window_Fade");

    private void Start()
    {
        Shader.SetGlobalFloat("_Window_Size", _WindowSize);
        Shader.SetGlobalFloat("_Window_Fade", _WindowFade);
        //Shader.SetGlobalVector(_PlayerScreenPosID, new Vector2(0.5f, 0.5f));
    }

    void Update()
    {
        Vector3 vp = Camera.main.WorldToViewportPoint(transform.position);
        Vector3 playerpos = transform.position;
        Shader.SetGlobalVector("_Player_Screen_Position", new Vector4(vp.x, vp.y, 0f, 0f));
        Shader.SetGlobalVector("_Player_World_Position", new Vector4(playerpos.x, playerpos.y, playerpos.z, 0f));
        

        Vector3 disp = Camera.main.transform.position - transform.position;
        if (Physics.Raycast(transform.position, disp, disp.magnitude))
        {
            Shader.SetGlobalInteger("_Window_Active", 0);
        }
        else
        {
            Shader.SetGlobalInteger("_Window_Active", 1);
        }
    }
}
