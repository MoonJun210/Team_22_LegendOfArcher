using System.Collections;
using UnityEngine;

public class EndingAnimations : MonoBehaviour
{
    public Animator[] animators;
    public Vector2 intervalRange = new Vector2(2f, 5f); // 최소~최대 주기 랜덤

    private void Start()
    {
        foreach (var animator in animators)
        {
            if (animator != null)
            {
                StartCoroutine(TriggerWithRandomInterval(animator));
            }
        }
    }

    private IEnumerator TriggerWithRandomInterval(Animator animator)
    {
        while (true)
        {
            float waitTime = Random.Range(intervalRange.x, intervalRange.y);
            yield return new WaitForSeconds(waitTime);
            animator.SetTrigger("IsJump");

        }
    }
}
