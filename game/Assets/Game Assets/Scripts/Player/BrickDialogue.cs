using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class BrickDialogue : MonoBehaviour
{
    [SerializeField] UnityEngine.TextAsset _DialogueTextFile;
    List<string> _RandomDialogue = new();
    List<string> _TargetedDialogue = new();

    private BlockController2 blockController;

    [SerializeField] private GameObject _BlockCanvas;
    [SerializeField] private GameObject _BrickBox;
    private TMP_Text _BrickText;

    //[SerializeField] private string[] _RandomLines;
    //[SerializeField] private string[] _TargetedLines;
    [SerializeField] private float _TypeSpeed = 0.05f;

    [HideInInspector] public UnityEvent<GameObject, BlockDialogueType> BlockDialogueEvent = new();

    private bool _IsRandomTalking = false;
    private bool _IsTargetedTalking = false;
    private bool _IsRestartTalking = false;
    private bool _IsInsideTargetedCollider = false;
    private bool _NearNPC;

    [Header("Brick Box Parameters")]
    public bool _BlockActive;

    public float _DisplayDelay = 5.0f;
    public float _BetweenLineDelay;
    public float _Height = 3f;

    private RectTransform _DialogueTransform;
    Transform _PlayerTransform;

    private IEnumerator _RandomDialogueCoroutine;
    private IEnumerator _TargetedBrickCoroutine;
    private IEnumerator _RestartBrickCoroutine;


    private void Start()
    {
        GameManager.Instance.Player.GetComponentInChildren<PlayerDialogueTrigger>().BlockDialogueEvent.AddListener(TargetedActivate);
        GameManager.Instance.Player.GetComponentInChildren<PlayerDialogueTrigger>().NPCDialogueEvent.AddListener(NPCActivated);
        GameManager.Instance.GameEvent.AddListener(OnRestart);

        _BrickText = _BrickBox.transform.Find("BrickText").GetComponent<TMP_Text>();
        blockController = GameManager.Instance.Player.GetComponent<BlockController2>();
        _PlayerTransform = GameManager.Instance.Player.transform;

        _DialogueTransform = _BrickBox.transform as RectTransform;
        _BetweenLineDelay = Random.Range(10.0f, 17.0f);

        LoadDialogue();
    }

    private void Update()
    {
        _DialogueTransform.position = Camera.main.WorldToScreenPoint(_PlayerTransform.position + new Vector3(0, _Height, 0));

        DisplayBrickSprite();
    }

    void LoadDialogue()
    {
        string path = "Assets/Game Assets/UI/Text Assets/Block Dialogue.txt";
        if (string.IsNullOrEmpty(path))
            Debug.Log("Could not load file path");
        StreamReader reader = new(path);
        List<string> currentDialogue = _RandomDialogue;
        int currentIndex = -1;
        string currentType = "";
        string line = reader.ReadLine();

        while (line != null)
        {
            //Debug.Log(line);

            if (string.Compare(line, "Random", true) == 0)
            {
                if (string.Compare(line, currentType, true) != 0)
                    currentIndex = -1;
                currentType = "Random";
                currentDialogue = _RandomDialogue;
                currentIndex++;
            }
            else if (string.Compare(line, "Targeted", true) == 0)
            {
                if (string.Compare(line, currentType, true) != 0)
                    currentIndex = -1;
                currentType = "Targeted";
                currentDialogue = _TargetedDialogue;
                currentIndex++;
            }
            else
            {
                currentDialogue.Add(line);
            }
            line = reader.ReadLine();
        }
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

            int lineIndex = Random.Range(0, _RandomDialogue.Count);
            BlockDialogueEvent?.Invoke(_BrickBox, BlockDialogueType.Random);

            foreach (char c in _RandomDialogue[lineIndex])
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

        BlockDialogueEvent?.Invoke(_BrickBox, BlockDialogueType.Targeted);

        while (_IsInsideTargetedCollider)
        {
            _BrickBox.SetActive(true);
            _BrickText.text = "";

            foreach (char c in _TargetedDialogue[index])
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
        BlockDialogueEvent?.Invoke(_BrickBox, BlockDialogueType.Targeted);

        foreach (char c in _TargetedDialogue[index])
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

    void TargetedActivate(GameObject _obj, bool _enter, int _index)
    {
        if (_enter)
        {
            _IsInsideTargetedCollider = true;
            if (!_IsTargetedTalking || _IsRandomTalking)
            {
                StopTalking();
                switch (_index)
                {
                    case 0:
                        _TargetedBrickCoroutine = TargetedDialogue(0, 0);
                        break;
                    case 1:
                        _TargetedBrickCoroutine = TargetedDialogue(1, 3);
                        break;
                    case 2:

                        break;
                    default:
                        break;
                }
                StartCoroutine(_TargetedBrickCoroutine);
            }
        }
        else
        {
            _IsInsideTargetedCollider = false;
        }
    }

    void NPCActivated(GameObject _obj, bool _enter, int _index)
    {
        _NearNPC = _enter;
        if (_enter)
            StopTalking();
    }

    /*
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("NPC"))
        {
            _NearNPC = true;
            StopTalking();
        }
        else if (collision.tag.StartsWith("Block Dialogue"))
        {
            _IsInsideTargetedCollider = true;
            if (!_IsTargetedTalking || _IsRandomTalking) {
                StopTalking();
                if (collision.CompareTag("Block Dialogue 0"))
                {
                    _TargetedBrickCoroutine = TargetedDialogue(0, 0);
                }
                else if (collision.CompareTag("Block Dialogue 1"))
                {
                    _TargetedBrickCoroutine = TargetedDialogue(1, 3);

                }
                else if (collision.CompareTag("Block Dialogue 2"))
                {

                }
                StartCoroutine(_TargetedBrickCoroutine);
            }
        }
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
    */

    private void OnRestart(GameEventTypes _type)
    {
        if (_type == GameEventTypes.Restart)
        {
            StopTalking();
            _RestartBrickCoroutine = RestartDialogue(Random.Range(3, 6));
            StartCoroutine(_RestartBrickCoroutine);
        }
    }
}