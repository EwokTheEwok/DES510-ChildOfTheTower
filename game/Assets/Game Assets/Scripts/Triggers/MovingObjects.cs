using UnityEngine;
using UnityEngine.Events;

public class MovingObjects : MonoBehaviour
{
    public Transform targetPosition;
    public float moveSpeed = 5f;
    private bool isMoving;

    public void MoveBrick()
    {
        isMoving = true;
    }

    void Update()
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards
            (transform.position, targetPosition.position, moveSpeed * Time.deltaTime);

        if (transform.position == targetPosition.position)
            isMoving = false;
    }
}