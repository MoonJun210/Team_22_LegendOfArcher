using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineFollowBoss : MonoBehaviour
{
    private Transform target;
    public float followSpeed = 2f;



    private void Awake()
    {
        target = BossSpawner.Instance.curBoss.transform;
    }

    private void Update()
    {
        if (target == null) return;

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * followSpeed * Time.deltaTime;
    }
}
