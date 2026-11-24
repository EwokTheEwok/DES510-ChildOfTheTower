using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private string gameSceneName;
    [SerializeField] private string endingSceneName;

    public void Play()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void Home()
    {
        SceneManager.LoadScene(endingSceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
