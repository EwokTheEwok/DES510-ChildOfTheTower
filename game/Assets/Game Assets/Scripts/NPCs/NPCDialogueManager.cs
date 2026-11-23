using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum CharacterNames
{
    Peter,
    Margaret,
    Thomas
}

public class NPCDialogueManager : MonoBehaviour
{
    [SerializeField] TextAsset _DialogueTextFile;
    List<List<string>> _PeterDialogue = new();
    List<List<string>> _MargaretDialogue = new();
    List<List<string>> _ThomasDialogue = new();

    public static NPCDialogueManager Instance { get => _Instance; }
    private static NPCDialogueManager _Instance;

    [Header("NPC")]
    [SerializeField] string _NPCTag = "NPC";
    private List<GameObject> _NPCList = new();
    private bool _NPCActive;
    private GameObject _CurrentNPC;
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
        /*
        _NPCList = GameObject.FindGameObjectsWithTag(_NPCTag).ToList();
        foreach (GameObject npcObject in _NPCList)
        {
            npcObject.GetComponent<NPC>().AddListener(NPCActivate);
        }
        */
        //GameManager.Instance.Player.GetComponentInChildren<PlayerDialogueTrigger>().NPCDialogueEvent.AddListener(NPCActivate);
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
        //"Assets/Game Assets/Scripts/Game/CharacterDialogue.txt"
        StreamReader reader = new StreamReader(AssetDatabase.GetAssetPath(_DialogueTextFile));
        List<List<string>> currentDialogue = _PeterDialogue;
        int currentIndex = -1;
        string currentName = "";
        string line = reader.ReadLine();

        while (line != null)
        {
            Debug.Log(line);

            if (string.Compare(line, "Peter", true) == 0)
            {
                if (string.Compare(line, currentName, true) != 0)
                {
                    currentIndex = -1;
                }
                currentName = "Peter";
                currentDialogue = _PeterDialogue;
                currentDialogue.Add(new List<string>());
                currentIndex++;
            }
            else if (string.Compare(line, "Margaret", true) == 0)
            {
                if (string.Compare(line, currentName, true) != 0)
                {
                    currentIndex = -1;
                }
                currentName = "Margaret";
                currentDialogue = _MargaretDialogue;
                currentDialogue.Add(new List<string>());
                currentIndex++;
            }
            else if (string.Compare(line, "Thomas", true) == 0)
            {
                if (string.Compare(line, currentName, true) != 0)
                {
                    currentIndex = -1;
                }
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

        foreach (List<string> peterlines in _PeterDialogue)
        {
            foreach (string pline in peterlines)
            {
                Debug.Log(pline);
            }
        }

        foreach (List<string> margaretlines in _MargaretDialogue)
        {
            foreach (string mline in margaretlines)
            {
                Debug.Log(mline);
            }
        }

        foreach (List<string> thomaslines in _ThomasDialogue)
        {
            foreach (string tline in thomaslines)
            {
                Debug.Log(tline);
            }
        }
    }

    private IEnumerator StartDia()
    {
        NPC npc = _CurrentNPC.GetComponent<NPC>();
        _ScreenNameText.text = npc.NPCName;
        _NameText.text = npc.NPCName;


        while (true)
        {
            _Canvas.SetActive(true);
            _Animator.SetBool("isActive", true);
            //GameObject.Find(_NameText.text).GetComponent<SpriteRenderer>().enabled = true;

            foreach (string line in npc.DialogueLines)
            {
                _ScreenDialogueText.text = "";

                if (_NameText.text == "Peter")
                {
                    PeterScreen.enabled = true;
                    MargaretScreen.enabled = false;
                    ThomasScreen.enabled = false;

                    PeterLocal.enabled = true;
                    MargaretLocal.enabled = false;
                    ThomasLocal.enabled = false;
                    NPCDialogueEvent?.Invoke(_CurrentNPC, CharacterNames.Peter);

                }
                else if (_NameText.text == "Margaret")
                {
                    PeterScreen.enabled = false;
                    MargaretScreen.enabled = true;
                    ThomasScreen.enabled = false;

                    PeterLocal.enabled = false;
                    MargaretLocal.enabled = true;
                    ThomasLocal.enabled = false;
                    NPCDialogueEvent?.Invoke(_CurrentNPC, CharacterNames.Margaret);

                }
                else if (_NameText.text == "Thomas")
                {
                    PeterScreen.enabled = false;
                    MargaretScreen.enabled = false;
                    ThomasScreen.enabled = true;

                    PeterLocal.enabled = false;
                    MargaretLocal.enabled = false;
                    ThomasLocal.enabled = true;
                    NPCDialogueEvent?.Invoke(_CurrentNPC, CharacterNames.Thomas);

                }

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
