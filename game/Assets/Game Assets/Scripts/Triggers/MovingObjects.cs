using UnityEngine;

public class MovingObjects : MonoBehaviour
{
    public Transform targetPosition;
    public float moveSpeed = 5f;
    public float rotateSpeed = 90f;
    private bool isMoving;

    public void MoveBrick()
    {
        isMoving = true;
    }

    void Update()
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetPosition.rotation, rotateSpeed * Time.deltaTime);

        if (transform.position == targetPosition.position && transform.rotation == targetPosition.rotation)
            isMoving = false;
    }
}