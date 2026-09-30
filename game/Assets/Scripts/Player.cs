using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    InputAction m_MoveAction;
    float m_Speed = 0.1f;

    void Start()
    {
        //var playerInput = GetComponent<PlayerInput>();
        //var actionMap = playerInput.actions.FindActionMap("Player");
        //actionMap.Enable();

        m_MoveAction = InputSystem.actions.FindAction("WASD");
    }

    void Update()
    {
        Vector2 moveVel = m_MoveAction.ReadValue<Vector2>() * m_Speed;
        this.GetComponentInChildren<CharacterController>().Move(new Vector3(moveVel.x, 0, moveVel.y));
    }
}
