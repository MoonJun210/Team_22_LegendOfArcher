
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWarningShooter : MonoBehaviour
{
    [SerializeField] private GameObject directionLinePrefab;
    [SerializeField] private GameObject projectilePrefab;

    public IEnumerator Fire(
        Vector3 origin,                 // 투사체와 경고선이 시작될 위치
        Vector2 direction,              // 발사 방향 (단위 벡터)
        float delay = 1f,               // 경고선이 유지되는 시간 (초)
        float lineLength = 20f,         // 경고선의 길이
        float lineWidth = 0.1f,         // 경고선의 두께
        Color? colorOverride = null,    // 선 색상 오버라이드 (null이면 기본 색상 사용)
        bool fireProjectile = true      // 투사체 발사 여부 (false면 경고선만 표시)
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

        // 선의 굵기 설정
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;

        // 색상 설정: 기본은 반투명 빨간색, 필요 시 외부에서 오버라이드
        Color lineColor = colorOverride ?? new Color(1f, 0f, 0f, 0.4f); // 기본: 경고선용 반투명 빨강
        lr.startColor = lr.endColor = lineColor;

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
}
