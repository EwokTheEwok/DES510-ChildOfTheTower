using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BrickDialogue : MonoBehaviour
{
    private BlockController2 blockController;

    private GameObject _BrickCanvas;
    private GameObject _BrickBox;
    private TMP_Text _BrickText;

    [SerializeField] private string[] _RandomLines;
    [SerializeField] private string[] _TargetedLines;
    [SerializeField] private float _TypeSpeed = 0.05f;

    [Header("Events")]
    public UnityEvent<int, GameObject> _RandomBrickDialogueEvent = new();
    public UnityEvent<int, GameObject> _BrickDialogueEvent = new();

    private bool _IsRandomTalking = false;
    private bool _IsTargetedTalking = false;
    private bool _IsRestartTalking = false;
    private bool _IsInsideTargetedCollider = false;
    private bool _NearNPC;

    [Header("Brick Box Parameters")]
    public bool _BlockActive;

    public float _DisplayDelay = 5.0f;
    //public float _NewLineDelay = 10.0f;
    public float _BetweenLineDelay;
    public float _Height = 3f;

    private RectTransform _DialogueTransform;
    Transform _PlayerTransform;

    private IEnumerator _RandomDialogueCoroutine;
    private IEnumerator _TargetedBrickCoroutine;
    private IEnumerator _RestartBrickCoroutine;


    private void Start()
    {
        GameManager.Instance._RestartEvent.AddListener(OnRestart);

        _BrickCanvas = GameManager.Instance.UI.transform.Find("Block Canvas").gameObject;
        _BrickBox = _BrickCanvas.transform.Find("BrickBox").gameObject;
        _BrickText = _BrickBox.transform.Find("BrickText").GetComponent<TMP_Text>();
        blockController = GameManager.Instance.Player.GetComponent<BlockController2>();
        _PlayerTransform = GameManager.Instance.Player.transform;

        _DialogueTransform = _BrickBox.transform as RectTransform;
        _BetweenLineDelay = Random.Range(10.0f, 17.0f);
    }

    private void Update()
    {
        _DialogueTransform.position = Camera.main.WorldToScreenPoint(_PlayerTransform.position + new Vector3(0, _Height, 0));

        DisplayBrickSprite();

       //_BetweenLineDelay = Random.Range(10.0f, 25.0f);
    }

   private void DisplayBrickSprite()
    {
        _BlockActive = blockController.IsBlockActive();
        GameObject.Find("Block Sprite").GetComponent<SpriteRenderer>().enabled = !_BlockActive;

        if (_BlockActive == true)
        {
            StopTalking();
        }
        else if (_BlockActive == false && !_IsRandomTalking && !_IsTargetedTalking && !_IsRestartTalking && !_NearNPC)
        {
            _RandomDialogueCoroutine = RandomDialogue();
            StartCoroutine(_RandomDialogueCoroutine);
        }
    }

    private IEnumerator RandomDialogue()
    {
        _IsRandomTalking = true;
        _BetweenLineDelay = Random.Range(10.0f, 25.0f);
        yield return new WaitForSeconds(_BetweenLineDelay);

        while (true)
        {
            
            _BrickBox.SetActive(true);
            _BrickText.text = "";

            int lineIndex = Random.Range(0, _RandomLines.Length);
            _RandomBrickDialogueEvent?.Invoke(lineIndex, _BrickBox);

            foreach (char c in _RandomLines[lineIndex])
            {
                _BrickText.text += c;
                yield return new WaitForSeconds(_TypeSpeed);
            }

            yield return new WaitForSeconds(Mathf.Abs(_DisplayDelay));

            _BrickBox.SetActive(false);
            _BetweenLineDelay = Random.Range(10.0f, 25.0f);
            yield return new WaitForSeconds(Mathf.Abs(_BetweenLineDelay));
        }
    }

    private IEnumerator TargetedDialogue(int MinIndex, int MaxIndex)
    {
        _IsTargetedTalking = true;

        _BrickBox.SetActive(true);
        _BrickText.text = "";

        int index = Random.Range(MinIndex, MaxIndex);

        _BrickDialogueEvent?.Invoke(index, _BrickBox);

        while (_IsInsideTargetedCollider)
        {
            _BrickBox.SetActive(true);
            _BrickText.text = "";

            foreach (char c in _TargetedLines[index])
            {
                _BrickText.text += c;
                yield return new WaitForSeconds(_TypeSpeed);
            }

            yield return new WaitForSeconds(Mathf.Abs(_DisplayDelay));

            _BrickBox.SetActive(false);
            _BetweenLineDelay = Random.Range(5.0f, 10.0f);
            yield return new WaitForSeconds(Mathf.Abs(_BetweenLineDelay));
        }

        Debug.Log("blah");
        _IsTargetedTalking = false;
    }

    private IEnumerator RestartDialogue(int index)
    {
        _IsRestartTalking = true;

        _BrickBox.SetActive(true);
        _BrickText.text = "";
        _BrickDialogueEvent?.Invoke(index, _BrickBox);

        foreach (char c in _TargetedLines[index])
        {
            _BrickText.text += c;
            yield return new WaitForSeconds(_TypeSpeed);
        }

        yield return new WaitForSeconds(Mathf.Abs(_DisplayDelay));
        _BrickBox.SetActive(false);

        _IsRestartTalking = false;
    }

    private void StopTalking()
    {
        if (_RandomDialogueCoroutine != null)
            StopCoroutine(_RandomDialogueCoroutine);
        if (_TargetedBrickCoroutine != null)
            StopCoroutine(_TargetedBrickCoroutine);
        if (_RestartBrickCoroutine != null)
            StopCoroutine(_RestartBrickCoroutine);
        _IsRandomTalking = false;
        _IsTargetedTalking = false;
        _IsRestartTalking = false;
        _BrickText.text = "";
        _BrickBox.SetActive(false);
    }
   
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("NPC"))
        {
            _NearNPC = true;
            StopTalking();
        }
        else if (collision.CompareTag("Block Dialogue"))
        {
            _IsInsideTargetedCollider = true;
            if (!_IsTargetedTalking || _IsRandomTalking) {
                StopTalking();
                if (collision.name == "Block Dialogue 0")
                {
                    _TargetedBrickCoroutine = TargetedDialogue(0, 0);
                }
                else if (collision.name == "Block Dialogue 1")
                {
                    _TargetedBrickCoroutine = TargetedDialogue(1, 3);

                }
                else if (collision.name == "Block Dialogue 2")
                {

                }
                StartCoroutine(_TargetedBrickCoroutine);
            }
        }
        /*else if (collision.CompareTag("Shadow Ocean"))
        {

        }
        */
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("NPC"))
        {
            _NearNPC = false;
        } else if (collision.CompareTag("Block Dialogue"))
        {
            _IsInsideTargetedCollider = false;
        }
    }

    private void OnRestart()
    {
        StopTalking();
        _RestartBrickCoroutine = RestartDialogue(Random.Range(3, 6));
        StartCoroutine(_RestartBrickCoroutine);
    }
}