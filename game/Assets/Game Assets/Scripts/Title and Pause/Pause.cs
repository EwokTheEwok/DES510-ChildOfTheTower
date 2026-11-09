using UnityEngine;
using UnityEngine.EventSystems;

public class Pause : MonoBehaviour
{
    bool isPaused = false;

    public GameObject PauseMenu;
    public GameObject ControlsMenu;
    public GameObject resumeButton;
    public GameObject controlResumeButton;

    private void OnEnable()
    {
        TitleScreenFirstButton();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && ControlsMenu.activeSelf == true)
        {
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && ControlsMenu.activeSelf == false)
        {
            TitleScreenFirstButton();

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

    public void TitleScreenFirstButton()
    {
        //This highlight the first button of the TitleScreen, to make it possible to use the keyboard and gamepad
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(resumeButton);
    }

    public void PauseGame()
    {
        PauseMenu.SetActive(true);
        isPaused = true;
    }

    public void Controls()
    {
        EventSystem.current.SetSelectedGameObject(controlResumeButton);
        ControlsMenu.SetActive(true);
        PauseMenu.SetActive(false);
        isPaused = false;
    }
    public void ResumeGame()
    {
        PauseMenu.SetActive(false);
        isPaused = false;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
