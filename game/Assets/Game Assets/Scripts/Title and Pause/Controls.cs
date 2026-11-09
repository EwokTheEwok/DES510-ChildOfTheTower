using UnityEngine;
using UnityEngine.EventSystems;

public class Controls : MonoBehaviour
{
    bool isPaused = false;

    public GameObject ControlsMenu;
    public GameObject resumeButton;

    EventSystem eventSystemScript;

    private void Start()
    {
        eventSystemScript = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        eventSystemScript.SetSelectedGameObject(resumeButton);
    }

    // Update is called once per frame
    void Update()
    {
   
        if (Input.GetKeyDown(KeyCode.Tab))
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
        ControlsMenu.SetActive(true);
        isPaused = true;
    }

    public void ResumeGame()
    {
        ControlsMenu.SetActive(false);
        isPaused = false;
    }

    public void Quit()
    {
        Application.Quit();
    }
}