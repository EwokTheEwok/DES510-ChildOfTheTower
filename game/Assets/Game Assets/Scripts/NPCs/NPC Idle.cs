using UnityEngine;

public class NPCIdle : MonoBehaviour
{
    [Header("Animations Settings")]
    [SerializeField] private float _MinTimeWait = 8.0f;
    [SerializeField] private float _MaxTimeWait = 10.0f;
    private Animator _Animator;
    private float _RandomMulti;
    private float _TotalWaitTime;
    private float _GoalWaitTime;

    private void Awake()
    {
        _Animator = GetComponentInChildren<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateIdleWaitTime();
    }

    // Update is called once per frame
    void Update()
    {
        PlayIdle2();
    }

    void PlayIdle2()
    {

        _TotalWaitTime += Time.deltaTime;

        if (_TotalWaitTime > _GoalWaitTime)
        {
            _Animator.SetTrigger("Play Idle 2");
            _TotalWaitTime = 0f;
            UpdateIdleWaitTime();
        }
    }

    void UpdateIdleWaitTime()
    {
        _RandomMulti = Random.Range(0.0f, 1.0f);
        _GoalWaitTime = _MinTimeWait + ((_MaxTimeWait - _MinTimeWait) * _RandomMulti);

    }
}
