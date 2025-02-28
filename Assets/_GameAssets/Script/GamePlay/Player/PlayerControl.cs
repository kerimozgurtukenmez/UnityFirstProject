using System;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] private bool _canJump;
    [Header("Sliding Settings")]
    [SerializeField] private KeyCode _slideKey;
    [SerializeField] private float _slideMultiplier;
    [SerializeField] private float _slideDrag;

    [Header("Ground Check Settings")]
    [SerializeField] private float _playerHeight;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundDrag;


    private Rigidbody _playerRigidbody;
    private float _VerticalInput, _HorizontalInput;
    private Vector3 _MovementDirection;
    private bool _isSlideing;
    void Awake()
    {
        _playerRigidbody = GetComponent<Rigidbody>();
        _playerRigidbody.freezeRotation = true;
    }
    private void Update() 
    {
        SetInputs();
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
    private void SetPlayerMovement() 
    {
        _MovementDirection = _orientationTransform.forward * _VerticalInput 
        + _orientationTransform.right * _HorizontalInput;
        if (_isSlideing) 
        {
            _playerRigidbody.AddForce(_MovementDirection.normalized * _movementSpeed * _slideMultiplier, ForceMode.Force);
        }
        else 
        {
            _playerRigidbody.AddForce(_MovementDirection.normalized * _movementSpeed, ForceMode.Force);
        }
    }
    private void SetPlayerDrag() 
    {
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
}

