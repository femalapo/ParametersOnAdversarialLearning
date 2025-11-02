using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerManual : MonoBehaviour
{
    InputAction moveAction;
    InputAction rotateAction;
    InputAction shootAction;

    PlayerController playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        rotateAction = InputSystem.actions.FindAction("Rotate");
        shootAction = InputSystem.actions.FindAction("Shoot");

        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        playerController.Move(moveValue);

        float rotateValue = rotateAction.ReadValue<float>();
        playerController.Rotate(rotateValue);

        if(shootAction.ReadValue<float>() == 1) { playerController.Shoot(); }
    }
}
