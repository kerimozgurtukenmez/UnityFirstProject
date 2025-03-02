using UnityEngine;

public class GoldWheatCollectible : MonoBehaviour
{
    [SerializeField] private PlayerControl _playerController;

    [SerializeField] private float _MovementSpeedBuff;
    [SerializeField] private float _movementSpeedBoostDuration;

    public void Collect()
    {
        _playerController.SetMovementSpeed(_MovementSpeedBuff, _movementSpeedBoostDuration);
        Destroy(this.gameObject);
    }
}
