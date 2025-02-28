using System;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using Debug = UnityEngine.Debug;

public class PlayerControl : MonoBehaviour
{
    [Header("Referances")]
    [SerializeField] private Transform _orientationTransform;

    [Header("Movement Settings")]
    [SerializeField] private KeyCode _movementKey;
    [SerializeField] private float _movementSpeed;

    [Header("Jump Settings")]
    [SerializeField] private KeyCode _jumpKey;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpCooldown;
    [SerializeField] private float _airMultiplier;
    [SerializeField] private float _airDrag;
    [SerializeField] private bool _canJump;
    [Header("Sliding Settings")]
    [SerializeField] private KeyCode _slideKey;
    [SerializeField] private float _slideMultiplier;
    [SerializeField] private float _slideDrag;

    [Header("Ground Check Settings")]
    [SerializeField] private float _playerHeight;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundDrag;

    private StateController _stateController;
    private Rigidbody _playerRigidbody;
    private float _VerticalInput, _HorizontalInput;
    private Vector3 _MovementDirection;
    private bool _isSlideing;
    void Awake()
    {
        _stateController = GetComponent<StateController>();
        _playerRigidbody = GetComponent<Rigidbody>();
        _playerRigidbody.freezeRotation = true;
    }
    private void Update() 
    {
        SetInputs();
        SetStates();
        SetPlayerDrag();
        LimitPlayerSpeed();
    }
    private void FixedUpdate() 
    {
        SetPlayerMovement();    
    }
    private void SetInputs()
    {
        _HorizontalInput = Input.GetAxisRaw("Horizontal");
        _VerticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(_slideKey)) 
        {
            _isSlideing = true;
        }
        else if  (Input.GetKeyDown(_movementKey))
        {
            _isSlideing = false;
        }

        else if (Input.GetKey(_jumpKey) && _canJump && IsGrounded()) 
        {
            _canJump = false;
            SetPlayerJump();
            Invoke(nameof(ResetJumping), _jumpCooldown);           
        }
    }
    private void SetStates() 
    {
        var movementDirection = GetMovementDirection();
        var isGrounded = IsGrounded();
        var isSliding = IsSliding();
        var currenState = _stateController.GetCurrentState();

        var newState = currenState switch 
        {
            _ when movementDirection == Vector3.zero && isGrounded && !isSliding => PlayerState.Idle,
            _ when movementDirection != Vector3.zero && isGrounded && !isSliding => PlayerState.Move,
            _ when movementDirection != Vector3.zero && isGrounded && isSliding => PlayerState.Slide,
            _ when movementDirection == Vector3.zero && isGrounded && isSliding => PlayerState.SlideIdle,
            _ when !_canJump && !isGrounded => PlayerState.Jump,
            _ => currenState
        };

        if (newState != currenState) 
        {
            _stateController.ChangeState(newState);
        }
    }
    private void SetPlayerMovement() 
    {
        _MovementDirection = _orientationTransform.forward * _VerticalInput 
        + _orientationTransform.right * _HorizontalInput;

        float forceMultiplier = _stateController.GetCurrentState() switch 
        {
            PlayerState.Move => 1f,
            PlayerState.Slide => _slideMultiplier,
            PlayerState.Jump => _airMultiplier,
            _ => 1f,
        };
        _playerRigidbody.AddForce(_MovementDirection.normalized * _movementSpeed * forceMultiplier, ForceMode.Force);
    }
    private void SetPlayerDrag() 
    {
        _playerRigidbody.linearDamping = _stateController.GetCurrentState() switch 
        {
            PlayerState.Move => _groundDrag,
            PlayerState.Slide => _slideDrag,
            PlayerState.Jump => _airDrag,
            _ => _playerRigidbody.linearDamping,
        };

        if (_isSlideing) 
        {
            _playerRigidbody.linearDamping = _slideDrag;
        }else {
            _playerRigidbody.linearDamping = _groundDrag;
        }
    }
    private void LimitPlayerSpeed() 
    {
        Vector3 flatVelocity = new Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);
        if (flatVelocity.magnitude > _movementSpeed) 
        {
            Vector3 limitedVelocity = flatVelocity.normalized * _movementSpeed;
            _playerRigidbody.linearVelocity = new Vector3(limitedVelocity.x, _playerRigidbody.linearVelocity.y, limitedVelocity.z);
        }
    }

    private void SetPlayerJump() 
    {
        _playerRigidbody.linearVelocity = new Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);
        _playerRigidbody.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }
    private void ResetJumping() 
    {
        _canJump = true;
    }
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + 0.2f, _groundLayer);
    }
    private Vector3 GetMovementDirection() 
    {
        return _MovementDirection.normalized;
    }
    private bool IsSliding() 
    {
        return _isSlideing;
    }
}

