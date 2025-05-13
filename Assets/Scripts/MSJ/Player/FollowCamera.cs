using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    private Transform target;
    private Transform player;
    float offsetX;
    float offsetY;

    [SerializeField] private float smoothSpeed = 5f; // 부드럽게 따라가는 정도

    private void Awake()
    {
        EventManager.Instance.RegisterEvent<GameObject>("SearchTarget", SearchTarget);
    }


    private void FixedUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = new Vector3(
           target.position.x,
           target.position.y,
           transform.position.z
       );

        // Lerp를 통해 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }

    void SearchTarget(GameObject target)
    {
        this.target = target.transform;
        offsetX = transform.position.x - this.target.position.x;
        offsetY = transform.position.y - this.target.position.y;

        Debug.Log("타겟변경");
    }

  
}
