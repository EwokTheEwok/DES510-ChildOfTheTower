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
    [SerializeField] private string[] targetedLines;
    [SerializeField] private float TypeSpeed = 0.05f;

    private int lineIndex;

    private bool _InRange = false;
    private bool _IsTalking = false;

    public float _LineDelay = 5.0f;
    public float _EndDelay = 10.0f;
    public float height = 2f;

    private RectTransform dialogueTransform;

    private IEnumerator coroutine;
    private IEnumerator npcCoroutine;

    private void Start()
    {
        dialogueTransform = BrickBox.transform as RectTransform;
        npcCoroutine = npcDia();
    }

    private void Update()
    {

        if (!_InRange)
        {
            dialogueTransform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, height, 0));
            if (!_IsTalking)
            {
                Debug.Log("aaa");
                coroutine = StartDia();
                StartCoroutine(coroutine);
            }
        }
    }

    private IEnumerator StartDia()
    {
        
        _IsTalking = true;
        lineIndex = Random.Range(0, BrickLines.Length);

        yield return new WaitForSeconds(Mathf.Abs(_LineDelay));

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

    private IEnumerator npcDia()
    {
        _IsTalking = true;
        //lineIndex = Random.Range(0, targetedLines.Length);
        lineIndex = 0;

        BrickBox.SetActive(true);
        BrickText.text = "";

        foreach (char c in targetedLines[lineIndex])
        {
            BrickText.text += c;
            yield return new WaitForSeconds(TypeSpeed);
        }

        yield return new WaitForSeconds(Mathf.Abs(_LineDelay));
        BrickBox.SetActive(false);
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
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                StopTalking();

                StartCoroutine(npcCoroutine);
            }
            _InRange = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("NPC"))
        {
            StopCoroutine(npcCoroutine);
            _InRange = false;
        }
    }
}