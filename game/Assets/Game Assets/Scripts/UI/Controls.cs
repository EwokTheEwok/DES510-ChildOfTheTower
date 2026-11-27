using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Controls : MonoBehaviour
{
    public Pause pause;
    
    bool isPaused = false;
    bool controlMenuActive = false;

    public GameObject ControlsMenu;
    public GameObject PauseMenu;
    public GameObject resumeButton;

    EventSystem eventSystemScript;

    [Header("Audio Events")]
    public UnityEvent<GameObject> _pauseFilterEvent;
    public UnityEvent<GameObject> _stopPauseFilterEvent;
    public UnityEvent<GameObject> _uiButtonEvent;
    public UnityEvent<GameObject> _uiScrollEvent;

    GameManager _GameInstance;

    private void Start()
    {
        eventSystemScript = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        eventSystemScript.SetSelectedGameObject(resumeButton);
        _GameInstance = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        isPaused = _GameInstance.Paused;

        if (Input.GetKeyDown(KeyCode.Tab) && !PauseMenu.activeInHierarchy)
        {
            TitleScreenFirstButton();

            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        TriggerScrollSound();
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
        PauseMenu.SetActive(false);
        ControlsMenu.SetActive(true);
        controlMenuActive = true;
        _pauseFilterEvent?.Invoke(gameObject);
    }

    public void ResumeGame()
    {
        Debug.Log("is she free");
        GameManager.Instance.Paused = false;
        Debug.Log("shes free");
        ControlsMenu.SetActive(false);
        controlMenuActive = false;
        _uiButtonEvent?.Invoke(gameObject);
        _stopPauseFilterEvent?.Invoke(gameObject);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void TriggerScrollSound()
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