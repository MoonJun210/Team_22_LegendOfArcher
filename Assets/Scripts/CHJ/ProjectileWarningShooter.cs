
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWarningShooter : MonoBehaviour
{
    [SerializeField] private GameObject directionLinePrefab;
    [SerializeField] private GameObject projectilePrefab;

    // 외부에서 호출되는 투사체 경고 및 발사 함수
    public IEnumerator Fire(
        Vector3 origin,                           // 시작 위치
        Vector2 direction,                        // 발사 방향
        float delay = 1f,                         // 경고선 유지 시간
        float lineLength = 20f,                   // 경고선 길이
        float lineWidth = 0.1f,                   // 최종 경고선 두께
        Color? colorOverride = null,              // 경고선 색상 지정 (없으면 기본)
        bool fireProjectile = true,               // 경고 후 투사체 발사 여부
        float growDuration = 0.2f                 // 경고선 굵어지는 데 걸리는 시간
    )
    {
        // 경고선 오브젝트 생성
        GameObject line = Instantiate(directionLinePrefab, origin, Quaternion.identity);
        LineRenderer lr = line.GetComponent<LineRenderer>();

        // 경고선에 사용할 기본 머티리얼 설정 (색 표현 가능하도록)
        lr.material = new Material(Shader.Find("Sprites/Default"));

        // 시작점과 끝점 설정 (두 점으로 구성)
        lr.positionCount = 2;
        lr.SetPosition(0, origin);
        lr.SetPosition(1, origin + (Vector3)(direction * lineLength));

        // 초기 두께는 0으로 설정
        lr.startWidth = 0f;
        lr.endWidth = 0f;

        // 색상 설정: 기본은 반투명 빨간색, 아니면 오버라이드
        Color lineColor = colorOverride ?? new Color(1f, 0f, 0f, 0.4f);
        lr.startColor = lr.endColor = lineColor;

        // 부모 설정 (정리용)
        line.transform.SetParent(transform);

        // 경고선 두께 애니메이션 실행
        StartCoroutine(AnimateLaserWidth(lr, lineWidth, growDuration));

        // 경고선 유지 시간만큼 대기
        yield return new WaitForSeconds(delay);

        // 경고선 제거
        Destroy(line);

        // 투사체 발사 (설정된 경우에만)
        if (fireProjectile && projectilePrefab != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, origin, Quaternion.identity);
            projectile.GetComponent<Projectile>().SetDirection(direction);
        }
    }


    // 경고선의 두께를 부드럽게 증가시키는 애니메이션
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

        // 종료 시 두께 고정
        lr.startWidth = targetWidth;
        lr.endWidth = targetWidth;
    }
}
