using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public enum CharacterNames
{
    Peter,
    Margaret,
    Thomas
}

public class NPCDialogueManager : MonoBehaviour
{
    [SerializeField] UnityEngine.TextAsset _DialogueTextFile;
    List<List<string>> _PeterDialogue = new();
    List<List<string>> _MargaretDialogue = new();
    List<List<string>> _ThomasDialogue = new();

    public static NPCDialogueManager Instance { get => _Instance; }
    private static NPCDialogueManager _Instance;

    [Header("NPC")]
    //[SerializeField] string _NPCTag = "NPC";
    //private List<GameObject> _NPCList = new();
    bool _NPCActive;
    int _DialogueIndex;
    GameObject _CurrentNPC;

    [SerializeField] private GameObject _Canvas;



    [Header("NPC Local Canvas")]
    [SerializeField] private GameObject _DialogueBox;
    [SerializeField] private GameObject _NameBox;
    [SerializeField] private TMP_Text _NameText;
    private RectTransform dialogueTransform;
    public Image PeterLocal;
    public Image MargaretLocal;
    public Image ThomasLocal;


    [Header("NPC Screen Canvas")]
    [SerializeField] private GameObject _ScreenDialogueBox;
    [SerializeField] private GameObject _ScreenNameBox;
    [SerializeField] private TMP_Text _ScreenDialogueText;
    [SerializeField] private TMP_Text _ScreenNameText;
    public Image PeterScreen;
    public Image MargaretScreen;
    public Image ThomasScreen;

    [Header("NPC Vars")]
    public float _LineDelay = 2f;
    public float _EndDelay = 7f;
    public float height = 3.5f;
    public float TypeSpeed = 0.05f;
    private IEnumerator _DialogueCoroutine;

    [Header("NPC Think Bubble")]
    [SerializeField] private Animator _Animator;

    [Header("Events")]
    [HideInInspector] public UnityEvent<GameObject, CharacterNames> NPCDialogueEvent = new();

    void Awake()
    {
        if (Instance == null)
        {
            _Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueTransform = _DialogueBox.transform as RectTransform;
        GameManager.Instance.Player.GetComponentInChildren<PlayerDialogueTrigger>().NPCDialogueEvent.AddListener(NPCActivate);
        LoadDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if (_NPCActive)
            dialogueTransform.position = Camera.main.WorldToScreenPoint(_CurrentNPC.transform.position + new Vector3(0, height, 0));
    }

    void LoadDialogue()
    {
        string path = AssetDatabase.GetAssetPath(_DialogueTextFile);
        if (string.IsNullOrEmpty(path))
            Debug.Log("Could not load file path");
        StreamReader reader = new (path);
        List<List<string>> currentDialogue = _PeterDialogue;
        int currentIndex = -1;
        string currentName = "";
        string line = reader.ReadLine();

        while (line != null)
        {
            //Debug.Log(line);

            if (string.Compare(line, "Peter", true) == 0)
            {
                if (string.Compare(line, currentName, true) != 0)
                    currentIndex = -1;
                currentName = "Peter";
                currentDialogue = _PeterDialogue;
                currentDialogue.Add(new List<string>());
                currentIndex++;
            }
            else if (string.Compare(line, "Margaret", true) == 0)
            {
                if (string.Compare(line, currentName, true) != 0)
                    currentIndex = -1;
                currentName = "Margaret";
                currentDialogue = _MargaretDialogue;
                currentDialogue.Add(new List<string>());
                currentIndex++;
            }
            else if (string.Compare(line, "Thomas", true) == 0)
            {
                if (string.Compare(line, currentName, true) != 0)
                    currentIndex = -1;
                currentName = "Thomas";
                currentDialogue = _ThomasDialogue;
                currentDialogue.Add(new List<string>());
                currentIndex++;
            }
            else
            {
                currentDialogue[currentIndex].Add(line);
            }
            line = reader.ReadLine();
        }
    }

    private IEnumerator StartDia()
    {
        _ScreenNameText.text = _CurrentNPC.name;
        _NameText.text = _CurrentNPC.name;
        CharacterNames character = CharacterNames.Peter;

        List<string> currentDialogue;

        switch (_CurrentNPC.name)
        {
            case "Peter":
                character = CharacterNames.Peter;
                currentDialogue = _PeterDialogue[_DialogueIndex];
                break;
            case "Margaret":
                character = CharacterNames.Margaret;
                currentDialogue = _MargaretDialogue[_DialogueIndex];
                break;
            case "Thomas":
                character = CharacterNames.Thomas;
                currentDialogue = _ThomasDialogue[_DialogueIndex];
                break;
            default:
                currentDialogue = new() { "Invalid npc name" };
                break;
        }

        PeterScreen.enabled = character == CharacterNames.Peter;
        MargaretScreen.enabled = character == CharacterNames.Margaret;
        ThomasScreen.enabled = character == CharacterNames.Thomas;

        PeterLocal.enabled = character == CharacterNames.Peter;
        MargaretLocal.enabled = character == CharacterNames.Margaret;
        ThomasLocal.enabled = character == CharacterNames.Thomas;

        NPCDialogueEvent?.Invoke(_CurrentNPC, character);

        while (_NPCActive)
        {
            _Canvas.SetActive(true);
            _Animator.SetBool("isActive", true);

            foreach (string line in currentDialogue)
            {
                _ScreenDialogueText.text = "";



                foreach (char c in line)
                {
                    _ScreenDialogueText.text += c;
                    yield return new WaitForSeconds(TypeSpeed);
                }
                yield return new WaitForSeconds(_LineDelay);
            }

            _Canvas.SetActive(false);
            _Animator.SetBool("isActive", false);
            yield return new WaitForSeconds(Mathf.Abs(_EndDelay - _LineDelay));
        }
        yield return null;
    }

    private void StopTalking()
    {
        _NPCActive = false;
        _Canvas.SetActive(false);
        _NameText.text = "";
        _ScreenDialogueText.text = "";
        _ScreenNameText.text = "";

        _Animator.SetBool("isActive", false);
    }

    private void NPCActivate(GameObject npcObject, bool enterExit, int index)
    {
        _CurrentNPC = npcObject;
        _DialogueIndex = index;

        if (enterExit)
        {
            if (!_NPCActive)
            {
                _NPCActive = true;
                _DialogueCoroutine = StartDia();
                StartCoroutine(_DialogueCoroutine);
            }
        }
        else
        {
            StopCoroutine(_DialogueCoroutine);
            StopTalking();
        }
    }
}
