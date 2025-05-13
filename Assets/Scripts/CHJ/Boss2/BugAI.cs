using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;



public class BugAI : MonoBehaviour
{
    private enum State { Wander, Charge, Stop }
    private State currentState = State.Wander;

    //기본 설정
    [SerializeField] private float moveSpeed = 2f;                // 평상시 이동 속도
    [SerializeField] private float chargeSpeed = 6f;              // 돌진 속도
    [SerializeField] private float directionChangeInterval = 1.5f;// 이동 방향 변경 간격
    [SerializeField] private float maxDistanceFromBoss = 6f;      // 보스로부터의 최대 거리 제한
    [SerializeField] private ProjectileWarningShooter shooter;    // 경고선 표시용
    [SerializeField] private float warningDelay = 1.2f;           // 경고 후 자폭 딜레이
    [SerializeField] private GameObject warningSign_Circle;       // 자폭 경고 시각 효과 프리팹

    //내부 상태
    private Rigidbody2D _rigidbody;
    private Vector2 moveDirection;                                // 현재 이동 방향
    private float directionTimer;                                 // 방향 변경 타이머

    private Transform bossTransform;                              // 소환 보스 위치
    private GameObject player;                                    // 플레이어 참조

    private Vector2 targetPosition;                               // 돌진 목표 위치
    private Vector2 chargeDirection;                              // 돌진 방향
    private bool isCharging = false;                              // 돌진 중 여부

    DetectPlayer _detectPlayer;                                   // 플레이어 감지

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _detectPlayer = GetComponentInChildren<DetectPlayer>();
    }

    private void Start()
    {
        // 보스 및 플레이어 참조 찾기
        GameObject boss = GameObject.FindWithTag("Boss");
        player = GameObject.FindGameObjectWithTag("Player");

        if (boss != null)
            bossTransform = boss.transform;

        shooter = GetComponent<ProjectileWarningShooter>();

        PickRandomDirection();
        directionTimer = directionChangeInterval;
    }

    private void Update()
    {
        if (currentState == State.Wander)
        {
            directionTimer -= Time.deltaTime;

            if (bossTransform == null)
                return;

            if (IsInsideBossRange())
            {
                // 일정 주기로 방향 변경
                if (directionTimer <= 0f)
                {
                    PickRandomDirection();
                    directionTimer = directionChangeInterval;
                }

                _rigidbody.velocity = moveDirection * moveSpeed;
            }
            else
            {
                // 보스 영역 이탈 시 복귀
                Vector2 returnDir = (bossTransform.position - transform.position).normalized;
                _rigidbody.velocity = returnDir * (moveSpeed * 0.5f);
            }

            TryAutoCharge(); // 조건 만족 시 자폭 돌진
        }
        else if (currentState == State.Charge)
        {
            _rigidbody.velocity = chargeDirection * chargeSpeed;

            // 돌진 목표에 도달하면 멈추고 자폭
            if (Vector2.Distance(transform.position, targetPosition) < 0.2f)
            {
                _rigidbody.velocity = Vector2.zero;
                currentState = State.Stop;
                isCharging = false;

                StartCoroutine(Explode());
            }
        }
    }

    // 페이즈 3 이상 + 감지되면 돌진 시도
    private void TryAutoCharge()
    {
        if (Boss2Controller.Phase >= 3 && !isCharging && _detectPlayer.detect)
        {
            StartCharge();
        }
    }

    // 돌진 시작 (외부에서도 호출 가능)
    public void StartCharge()
    {
        if (isCharging || player == null || shooter == null)
            return;

        isCharging = true;
        currentState = State.Stop;
        _rigidbody.velocity = Vector2.zero;

        targetPosition = player.transform.position;
        chargeDirection = (targetPosition - (Vector2)transform.position).normalized;

        StartCoroutine(ChargeWithWarning());            // 경고선 연출 후 돌진
        StartCoroutine(SelfDestructTimer(3f));          // 실패 시 3초 후 강제 자폭
    }

    // 돌진 후 일정 시간 내 자폭하지 않으면 강제 자폭
    private IEnumerator SelfDestructTimer(float time)
    {
        yield return new WaitForSeconds(time);

        if (this != null && gameObject.activeInHierarchy)
        {
            Debug.Log("시간 초과로 강제 자폭");
            _rigidbody.velocity = Vector2.zero;
            StartCoroutine(Explode());
        }
    }

    // 랜덤 방향 선택
    private void PickRandomDirection()
    {
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        moveDirection = new Vector2(x, y).normalized;
    }

    // 보스 범위 내에 있는지 판단
    private bool IsInsideBossRange()
    {
        return bossTransform != null &&
               Vector2.Distance(transform.position, bossTransform.position) <= maxDistanceFromBoss;
    }

    // 플레이어 공격에 맞으면 파괴
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 15)
        {
            Destroy(collision.gameObject);
            Destroy(this.gameObject);
        }
    }

    // 경고선 시각 후 돌진 시작
    private IEnumerator ChargeWithWarning()
    {
        StartCoroutine(shooter.Fire(
            transform.position,
            chargeDirection,
            delay: 1f,
            lineLength: 20f,
            lineWidth: 0.5f,
            fireProjectile: false
        ));

        yield return new WaitForSeconds(1f);
        currentState = State.Charge;
    }

    // 자폭 연출 및 범위 데미지 처리
    private IEnumerator Explode()
    {
        // 경고 이펙트 표시
        if (warningSign_Circle != null)
        {
            GameObject warning = Instantiate(warningSign_Circle, transform.position + Vector3.forward * -1, Quaternion.identity);
            warning.transform.localScale = Vector3.one * 3f;
            warning.transform.SetParent(transform);
            Destroy(warning, 1.2f);
        }

        // 잠깐 연출 시간 대기
        yield return new WaitForSeconds(1f);

        // 범위 내 플레이어 데미지
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 3f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<PlayerController>()?.TakeDamaged();
            }
        }

        Destroy(gameObject); // 자폭 후 제거
    }
}
