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
        //Moves the object from its original place to the new one witha  public speed variable
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards
            (transform.position, targetPosition.position, moveSpeed * Time.deltaTime);

        if (transform.position == targetPosition.position)
            isMoving = false;
    }
}