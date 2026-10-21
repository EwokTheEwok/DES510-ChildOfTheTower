using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class BrickDialogue : MonoBehaviour
{

    [SerializeField] private GameObject BrickBox;
    [SerializeField] private TMP_Text BrickText;

    [SerializeField] private string[] BrickLines;
    [SerializeField] private float TypeSpeed = 0.05f;

    private int lineIndex;

    private bool _InRange = false;
    private bool _IsTalking = false;

    public float _LineDelay = 5.0f;
    public float _EndDelay = 10.0f;
    public float height = 2f;

    private RectTransform dialogueTransform;

    private IEnumerator coroutine;

    private void Start()
    {
        dialogueTransform = BrickBox.transform as RectTransform;

    }

    private void Update()
    {

        if (_InRange)
        {
            dialogueTransform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, height, 0));
            if (!_IsTalking)
            {
                coroutine = StartDia();
                StartCoroutine(coroutine);
            }
        }
    }

    private IEnumerator StartDia()
    {
        _IsTalking = true;
        lineIndex = Random.Range(0, BrickLines.Length);

        BrickBox.SetActive(true);
        BrickText.text = "";

        foreach (char c in BrickLines[lineIndex])
        {
            BrickText.text += c;
            yield return new WaitForSeconds(TypeSpeed);
        }

        yield return new WaitForSeconds(Mathf.Abs(_LineDelay));
        BrickBox.SetActive(false);

        yield return new WaitForSeconds(Mathf.Abs(_EndDelay - _LineDelay));
        _IsTalking = false;
    }

    private void StopTalking()
    {
        _IsTalking = false;
        BrickText.text = "";
        BrickBox.SetActive(false);
    }
   
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("NPC"))
        {
            StopCoroutine(coroutine);
            StopTalking();
            _InRange = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("NPC"))
        {
            _InRange = false;

        }
    }
}