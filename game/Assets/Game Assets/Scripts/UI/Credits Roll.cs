using System.Collections;
using TMPro;
using UnityEngine;

public class CreditsRoll : MonoBehaviour
{
    public SceneFade sceneFade;

    [SerializeField] private float SceneFadeDuration;
    [SerializeField] private TMP_Text TheEnd;
    [SerializeField] private TMP_Text ThankYou;
    [SerializeField] private GameObject Credits;
    [SerializeField] private GameObject FinalScreen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(credits());
    }

    private IEnumerator credits()
    {
        yield return new WaitForSeconds(3);
        yield return sceneFade.FadeOutCoroutine(SceneFadeDuration);
        TheEnd.gameObject.SetActive(false);
        ThankYou.gameObject.SetActive(true);
        yield return sceneFade.FadeInCoroutine(SceneFadeDuration);

        yield return new WaitForSeconds(5);
        yield return sceneFade.FadeOutCoroutine(SceneFadeDuration);
        ThankYou.gameObject.SetActive(false);
        Credits.SetActive(true);
        yield return sceneFade.FadeInCoroutine(SceneFadeDuration);

        yield return new WaitForSeconds(3);
        yield return sceneFade.FadeOutCoroutine(SceneFadeDuration);
        Credits.SetActive(false);
        FinalScreen.SetActive(true);
        yield return sceneFade.FadeInCoroutine(SceneFadeDuration);

    }
}
