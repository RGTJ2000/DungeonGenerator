using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;
    public float lookSensitivity = 0.5f;
    public float jumpForce = 5f;

    private CharacterController _controller;
    private InputActions _inputActions;
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private float _verticalVelocity;
    private bool _jumpPressed;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _inputActions = new InputActions();


    }

    private void OnEnable()
    {
        _inputActions.General.Enable();
    }

    private void OnDisable()
    {
        _inputActions.General.Disable();
    }

    private void Update()
    {
        // Read input values
        _moveInput = _inputActions.General.MovePlayer.ReadValue<Vector2>();
        _lookInput = _inputActions.General.MouseLook.ReadValue<Vector2>();


        // Movement
        Vector3 move = (transform.right * _moveInput.x + transform.forward * _moveInput.y) * moveSpeed;
        _controller.Move(move * Time.deltaTime);

        // Mouse Look (horizontal only)
        transform.Rotate(Vector3.up * _lookInput.x * lookSensitivity);

        /*
        // Jump
        if (_controller.isGrounded)
        {
            _verticalVelocity = -0.5f; // Small ground stick
            if (_jumpPressed) _verticalVelocity = jumpForce;
        }
        _verticalVelocity += Physics.gravity.y * Time.deltaTime;
        _controller.Move(Vector3.up * _verticalVelocity * Time.deltaTime);
        */

    }

}

  