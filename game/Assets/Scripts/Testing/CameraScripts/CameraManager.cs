using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;


public class CameraManager : MonoBehaviour
{
    private Vector2 delta;

    private bool isMoving;
    private bool isRotating;
    private bool isBusy;

    private float xRotation;

    [SerializeField] private float movementSpeed = 10.0f;
    [SerializeField] private float rotationSpeed = 0.5f;

    private void Awake()
    {
        xRotation = transform.rotation.eulerAngles.x;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        delta = context.ReadValue<Vector2>();
       
    }

    public void OnMove(InputAction.CallbackContext context)
    {

        isMoving = context.started || context.performed;

    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (isBusy) return;

        isRotating = context.started || context.performed;

        if (context.canceled)
        {
            isBusy = true;
            SnapRotation();
        }
    }

    private void LateUpdate()
    {
        if (isMoving)
        {
            var position = transform.right * (delta.x * -movementSpeed);
            position += transform.up * (delta.y * -movementSpeed);
            transform.position += position * Time.deltaTime; 
        }

        if (isRotating)
        {
            transform.Rotate(new Vector3(xRotation, delta.x * rotationSpeed, 0.0f));
            transform.rotation = Quaternion.Euler(xRotation, transform.rotation.eulerAngles.y, 0.0f);
        }
    }

    private void SnapRotation()
    {
        transform.DORotate(SnappedVector(), 0.5f)
            .SetEase(Ease.OutBounce)
            .OnComplete(() =>
            {
                isBusy = false;
            });
    }

    private Vector3 SnappedVector()
    {
        var endValue = 0.0f;
        var currentY = Mathf.Ceil(transform.rotation.eulerAngles.y);

        endValue = currentY switch
        {
            >= 0 and <= 90 => 45.0f,
            >= 91 and <= 180 => 135.0f,
            >= 181 and <= 270 => 225.0f,
            _ => 315.0f
        };

        return new Vector3(xRotation, endValue, 0.0f);
    }
}
