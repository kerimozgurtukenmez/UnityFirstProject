using UnityEngine;

public class HolyWheatCollectible : MonoBehaviour, ICollectible
{
    [SerializeField] private PlayerControl _playerController;
    [SerializeField] private WheatDesignSO _wheatDesignSO;

    public void Collect()
    {
        _playerController.SetJumpForce(_wheatDesignSO._IncreaseDecraseMultiplier, _wheatDesignSO._ResetBoostDuration);
        Destroy(this.gameObject);
    }
}
