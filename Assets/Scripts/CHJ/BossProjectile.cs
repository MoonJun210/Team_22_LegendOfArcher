using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float speed = 5f;
    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        Debug.Log("SetDirection 호출됨, 방향: " + direction);
    }

    private void Update()
    {
        Debug.Log("Projectile 이동 중, 현재 방향: " + direction);
        transform.Translate(direction * speed * Time.deltaTime);
    }

//private void OnTriggerEnter2D(Collider2D collision)
//    {
//        if (collision.CompareTag("Player"))
//        {
//            Debug.Log("Player hit by projectile!");
//            // TODO: 데미지 적용
//            Destroy(gameObject);
//        }
//        else if (!collision.isTrigger)
//        {
//            // 벽 같은 거에 부딪히면 제거
//            Destroy(gameObject);
//        }
//    }
}
