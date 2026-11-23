using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialControls : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image tutorialBox;
    [SerializeField] private GameObject TutorialBox;
    [SerializeField] private UnityEngine.UI.Image TutorialImage1;
    [SerializeField] private UnityEngine.UI.Image TutorialImage2;
    [SerializeField] private TMP_Text TutorialText1;
    [SerializeField] private TMP_Text TutorialText2;

 

    [SerializeField] private string[] UI1Text;
    [SerializeField] private string[] UI2Text;
    [SerializeField] private float TypeSpeed = 0.05f;

    private bool _InRange = false;
    private bool _IsActive = false;

    public float _LineDelay = 2.0f;
    public float _EndDelay = 7.0f;
    public float height = 100000f;

    private IEnumerator coroutine;



    private void Start()
    {
        tutorialBox.CrossFadeAlpha(0.0f, 0.05f, false);
        TutorialText1.CrossFadeAlpha(0.0f, 0.05f, false);
        TutorialText2.CrossFadeAlpha(0.0f, 0.05f, false);
        TutorialImage1.CrossFadeAlpha(0.0f, 0.05f, false);
        TutorialImage2.CrossFadeAlpha(0.0f, 0.05f, false);

    }

    private void Update()
    {


        if (_InRange)
        {

            if (!_IsActive)

            {
                coroutine = StartDia();
                StartCoroutine(coroutine);
            }
        }
    }

    private IEnumerator StartDia()
    {
        _IsActive = true;
        TutorialBox.SetActive(true);
        tutorialBox.CrossFadeAlpha(1.0f, 0.5f, false);
        TutorialText1.CrossFadeAlpha(1.0f, 0.5f, false);
        TutorialText2.CrossFadeAlpha(1.0f, 0.5f, false);
        TutorialImage1.CrossFadeAlpha(1.0f, 0.5f, false);
        TutorialImage2.CrossFadeAlpha(1.0f, 0.5f, false);


        foreach (string line in UI1Text)
        {
            TutorialText1.text = "";
        
            foreach (char c in line)
            {
                TutorialText1.text += c;
                yield return new WaitForSeconds(TypeSpeed);
            }
            yield return new WaitForSeconds(_LineDelay);
        }

        foreach (string line in UI2Text)
        {
            TutorialText2.text = "";

            foreach (char c in line)
            {
                TutorialText2.text += c;
                yield return new WaitForSeconds(TypeSpeed);
            }
            yield return new WaitForSeconds(_LineDelay);
        }

    }

    private void StopTalking()
    {
        _IsActive = false;
        TutorialText1.text = "";
        TutorialText2.text = "";

        tutorialBox.CrossFadeAlpha(0.0f, 0.3f, false);
        TutorialText1.CrossFadeAlpha(0.0f, 0.3f, false);
        TutorialText2.CrossFadeAlpha(0.0f, 0.3f, false);
        TutorialImage1.CrossFadeAlpha(0.0f, 0.3f, false);
        TutorialImage2.CrossFadeAlpha(0.0f, 0.3f, false);


        //ControlBox.SetActive(false);
    }
  
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player") && !_IsActive)
        {
            _InRange = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            StopCoroutine(coroutine);
            StopTalking();
            _InRange = false;

        }
    }
}
