using System.Collections;
using UnityEngine;

public class DieExplosion : MonoBehaviour
{
    [SerializeField] private GameObject dieParticle;
    [SerializeField] private float explosionDelay = 1.5f;
    [SerializeField] private float returnDelay = 1f;
    [SerializeField] private Vector3 particleOffset = new Vector3(0, 0, -2);

    [SerializeField] private GameObject upgradeZone;

    public void ExecuteDeathSequence()
    {
        // 플레이어 타겟 재설정
        EventManager.Instance.TriggerEvent("SearchTarget", gameObject);
        Invoke(nameof(HandleExplosion), explosionDelay);
    }

    private void HandleExplosion()
    {
        Instantiate(dieParticle, transform.position + particleOffset, Quaternion.identity);
        gameObject.SetActive(false);
        Invoke(nameof(ReturnToPlayerAndCleanUp), returnDelay);
    }

    private void ReturnToPlayerAndCleanUp()
    {
        GameManager.instance.CameraTargetToPlayer();
        MapManager.MapInstance.ChagneMapCondition(2);
        upgradeZone.transform.position = this.gameObject.transform.position;
        upgradeZone.SetActive(true);
        Destroy(gameObject);
    }
}
