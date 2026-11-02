using System.Collections;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class TutorialControls : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image controlBox;
    [SerializeField] private GameObject ControlBox;
    [SerializeField] private TMP_Text ControlText1;
    [SerializeField] private TMP_Text ControlText2;
    [SerializeField] private UnityEngine.UI.Image buttons;
    [SerializeField] private UnityEngine.UI.Image buttons2;
    [SerializeField] private UnityEngine.UI.Image buttons3;
    [SerializeField] private UnityEngine.UI.Image buttons4;
    [SerializeField] private UnityEngine.UI.Image buttons5;
    [SerializeField] private UnityEngine.UI.Image buttons6;
 

    [SerializeField] private string[] UI1Text;
    [SerializeField] private string[] UI2Text;
    [SerializeField] private float TypeSpeed = 0.05f;

    private int lineIndex = 0;

    private bool _InRange = false;
    private bool _IsActive = false;
    private bool _IsTyping = false;

    public float _LineDelay = 2.0f;
    public float _EndDelay = 7.0f;
    public float height = 100000f;

    private IEnumerator coroutine;



    private void Start()
    {
        controlBox.CrossFadeAlpha(0.0f, 0.05f, false);
        ControlText1.CrossFadeAlpha(0.0f, 0.05f, false);
        ControlText2.CrossFadeAlpha(0.0f, 0.05f, false);
        buttons.CrossFadeAlpha(0.0f, 0.05f, false);
        buttons2.CrossFadeAlpha(0.0f, 0.05f, false);
        buttons3.CrossFadeAlpha(0.0f, 0.05f, false);
        buttons4.CrossFadeAlpha(0.0f, 0.05f, false);
        buttons5.CrossFadeAlpha(0.0f, 0.05f, false);
        buttons6.CrossFadeAlpha(0.0f, 0.05f, false);

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
        lineIndex = 0;
        _IsActive = true;
        ControlBox.SetActive(true);
        controlBox.CrossFadeAlpha(1.0f, 0.5f, false);
        ControlText1.CrossFadeAlpha(1.0f, 0.5f, false);
        ControlText2.CrossFadeAlpha(1.0f, 0.5f, false);
        buttons.CrossFadeAlpha(1.0f, 0.5f, false);
        buttons2.CrossFadeAlpha(1.0f, 0.5f, false);
        buttons3.CrossFadeAlpha(1.0f, 0.5f, false);
        buttons4.CrossFadeAlpha(1.0f, 0.5f, false);
        buttons5.CrossFadeAlpha(1.0f, 0.5f, false);
        buttons6.CrossFadeAlpha(1.0f, 0.5f, false);





        foreach (string line in UI1Text)
        {
            ControlText1.text = "";
        
            foreach (char c in line)
            {
                ControlText1.text += c;
                yield return new WaitForSeconds(TypeSpeed);
            }
            yield return new WaitForSeconds(_LineDelay);
        }

        foreach (string line in UI2Text)
        {
            ControlText2.text = "";

            foreach (char c in line)
            {
                ControlText2.text += c;
                yield return new WaitForSeconds(TypeSpeed);
            }
            yield return new WaitForSeconds(_LineDelay);
        }

    }

    private void StopTalking()
    {
        _IsActive = false;
        ControlText1.text = "";
        ControlText2.text = "";

        controlBox.CrossFadeAlpha(0.0f, 0.3f, false);
        ControlText1.CrossFadeAlpha(0.0f, 0.3f, false);
        ControlText2.CrossFadeAlpha(0.0f, 0.3f, false);
        buttons.CrossFadeAlpha(0.0f, 0.3f, false);
        buttons2.CrossFadeAlpha(0.0f, 0.3f, false);
        buttons3.CrossFadeAlpha(0.0f, 0.3f, false);
        buttons4.CrossFadeAlpha(0.0f, 0.3f, false);
        buttons5.CrossFadeAlpha(0.0f, 0.3f, false);
        buttons6.CrossFadeAlpha(0.0f, 0.3f, false);

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
