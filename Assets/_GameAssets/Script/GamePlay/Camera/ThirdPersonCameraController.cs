using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    [Header("Referances")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _oriantationTransform;
    [SerializeField] private Transform _playerVisualTransform;
    [Header("Settings")]
    [SerializeField] private float _rotationSpeed;
    private void Update()
    {
        Vector3 viewDirection = _playerTransform.position - new Vector3(transform.position.x, _playerTransform.position.y, transform.position.z);

        _oriantationTransform.forward = viewDirection.normalized;

        float horizentalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 inputDirection = _oriantationTransform.forward * verticalInput + _oriantationTransform.right * horizentalInput;

        if (inputDirection != Vector3.zero) 
        {
            _playerVisualTransform.forward 
            = Vector3.Slerp(_playerVisualTransform.forward, inputDirection.normalized, Time.deltaTime * _rotationSpeed);
        }
    }
}
