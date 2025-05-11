using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TempTriggerZone : MonoBehaviour
{
    private void Reset()
    {
        // Collider2D 기본 설정: Trigger로 만들기
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어 태그 확인 (Player 태그가 붙어 있어야 합니다)
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered boss trigger zone!");
            UpgradeMenu.Instance.OnBossDefeated();
            // 이후 한 번만 실행하고 싶다면, 이 오브젝트를 비활성화
            // gameObject.SetActive(false);
        }
    }
}
