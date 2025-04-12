using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Camera")]
    public Transform cameraPivot; // Assign camera transform in Inspector
    public Camera _camera;
    private float _pitch;
    public float sensitivityHor = 0.3f;
    public float sensitivityVert = 0.3f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 1f;  // Typical value: 1.5-3f
    [SerializeField] private float gravity = -9.81f; // Default Unity gravity



    [Header("Settings")]
    public float moveSpeed = 5f;
    public float lookSensitivity = 0.5f;
    public float jumpForce = 5f;
    public float turnSpeed = 1f;

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

        _camera = cameraPivot.gameObject.GetComponent<Camera>();

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

        // Input reading (unchanged)
        _moveInput = _inputActions.General.MovePlayer.ReadValue<Vector2>();
        _jumpPressed = _inputActions.General.Jump.IsPressed();

        if (_camera.enabled == true)
        {
            _lookInput = _inputActions.General.MouseLook.ReadValue<Vector2>();
            float delta = _lookInput.x * sensitivityHor;
            float rotationYaxis = transform.localEulerAngles.y + delta;
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotationYaxis, 0);

            float currentAngle = cameraPivot.localEulerAngles.x;
            if (currentAngle > 180) currentAngle -= 360;
            float targetAngle = currentAngle - (_lookInput.y * sensitivityVert);
            targetAngle = Mathf.Clamp(targetAngle, -45f, 45f);
            cameraPivot.localEulerAngles = new Vector3(targetAngle, cameraPivot.localEulerAngles.y, 0);



        }
        else
        {
            float delta = _moveInput.x * turnSpeed;
            float rotationYaxis = transform.localEulerAngles.y + delta;
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotationYaxis, 0);
            _moveInput.x = 0;

        }

       


        // Movement - COMBINED into single Move() call
        Vector3 horizontalMove = (transform.right * _moveInput.x + transform.forward * _moveInput.y) * moveSpeed;
        Vector3 verticalMove = Vector3.up * _verticalVelocity;
        _controller.Move((horizontalMove + verticalMove) * Time.deltaTime);

        // Jump/Gravity (unchanged)
        _verticalVelocity += gravity * Time.deltaTime;

        if (_controller.isGrounded)
        {
            _verticalVelocity = -0.1f;
            if (_jumpPressed)
            {
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }


    }

}

  