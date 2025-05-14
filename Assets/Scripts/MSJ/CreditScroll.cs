using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditScroll : MonoBehaviour
{
    public float scrollSpeed = 30f;

    float timer = 0;

    private void Start()
    {
        EventManager.Instance.TriggerEvent("FadeOut", 1f);
        SoundManager.instance.ChangeBackGroundMusic("EndingBGM");
    }

    void Update()
    {
        timer += Time.deltaTime;
        if(timer < 1.5)
        {
            return;
        }
        if (Input.GetMouseButton(0))
        {
            scrollSpeed = 100f;
        }
        else
        {
            scrollSpeed = 30f;
        }

        if (this.transform.position.y > 1500)
        {
            //#if UNITY_EDITOR
            //            UnityEditor.EditorApplication.isPlaying = false;  // 에디터에서 플레이 모드 종료
            //#else
            //    Application.Quit();  // 빌드된 게임 종료
            //#endif
            return;
        }
        transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);

       
    }
}
