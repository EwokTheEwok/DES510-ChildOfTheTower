using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject PauseMenu;
    public GameObject ControlsMenu;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenu.SetActive(true);
        }
    }

    public void ResumeButton()
    {
        PauseMenu.SetActive(false);
    }

    public void ControlsButton()
    {
        PauseMenu.SetActive(false);
        ControlsMenu.SetActive(true);

    }

    public void Quit()
    {
        Application.Quit();
    }
}
