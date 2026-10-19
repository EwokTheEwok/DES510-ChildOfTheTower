using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class TutorialControls : MonoBehaviour
{
    [SerializeField] private GameObject DialogueBox;
    [SerializeField] private TMP_Text DialogueText;

    [SerializeField] private string[] DialogueLines;
    [SerializeField] private float TypeSpeed = 0.05f;

    private int lineIndex = 0;

    private bool _InRange = false;
    private bool _IsActive = false;
    private bool _IsTyping = false;

    public float _LineDelay = 2.0f;
    public float _EndDelay = 7.0f;
    public float height = 100000f;


    private IEnumerator coroutine;

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
        DialogueBox.SetActive(true);

        foreach (string line in DialogueLines)
        {
            DialogueText.text = "";
            foreach (char c in line)
            {
                DialogueText.text += c;
                yield return new WaitForSeconds(TypeSpeed);
            }
            yield return new WaitForSeconds(_LineDelay);
        }
    }

    private void StopTalking()
    {
        _IsActive = false;
        DialogueText.text = "";
        DialogueBox.SetActive(false);
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
