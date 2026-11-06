using UnityEngine;
using UnityEngine.EventSystems;

public class Pause : MonoBehaviour
{
    bool isPaused = false;
    bool controlsUp = false;
    bool pauseUp = false;

    public GameObject PauseMenu;
    public GameObject ControlsMenu;
    public GameObject resumeButton;
    public GameObject quitButton;
    public GameObject controlsButton;

    EventSystem eventSystemScript;

    private void Start()
    {
        eventSystemScript = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        eventSystemScript.SetSelectedGameObject(resumeButton); 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            
            //if the game is not paused and the control menu isnt up the display the paused screen
            if (!isPaused && !controlsUp){

                PauseGame();

            //if the game is paused and the control menu is up the display the paused screen and turn off the control screen
            } else if (isPaused && controlsUp){

                PauseGame();

            // resume game
            } else{

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
        else if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {

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

    void PauseGame()
    {
        ControlsMenu.SetActive(false);
        PauseMenu.SetActive(true);
        isPaused = true;
        pauseUp = true;
        controlsUp = false;
    }

    public void TitleScreenFirstButton()
    {
        //This highlight the first button of the TitleScreen, to make it possible to use the keyboard and gamepad
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(resumeButton);
    }

    void ResumeGame()
    {
        PauseMenu.SetActive(false);
        ControlsMenu.SetActive(false);
        isPaused = false;
        pauseUp = false;
        controlsUp = false;
    }

    void ControlScreen()
    {
        PauseMenu.SetActive(false);
        ControlsMenu.SetActive(true);
        isPaused = true;
        pauseUp = false;
        controlsUp = true;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
