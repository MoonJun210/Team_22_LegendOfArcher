using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class TempTriggerZone : MonoBehaviour
{
    GameObject map;
    private void Start()
    {
        map = GameObject.Find("Map");
    }

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Map 오브젝트를 찾아 SixthGrid 자식이 켜져있는지 확인
            if (map != null)
            {
                Transform sixthGrid = map.transform.Find("SixthGird");
                if (sixthGrid != null && sixthGrid.gameObject.activeSelf)
                {
                    // EndingScene으로 이동
                    SceneManager.LoadScene("EndingScene");
                    return; // 이후 코드 실행 안함
                }
            }
            Debug.Log("Player entered boss trigger zone!");
            UpgradeMenu.Instance.OnBossDefeated();
            SoundManager.PlayClip("UpgradeSound");
            this.gameObject.SetActive(false);
        }
    }
}
