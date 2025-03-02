using UnityEngine;

public class HolyWheatCollectible : MonoBehaviour
{
    [SerializeField] private PlayerControl _playerController;

    [SerializeField] private float _jumpForceBuff;
    [SerializeField] private float _jumpForceDuration;

    public void Collect()
    {
        _playerController.SetJumpForce(_jumpForceBuff, _jumpForceDuration);
        Destroy(this.gameObject);
    }
}
