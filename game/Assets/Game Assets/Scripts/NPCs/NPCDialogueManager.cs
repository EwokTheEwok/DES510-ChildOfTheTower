using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class NPCDialogueManager : MonoBehaviour
{
    [Header("NPC")]
    public string _NPCTag = "NPC";
    private List<GameObject> _NPCList = new();
    private bool _NPCActive;
    private GameObject _CurrentNPC;
    private bool _NPCTalking;

    [Header("NPC Local Canvas")]
    [SerializeField] private GameObject _LocalCanvas;
    [SerializeField] private GameObject _DialogueBox;
    [SerializeField] private GameObject _NameBox;
    [SerializeField] private TMP_Text _NameText;
    private RectTransform dialogueTransform;

    [Header("NPC Screen Canvas")]
    [SerializeField] private GameObject _ScreenCanvas;
    [SerializeField] private GameObject _ScreenDialogueBox;
    [SerializeField] private GameObject _ScreenNameBox;
    [SerializeField] private TMP_Text _ScreenDialogueText;
    [SerializeField] private TMP_Text _ScreenNameText;

    [Header("NPC Vars")]
    public float _LineDelay = 2f;
    public float _EndDelay = 7f;
    public float height = 10f;
    public float TypeSpeed = 0.05f;
    private IEnumerator _DialogueCoroutine;

    [Header("NPC Think Bubble")]
    private Animator _Animator;

    [Header("Events")]
    public UnityEvent<GameObject> _PeterEvent = new();
    public UnityEvent<GameObject> _MargaretEvent = new();
    public UnityEvent<GameObject> _ThomasEvent = new();



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _Animator = GetComponentInChildren<Animator>(true);

        dialogueTransform = _DialogueBox.transform as RectTransform;
        _NPCList = GameObject.FindGameObjectsWithTag(_NPCTag).ToList();
        foreach (GameObject npcObject in _NPCList)
        {
            npcObject.GetComponent<NPC>().AddListener(NPCActivate);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_NPCActive)
            dialogueTransform.position = Camera.main.WorldToScreenPoint(_CurrentNPC.transform.position + new Vector3(0, height, 0));
    }

    private IEnumerator StartDia()
    {
        _NPCTalking = true;
        NPC npc = _CurrentNPC.GetComponent<NPC>();
        _ScreenNameText.text = npc.NPCName;
        _NameText.text = npc.NPCName;


        while (true)
        {
            _LocalCanvas.SetActive(true);
            _ScreenCanvas.SetActive(true);
            _Animator.SetBool("isActive", true);

            foreach (string line in npc.DialogueLines)
            {
                _ScreenDialogueText.text = "";

                if (_NameText.text == "Peter")
                {
                    _PeterEvent?.Invoke(_CurrentNPC);

                } else if (_NameText.text == "Margaret")
                {
                    _MargaretEvent?.Invoke(_CurrentNPC);

                }
                else if (_NameText.text == "Thomas")
                {
                    _ThomasEvent?.Invoke(_CurrentNPC);

                }

                foreach (char c in line)
                    {
                        _ScreenDialogueText.text += c;
                        yield return new WaitForSeconds(TypeSpeed);
                    }
                yield return new WaitForSeconds(_LineDelay);
            }

            _ScreenCanvas.SetActive(false);
            _LocalCanvas.SetActive(false);
            _Animator.SetBool("isActive", false);
            yield return new WaitForSeconds(Mathf.Abs(_EndDelay - _LineDelay));
        }
    }

    private void StopTalking()
    {
        _NPCActive = false;
        _NPCTalking = false;
        _LocalCanvas.SetActive(false);
        _NameText.text = "";
        
        _ScreenDialogueText.text = "";
        _ScreenCanvas.SetActive(false);
        _ScreenNameText.text = "";

        _Animator.SetBool("isActive", false);
    }

    private void NPCActivate(GameObject npcObject, bool enterExit)
    {
        _CurrentNPC = npcObject;
        if (enterExit)
        {
            if (!_NPCActive)
            {
                _NPCActive = true;
                _DialogueCoroutine = StartDia();
                //_AnimationCoroutine = StartAnimation();
                StartCoroutine(_DialogueCoroutine);
                //StartCoroutine(_AnimationCoroutine);
            }
        }
        else
        {
            StopCoroutine(_DialogueCoroutine);
            //StopCoroutine(_AnimationCoroutine);
            StopTalking();
        }
    }
}
