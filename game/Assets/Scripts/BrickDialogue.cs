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
    private bool _IsActive = true;

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

        if (!_InRange)
        {
            dialogueTransform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, height, 0));
            if (_IsActive)

            {
                coroutine = StartDia();
                StartCoroutine(coroutine);
            }
        }
    }

    private IEnumerator StartDia()
    {
        lineIndex = Random.Range(0, BrickLines.Length);
        
        BrickBox.SetActive(true);

        BrickText.text = "";
        foreach (char c in BrickLines[lineIndex])
        {
             BrickText.text += c;
             yield return new WaitForSeconds(TypeSpeed);
        }
        Debug.Log("yes");
        yield return new WaitForSeconds(Mathf.Abs(_LineDelay));
        _IsActive = false;
        Debug.Log("i am");
        BrickBox.SetActive(false);
        yield return new WaitForSeconds(Mathf.Abs(_EndDelay - _LineDelay));
        Debug.Log("working");
    }

    private void StopTalking()
    {
        _IsActive = false;
        BrickText.text = "";
        BrickBox.SetActive(false);
    }
   
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("NPC") && !_IsActive)
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