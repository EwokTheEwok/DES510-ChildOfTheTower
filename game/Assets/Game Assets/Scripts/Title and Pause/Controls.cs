using UnityEngine;

public class Controls : MonoBehaviour
{
    public GameObject ControlsMenu;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ControlsMenu.SetActive(true);
        }
    }

    public void ResumeButton()
    {
        ControlsMenu.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}