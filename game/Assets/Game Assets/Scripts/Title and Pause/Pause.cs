using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Pause : MonoBehaviour
{
    bool isPaused = false;
    bool controlMenuActive = false;

    public GameObject PauseMenu;
    public GameObject ControlsMenu;
    public GameObject resumeButton;
    public GameObject controlResumeButton;

    [Header("Audio Events")]
    public UnityEvent<GameObject> _pauseFilterEvent;
    public UnityEvent<GameObject> _stopPauseFilterEvent;
    public UnityEvent<GameObject> _uiButtonEvent;
    public UnityEvent<GameObject> _uiScrollEvent;

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

            if (!isPaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }


        } 

        TriggerScrollSound();
        TriggerControlMenuScrollSound();
        
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
        _pauseFilterEvent?.Invoke(gameObject);
    }

    public void Controls()
    {
        EventSystem.current.SetSelectedGameObject(controlResumeButton);
        ControlsMenu.SetActive(true);
        controlMenuActive = true;
        _uiButtonEvent?.Invoke(gameObject);
        PauseMenu.SetActive(false);
        isPaused = false;
    }
    public void ResumeGame()
    {
        PauseMenu.SetActive(false);
        isPaused = false;
        _uiButtonEvent?.Invoke(gameObject);
        _stopPauseFilterEvent?.Invoke(gameObject);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void TriggerScrollSound()
    {
        if (isPaused && Input.GetKeyDown(KeyCode.UpArrow))
        {
            _uiScrollEvent?.Invoke(gameObject);
        }

        else if (isPaused && Input.GetKeyDown(KeyCode.DownArrow))
        {
            _uiScrollEvent?.Invoke(gameObject);
        }
    }

    public void TriggerControlMenuScrollSound()
    {
        if (controlMenuActive && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _uiScrollEvent?.Invoke(gameObject);
        }

        else if (controlMenuActive && Input.GetKeyDown(KeyCode.RightArrow))
        {
            _uiScrollEvent?.Invoke(gameObject);
        }
    }
}
