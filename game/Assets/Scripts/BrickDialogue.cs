using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class BrickDialogue : MonoBehaviour
{
    public BlockController blockController;

    [SerializeField] private GameObject BrickBox;
    [SerializeField] private TMP_Text BrickText;

    [SerializeField] private string[] BrickLines;
    [SerializeField] private string[] targetedLines;
    [SerializeField] private float TypeSpeed = 0.05f;

    private bool _IsRandomTalking = false;
    private bool _IsTargetedTalking = false;
    public bool _SpriteRendered;

    public float _DisplayDelay = 5.0f;
    public float _NewLineDelay = 10.0f;
    public float height = 3f;

    private RectTransform dialogueTransform;

    private IEnumerator _RandomDialogueCoroutine;
    private IEnumerator _TargetedBrickCoroutine;


    private void Start()
    {
        dialogueTransform = BrickBox.transform as RectTransform;
        
        _RandomDialogueCoroutine = RandomDialogue();
        StartCoroutine(_RandomDialogueCoroutine);

        blockController = GetComponent<BlockController>();
    }

    private void Update()
    {
        dialogueTransform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, height, 0));

        brickSprite();
    }

   private void brickSprite()
    {
        _SpriteRendered = blockController.IsBlockActive();

        if (_SpriteRendered == true)
        {
            GameObject.Find("Block Sprite").GetComponent<SpriteRenderer>().enabled = false;
        }
        else if (_SpriteRendered == false)
        {
            GameObject.Find("Block Sprite").GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    private IEnumerator RandomDialogue()
    {
        yield return new WaitForSeconds(Mathf.Abs(_DisplayDelay));

        while (true)
        {
            _IsRandomTalking = true;
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
            _IsRandomTalking = false;
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
            if (!_IsTargetedTalking)
            {
                StopTalking();
                _TargetedBrickCoroutine = TargetedDialogue(Random.Range(0, 3));
                StartCoroutine(_TargetedBrickCoroutine);
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("NPC"))
        {
            _RandomDialogueCoroutine = RandomDialogue();
            StartCoroutine(_RandomDialogueCoroutine);
        }
    }
}