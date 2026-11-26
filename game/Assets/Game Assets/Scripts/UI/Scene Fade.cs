using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneFade : MonoBehaviour
{
    [SerializeField] private Image BlackScreen;
    [SerializeField] private float elapsedPercentage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        BlackScreen = GetComponent<Image>();
    }

    public IEnumerator FadeInCoroutine(float duration)
    {
        Color startColor = new Color(BlackScreen.color.r, BlackScreen.color.g, BlackScreen.color.b, 1);
        Color targetColor = new Color(BlackScreen.color.r, BlackScreen.color.g, BlackScreen.color.b, 0);

        yield return FadeCoroutine(startColor, targetColor, duration);
        gameObject.SetActive(false);
    }

    public IEnumerator FadeOutCoroutine(float duration)
    {
        Color startColor = new Color(BlackScreen.color.r, BlackScreen.color.g, BlackScreen.color.b, 0);
        Color targetColor = new Color(BlackScreen.color.r, BlackScreen.color.g, BlackScreen.color.b, 1);

        gameObject.SetActive(true);
        yield return FadeCoroutine(startColor, targetColor, duration);
    }

    private IEnumerator FadeCoroutine(Color startColor, Color targetColor, float duration)
    {
        float elapsedTime = 0;
        elapsedPercentage = 0;

        while(elapsedPercentage < 1)
        {
            elapsedPercentage = elapsedTime / duration;
            BlackScreen.color = Color.Lerp(startColor, targetColor, elapsedPercentage);

            yield return null;
            elapsedTime += Time.deltaTime;
        }
    }
}
