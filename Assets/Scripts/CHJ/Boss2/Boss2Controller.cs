using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Boss2Controller : MonoBehaviour
{
    //외부 연결 프리팹 및 설정값
    [SerializeField] private GameObject warningZonePrefab;  // 지점 경고 프리팹
    [SerializeField] private GameObject bugPrefab;          // 벌레 소환용 프리팹
    [SerializeField] private int maxBugCount = 8;           // 동시 존재할 수 있는 벌레 최대 수
    [SerializeField] private float summonInterval = 4f;     // 벌레 소환 주기
    [SerializeField] private Tilemap groundTilemap;         // 벌레가 소환될 수 있는 타일맵

    public static int Phase { get; private set; }           // 외부 접근용 페이즈 정보

    //내부 상태 및 컴포넌트
    private StatHandler statHandler;                        // 체력 등 스탯 관리용
    private int phase = 1;                                  // 현재 페이즈

    GameObject _player;                                     // 플레이어 오브젝트
    PlayerController _playerController;                     // 플레이어 컨트롤러 참조
    DieExplosion _die;                                      // 사망 연출 처리용

    DetectPlayer _detectPlayer;                             // 플레이어 감지 컴포넌트

    private void Awake()
    {
        statHandler = GetComponent<StatHandler>();
        _die = GetComponent<DieExplosion>();
        _detectPlayer = GetComponentInChildren<DetectPlayer>();


        EventManager.Instance.RegisterEvent<GameObject>("GetPlayerPosition", GetPlayerPosition);
    }

    private void Start()
    {

        if (_player != null)
        {
            _playerController = _player.GetComponent<PlayerController>();
            StartCoroutine(BossRoutine());
        }
    }

    private void Update()
    {
        if (_player != null)
        {
            CheckPhase();
        }
    }

    // 페이즈 전환
    private void CheckPhase()
    {
        float hpRatio = statHandler.CurrentHP / statHandler.MaxHP;

        if (hpRatio <= 0.75f && phase < 2)
        {
            phase = 2;
            Phase = 2;
            ActivatePhase2Patterns();
        }
        if (hpRatio <= 0.5f && phase < 3)
        {
            phase = 3;
            Phase = 3;
            ActivatePhase3Patterns();
        }
        if (hpRatio <= 0.25f && phase < 4)
        {
            phase = 4;
            Phase = 4;
            ActivatePhase4Patterns();
        }
    }


    private IEnumerator BossRoutine()
    {
        while (statHandler.CurrentHP > 0)
        {
            yield return StartCoroutine(DoPattern());
        }

        OnDeath();
    }


    private IEnumerator DoPattern()
    {
        PerformRandomAttack();
        yield return new WaitForSeconds(2.5f);
    }


    private void PerformRandomAttack()
    {
        int pattern = Random.Range(0, 6);
        Debug.Log("보스2 통상 패턴 실행: " + pattern);

        switch (pattern)
        {
            case 0: StartCoroutine(MeleeApproachAttack()); break; // 근접 공격
            case 1:
            case 2:
            case 3:
            case 4:
            case 5: StartCoroutine(RunAway()); break; // 도망 (조건부 반격)
        }
    }

    // 근접 추적 후 공격 (플레이어 감지되었을 때만)
    private IEnumerator MeleeApproachAttack()
    {
        if (!_detectPlayer.detect) yield break;

        Vector3 targetPos = _player.transform.position;
        float speed = statHandler.MoveSpeed;
        float duration = 1.0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 범위 내 플레이어 데미지
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1.2f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                _playerController.TakeDamaged();
            }
        }

        yield return new WaitForSeconds(0.5f);
    }

    // 플레이어로부터 도망가는 행동, 실패 시 반격
    private IEnumerator RunAway()
    {
        Debug.Log("보스 도망 시작");

        Vector2 awayDirection = (transform.position - _player.transform.position).normalized;
        Vector3 targetPos = transform.position + (Vector3)awayDirection * 5f;
        Vector3 lastPos = transform.position;

        float duration = 1f;
        float time = 0f;
        float moveSpeed = statHandler.MoveSpeed;

        while (time < duration)
        {
            time += Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // 이동 거리 짧거나 벽에 막히면 근접 반격
        float movedDist = Vector3.Distance(transform.position, lastPos);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, awayDirection, 0.5f, LayerMask.GetMask("Ground"));

        if (movedDist < 0.1f || hit.collider != null)
        {
            Debug.Log("도망 실패 → 근접 반격 전환");
            yield return StartCoroutine(MeleeApproachAttack());
        }

        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator RepeatBugSummon()
    {
        while (phase >= 2)
        {
            GameObject[] bugs = GameObject.FindGameObjectsWithTag("Bug");

            if (bugs.Length < maxBugCount)
            {
                yield return StartCoroutine(SummonBug());
            }

            yield return new WaitForSeconds(summonInterval);
        }
    }

    private IEnumerator SummonBug()
    {
        Vector3 spawnPos = Vector3.zero;
        bool found = false;

        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector2 offset = Random.insideUnitCircle.normalized * 3f;
            Vector3 candidate = transform.position + (Vector3)offset;
            Vector3Int tilePos = groundTilemap.WorldToCell(candidate);

            if (groundTilemap.HasTile(tilePos))
            {
                spawnPos = candidate;
                found = true;
                break;
            }
        }

        if (found)
        {
            Instantiate(bugPrefab, spawnPos, Quaternion.identity);
            Debug.Log($"벌레 소환 위치: {spawnPos}");
        }
        else
        {
            Debug.LogWarning("타일 위에서 벌레 소환 실패");
        }

        yield return null;
    }

    // 페이즈별 패턴 활성화
    private void ActivatePhase2Patterns()
    {
        Debug.Log("Phase 2: 벌레 소환 루프 시작");
        StartCoroutine(RepeatBugSummon());
    }

    private void ActivatePhase3Patterns()
    {
        Debug.Log("Phase 3: 벌레 자폭 시작");
    }

    private void ActivatePhase4Patterns()
    {
        Debug.Log("Phase 4: 소환 강화 시작");
        summonInterval = 0.5f;   // 소환 간격 짧게
        maxBugCount = 10;        // 최대 소환 수 증가
    }

    // 유틸리티

    // 이벤트 통해 플레이어 정보 수신
    private void GetPlayerPosition(GameObject player)
    {
        _player = player;
        _playerController = _player.GetComponent<PlayerController>();
        StartCoroutine(BossRoutine());
    }

    // 보스 사망 처리
    private void OnDeath()
    {
        Debug.Log("보스2 사망 처리");
        _die.ExecuteDeathSequence();
    }

    // 투사체 충돌 처리
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 15)
        {
            statHandler.TakeDamage(_playerController.GetPower());
            Destroy(collision.gameObject);
        }
    }
}
