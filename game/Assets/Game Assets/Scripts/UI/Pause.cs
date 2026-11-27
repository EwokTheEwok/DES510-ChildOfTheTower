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

    GameManager _GameInstance;

    private void Start()
    {
        _GameInstance = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        isPaused = _GameInstance.Paused;

        if (Input.GetKeyDown(KeyCode.Escape) && ControlsMenu.activeInHierarchy)
        {
            TitleScreenFirstButton();
            ControlsMenu.SetActive(false);
            PauseMenu.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            TitleScreenFirstButton();

            if (isPaused)
                ResumeGame();
            else
                PauseGame();
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
        GameManager.Instance.Paused = true;
        PauseMenu.SetActive(true);
        _pauseFilterEvent?.Invoke(gameObject);
    }

    public void Controls()
    {
        EventSystem.current.SetSelectedGameObject(controlResumeButton);
        ControlsMenu.SetActive(true);
        controlMenuActive = true;
        _uiButtonEvent?.Invoke(gameObject);
        PauseMenu.SetActive(false);
    }
    public void ResumeGame()
    {
        GameManager.Instance.Paused = false;
        PauseMenu.SetActive(false);
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
