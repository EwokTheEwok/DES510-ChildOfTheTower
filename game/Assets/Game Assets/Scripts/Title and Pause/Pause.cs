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
        Debug.Log("clicked");
        PauseMenu.SetActive(false);
        Debug.Log("post clicked");
    }

    /*
     *     bool isPaused = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        PauseMenu.SetActive(true);
        isPaused = true;
    }

    void ResumeGame()
    {
        PauseMenu.SetActive(false);
        isPaused = false;
    }
     */

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
