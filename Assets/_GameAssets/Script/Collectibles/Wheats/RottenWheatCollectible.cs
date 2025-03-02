using UnityEngine;

public class RottenWheatCollectible : MonoBehaviour
{
    [SerializeField] private PlayerControl _playerController;

    [SerializeField] private float _MovementSpeedDebuff;
    [SerializeField] private float _movementSpeedBoostDuration;

    public void Collect()
    {
        _playerController.SetMovementSpeed(_MovementSpeedDebuff, _movementSpeedBoostDuration);
        Destroy(this.gameObject);
    }
}
