using UnityEngine;

public class pauseUI : MonoBehaviour
{
    [SerializeField] private GameObject PauseUIContainer;
    [SerializeField] private GameObject PauseContainer;
    [SerializeField] private GameObject ControlsContainer;

    private void Update()
    {
        if (PauseContainer.activeSelf == true || ControlsContainer.activeSelf == true)
        {
            PauseUIContainer.SetActive(false);

        }else if (PauseContainer.activeSelf == false || ControlsContainer.activeSelf == false)
        {
            PauseUIContainer.SetActive(true);
        }
    }

}
