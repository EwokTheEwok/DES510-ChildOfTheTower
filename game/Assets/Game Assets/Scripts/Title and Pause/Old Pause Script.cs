using UnityEngine;
using UnityEngine.EventSystems;

public class OldPauseScript : MonoBehaviour
{
    bool isPaused = false;
    bool controlsUp = false;
    bool pauseUp = false;

    public GameObject PauseMenu;
    public GameObject ControlsMenu;
    public GameObject resumeButton; //button in slot zero

    EventSystem eventSystemScript;


    private void Start()
    {
        eventSystemScript = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            //if the game is not paused and the control menu isnt up the display the paused screen
            if (!isPaused && !controlsUp)
            {

                PauseGame();

                //if the game is paused and the control menu is up the display the paused screen and turn off the control screen
            }
            else if (isPaused && controlsUp)
            {

                PauseGame();

                // resume game
            }
            else
            {

                ResumeGame();

            }

        }
        else if (Input.GetKeyDown(KeyCode.Tab))
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


    public void PauseGame()
    {
        ControlsMenu.SetActive(false);
        PauseMenu.SetActive(true);
        isPaused = true;
        pauseUp = true;
        controlsUp = false;
    }

    public void ResumeGame()
    {
        PauseMenu.SetActive(false);
        ControlsMenu.SetActive(false);
        isPaused = false;
        pauseUp = false;
        controlsUp = false;
    }

    public void ControlScreen()
    {
        PauseMenu.SetActive(false);
        ControlsMenu.SetActive(true);
        isPaused = true;
        pauseUp = false;
        controlsUp = true;
        EventSystem.current.SetSelectedGameObject(resumeButton);
        Debug.Log(EventSystem.current.currentSelectedGameObject);
    }

    public void Quit()
    {
        Application.Quit();
    }
}