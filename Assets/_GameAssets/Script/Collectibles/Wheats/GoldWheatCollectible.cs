using UnityEngine;

public class GoldWheatCollectible : MonoBehaviour, ICollectible
{
    [SerializeField] private PlayerControl _playerController;
    [SerializeField] private WheatDesignSO _wheatDesignSO;

    public void Collect()
    {
        _playerController.SetMovementSpeed(_wheatDesignSO._IncreaseDecraseMultiplier, _wheatDesignSO._ResetBoostDuration);
        Destroy(this.gameObject);
    }
}
