using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var playerInput = GetComponent<PlayerInput>();
        var actionMap = playerInput.actions.FindActionMap("Player");
        actionMap.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
