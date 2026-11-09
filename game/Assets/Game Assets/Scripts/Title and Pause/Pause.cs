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

        } else if (Input.GetKeyDown(KeyCode.Tab)){

            //if the game is not paused and the pause menu isnt up the display the control screen
            if (!isPaused && !pauseUp)
            {
                
                ControlScreen();

                //if the game is paused and the pause menu is up the display the control screen and turn off the pause screen
            }
            else if (isPaused && pauseUp)
            {
                ControlScreen();

            // resume game
            }
            else
            {

                ResumeGame();

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
        pauseUp = true;
        controlsUp = false;
    }

    public void Controls()
    {
        EventSystem.current.SetSelectedGameObject(controlResumeButton);
        ControlsMenu.SetActive(true);
        PauseMenu.SetActive(false);
        ControlsMenu.SetActive(false);
        isPaused = false;
        pauseUp = false;
        controlsUp = false;
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
