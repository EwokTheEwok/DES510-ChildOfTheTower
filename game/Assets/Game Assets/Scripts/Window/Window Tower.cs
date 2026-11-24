using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class Tower : MonoBehaviour
{
    GameObject _Player;
    Camera _Camera;
    GameObject _Tower;
    Tween _TweenObject;

    float _TransitionValue = 1f;
    readonly float _TweenDuration = 1f;

    float _TrackingY;
    [SerializeField] float _SmoothTime = 0.5f;
    float _Speed = 0f;

    bool _Active = false;

    [Header("Events")]
    [HideInInspector] public UnityEvent<bool> InsideEvent = new();

    private void Start()
    {
        _Camera = Camera.main;
        _Tower = gameObject;
        _Player = GameManager.Instance.Player;
        _TrackingY = _Player.transform.position.y;
        Shader.SetGlobalInteger("_Tower_Active", 1);
        Shader.SetGlobalFloat("_Tower_Transition", _TransitionValue);

        Vector3 cameraRotation = _Camera.transform.eulerAngles;
        Vector3 TowerPos = _Tower.transform.position;

        Shader.SetGlobalVector("_Tower_Position", TowerPos);
        Shader.SetGlobalVector("_Tower_Camera_Rotation", cameraRotation);
        Shader.SetGlobalFloat("_Tower_Player_Y", 0f);
    }

    void Update()
    {
        if (_Active) {

            _TrackingY = Mathf.SmoothDamp(_TrackingY, _Player.transform.position.y, ref _Speed, _SmoothTime);

            Vector3 cameraRotation = _Camera.transform.eulerAngles;
            Vector3 TowerPos = _Tower.transform.position;
            
            Shader.SetGlobalVector("_Tower_Position", TowerPos);
            Shader.SetGlobalVector("_Tower_Camera_Rotation", cameraRotation);
            Shader.SetGlobalFloat("_Tower_Player_Y", _TrackingY);
        }
        Shader.SetGlobalFloat("_Tower_Transition", _TransitionValue);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.PlayerCamera.SetInsideMode(true);
            _Active = true;
            InsideEvent?.Invoke(true);
            _TweenObject.Complete();
            _TweenObject = DOTween.To(() => _TransitionValue, x => _TransitionValue = x, 0f, _TweenDuration).SetEase(Ease.InOutSine);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!GameManager.Instance.PlayerDied)
                GameManager.Instance.PlayerCamera.SetInsideMode(false);
            _Active = false;
            InsideEvent?.Invoke(false);
            _TweenObject.Complete();
            _TweenObject = DOTween.To(() => _TransitionValue, x => _TransitionValue = x, 1f, _TweenDuration).SetEase(Ease.InOutSine);
        }
    }
} 