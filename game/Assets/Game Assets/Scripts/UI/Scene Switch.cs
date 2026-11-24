using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    [SerializeField] private float SceneFadeDuration;
    private SceneFade sceneFade;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        sceneFade = GetComponentInChildren<SceneFade>();
    }

    // Update is called once per frame
    private IEnumerator Start()
    {
        yield return sceneFade.FadeInCoroutine(SceneFadeDuration);
    }

    public void LoadScene(string sceneName)
    {
        Debug.Log(sceneName);
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }
    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        Debug.Log(sceneName);
        yield return sceneFade.FadeOutCoroutine(SceneFadeDuration);
        yield return SceneManager.LoadSceneAsync(sceneName);
        Debug.Log(sceneName);
    }
}
