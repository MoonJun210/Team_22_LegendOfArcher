
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWarningShooter : MonoBehaviour
{
    [SerializeField] private GameObject directionLinePrefab;
    [SerializeField] private GameObject projectilePrefab;

    public IEnumerator Fire(
        Vector3 origin,
        Vector2 direction,
        float delay = 1f,
        float lineLength = 20f,
        float lineWidth = 0.1f,
        Color? colorOverride = null,
        bool fireProjectile = true,
        float growDuration = 0.2f
    )
    {
        // 경고선(LineRenderer) 오브젝트 생성
        GameObject line = Instantiate(directionLinePrefab, origin, Quaternion.identity);
        LineRenderer lr = line.GetComponent<LineRenderer>();

        // 머티리얼을 Sprites/Default로 지정 (색상 표현을 위해 필요)
        lr.material = new Material(Shader.Find("Sprites/Default"));

        // 선은 두 점으로 구성: 시작점(origin), 끝점(origin + 방향 * 길이)
        lr.positionCount = 2;
        lr.SetPosition(0, origin);
        lr.SetPosition(1, origin + (Vector3)(direction * lineLength));

        // 처음에는 두께 0으로 시작
        lr.startWidth = 0f;
        lr.endWidth = 0f;

        // 색상 설정: 기본은 반투명 빨간색, 필요 시 외부에서 오버라이드
        Color lineColor = colorOverride ?? new Color(1f, 0f, 0f, 0.4f);
        lr.startColor = lr.endColor = lineColor;

        line.transform.SetParent(transform);
        // 두께가 부드럽게 증가하는 코루틴 실행
        StartCoroutine(AnimateLaserWidth(lr, lineWidth, growDuration));

        // 설정된 시간만큼 경고선 유지
        yield return new WaitForSeconds(delay);

        // 경고선 제거
        Destroy(line);

        // 투사체 생성 및 발사 (설정된 경우에만)
        if (fireProjectile && projectilePrefab != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, origin, Quaternion.identity);
            projectile.GetComponent<Projectile>().SetDirection(direction);
        }
    }

    // === 두께를 부드럽게 키우는 애니메이션 코루틴 ===
    private IEnumerator AnimateLaserWidth(LineRenderer lr, float targetWidth, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            float width = Mathf.Lerp(0f, targetWidth, time / duration);
            lr.startWidth = width;
            lr.endWidth = width;

            time += Time.deltaTime;
            yield return null;
        }

        // 마지막에 정확히 고정
        lr.startWidth = targetWidth;
        lr.endWidth = targetWidth;
    }
}
