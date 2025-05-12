using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TempTriggerZone : MonoBehaviour
{
    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered boss trigger zone!");
            UpgradeMenu.Instance.OnBossDefeated();
        }
    }
}
