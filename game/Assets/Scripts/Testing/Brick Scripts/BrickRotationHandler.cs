using UnityEngine;
using UnityEngine.InputSystem;

public class BrickRotationHandler : MonoBehaviour
{
    [SerializeField] private Transform _cameraRefPoint;

    [SerializeField] private LayerMask _brickMask;

    [SerializeField] private Camera[] _cameras;

    private Vector3 _cameraPosition;


    // Disabling second test camera
    private void Awake()
    {
        _cameras[1].gameObject.SetActive(false);
        _cameras[0] = Camera.main;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cameraPosition = Camera.main.transform.position;
    }

    // Update is called once per frame
    // Switch camera to secondary test camera
    void Update()
    {
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            _cameras[1].gameObject.SetActive(true);
            _cameras[1] = Camera.main;
        }
    }

    private void FixedUpdate()
    {
        //RaycastHit hit;

        //if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, _brickMask))
        //{
        //    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        //}
    }
}
