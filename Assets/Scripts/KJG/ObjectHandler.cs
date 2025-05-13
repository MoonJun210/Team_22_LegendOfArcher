using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectHandler : MonoBehaviour
{
    [SerializeField] private float moveObjDistance = 1f; // 미는거리 
    [SerializeField] private float moveObjSpeed = 3f;  // 미는속도 

    private GameObject target;
    private bool isMoving = false;
    private bool canPush = false;

    private void Update()
    {
        DetectObj();
    }

    public void PushObj(Vector2 direction)
    {
        if (!isMoving)
        {
            StartCoroutine(MoveObject(direction));
        }
    }

    private IEnumerator MoveObject(Vector2 direction)
    {
        isMoving = true;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + (Vector3)direction * moveObjDistance;
        Vector3 targetStartPos = target.transform.position;
        Vector3 targetEndPos = targetStartPos + (Vector3)direction * moveObjDistance;

        float t = 0f;
        while (t < 1f)
        {
            target.transform.position = Vector3.Lerp(targetStartPos, targetEndPos, t);
            transform.position = Vector3.Lerp(startPos, endPos, t);
            t += Time.deltaTime * moveObjSpeed;
            yield return null;
        }

        transform.position = endPos;
        target.transform.position = targetEndPos;
        isMoving = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PushObject"))
        {
            canPush = true;
            target = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PushObject"))
        {
            canPush = false;
            target = null;
        }
    }

    public void DetectObj()
    {
        if (Input.GetKeyDown(KeyCode.F) && canPush)
        {
            Vector2 targetDirection = target.transform.position - transform.position;
            if (Mathf.Abs(targetDirection.x) > Mathf.Abs(targetDirection.y))
            {
                targetDirection.y = 0;
                targetDirection = targetDirection.normalized;
                PushObj(targetDirection);
            }
            else if (Mathf.Abs(targetDirection.x) < Mathf.Abs(targetDirection.y))
            {
                targetDirection.x = 0;
                targetDirection = targetDirection.normalized;
                PushObj(targetDirection);
            }
            else { }
        }
    }
}
