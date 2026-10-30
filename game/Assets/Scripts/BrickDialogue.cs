using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class BrickDialogue : MonoBehaviour
{
    public BlockController2 blockController;

    [SerializeField] private GameObject BrickBox;
    [SerializeField] private TMP_Text BrickText;

    [SerializeField] private string[] BrickLines;
    [SerializeField] private string[] targetedLines;
    [SerializeField] private float TypeSpeed = 0.05f;

    private bool _IsRandomTalking = false;
    private bool _IsTargetedTalking = false;
    public bool _BlockActive;

    public float _DisplayDelay = 5.0f;
    public float _NewLineDelay = 10.0f;
    public float _BetweenLineDelay;
    public float height = 3f;

    private RectTransform dialogueTransform;

    private IEnumerator _RandomDialogueCoroutine;
    private IEnumerator _TargetedBrickCoroutine;


    private void Start()
    {
        dialogueTransform = BrickBox.transform as RectTransform;

        blockController = GetComponent<BlockController2>();

        _BetweenLineDelay = Random.Range(10.0f, 17.0f);
    }

    private void Update()
    {
        dialogueTransform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, height, 0));

        brickSprite();

        _BetweenLineDelay = Random.Range(10.0f, 25.0f);
    }

   private void brickSprite()
    {
        _BlockActive = blockController.IsBlockActive();
        GameObject.Find("Block Sprite").GetComponent<SpriteRenderer>().enabled = !_BlockActive;

        if (_BlockActive == true)
        {
            StopTalking();
        }
        else if (_BlockActive == false && !_IsRandomTalking)
        {
            _RandomDialogueCoroutine = RandomDialogue();
            StartCoroutine(_RandomDialogueCoroutine);
        }
    }

    private IEnumerator RandomDialogue()
    {
        _IsRandomTalking = true;

        yield return new WaitForSeconds(_BetweenLineDelay);

        while (true)
        {
            
            BrickBox.SetActive(true);
            BrickText.text = "";

            int lineIndex = Random.Range(0, BrickLines.Length);
            foreach (char c in BrickLines[lineIndex])
            {
                BrickText.text += c;
                yield return new WaitForSeconds(TypeSpeed);
            }

            yield return new WaitForSeconds(Mathf.Abs(_DisplayDelay));

            BrickBox.SetActive(false);
            yield return new WaitForSeconds(Mathf.Abs(_NewLineDelay));
        }
    }

    private IEnumerator TargetedDialogue(int index)
    {
        _IsTargetedTalking = true;

        BrickBox.SetActive(true);
        BrickText.text = "";

        foreach (char c in targetedLines[index])
        {
            BrickText.text += c;
            yield return new WaitForSeconds(TypeSpeed);
        }

        yield return new WaitForSeconds(Mathf.Abs(_DisplayDelay));
        BrickBox.SetActive(false);
        _IsTargetedTalking = false;
    }

    private void StopTalking()
    {
        if (_RandomDialogueCoroutine != null)
            StopCoroutine(_RandomDialogueCoroutine);
        if (_TargetedBrickCoroutine != null)
            StopCoroutine(_TargetedBrickCoroutine);
        _IsRandomTalking = false;
        BrickText.text = "";
        BrickBox.SetActive(false);
    }
   
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("NPC"))
        {
            StopTalking();
        }
        else if (collision.CompareTag("Block Dialogue 0"))
        {
            StopTalking();
            _TargetedBrickCoroutine = TargetedDialogue(0);
            StartCoroutine(_TargetedBrickCoroutine);
        }
        else if (collision.CompareTag("Block Dialogue 1"))
        {
            StopTalking();
            _TargetedBrickCoroutine = TargetedDialogue(Random.Range(1, 3));
            StartCoroutine(_TargetedBrickCoroutine);
        }
        else if (collision.CompareTag("Shadow Ocean"))
        {
            StopTalking();
            _TargetedBrickCoroutine = TargetedDialogue(Random.Range(3, 6));
            StartCoroutine(_TargetedBrickCoroutine);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
            StopTalking();
            _RandomDialogueCoroutine = RandomDialogue();
            StartCoroutine(_RandomDialogueCoroutine);
    }
}