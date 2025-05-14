using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Boss2Controller : MonoBehaviour
{
    //외부 연결 프리팹 및 설정값
    [SerializeField] private GameObject warningZonePrefab;  // 지점 경고 프리팹
    [SerializeField] private GameObject bugPrefab;          // 벌레 소환용 프리팹
    [SerializeField] private GameObject spawnEffectPrefab;  // 벌레 소환 이팩트
    [SerializeField] private int maxBugCount = 4;           // 동시 존재할 수 있는 벌레 최대 수
    [SerializeField] private float summonInterval = 2f;     // 벌레 소환 주기

    public static int Phase { get; private set; }           // 외부 접근용 페이즈 정보

    //내부 상태 및 컴포넌트
    private StatHandler statHandler;                        // 체력 등 스탯 관리용
    private int phase = 1;                                  // 현재 페이즈

    GameObject _player;                                     // 플레이어 오브젝트
    PlayerController _playerController;                     // 플레이어 컨트롤러 참조
    DieExplosion _die;                                      // 사망 연출 처리용

    DetectPlayer _detectPlayer;                             // 플레이어 감지 컴포넌트

    [Header("Boss Phase Color")]
    [SerializeField] private SpriteRenderer bossRenderer;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color phase2Color = new Color(1f, 0.7f, 0.7f);  // 연한 붉은색
    [SerializeField] private Color phase3Color = new Color(1f, 0.4f, 0.4f);  // 더 진한 붉은색
    [SerializeField] private Color rageColor = Color.red;                   // 분노 상태

    private List<GameObject> summonedBugs = new List<GameObject>(); // 소환한 버그들 리스트

    private void Awake()
    {
        statHandler = GetComponent<StatHandler>();
        _die = GetComponent<DieExplosion>();
        _detectPlayer = GetComponentInChildren<DetectPlayer>();


        EventManager.Instance.RegisterEvent<GameObject>("InitPlayerSpawned", GetPlayerPosition);
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

        if (hpRatio <= 0.8f && phase < 2)
        {
            phase = 2;
            Phase = 2;
            bossRenderer.color = phase2Color;
            ActivatePhase2Patterns();
        }
        if (hpRatio <= 0.65f && phase < 3)
        {
            phase = 3;
            Phase = 3;
            bossRenderer.color = phase3Color;
            ActivatePhase3Patterns();
        }
        if (hpRatio <= 0.5f && phase < 4)
        {
            phase = 4;
            Phase = 4;
            bossRenderer.color = rageColor;
            ActivatePhase4Patterns();
        }
    }


    private IEnumerator BossRoutine()
    {
        while (statHandler.CurrentHP > 0)
        {
            yield return StartCoroutine(DoPattern());
        }

    }


    private IEnumerator DoPattern()
    {
        PerformRandomAttack();
        yield return new WaitForSeconds(2.5f);
    }


    private void PerformRandomAttack()
    {
        Debug.Log("보스2 통상 패턴 실행: Phase " + phase);

        switch (phase)
        {
            case 1: StartCoroutine(MeleeApproachAttack(1f)); break;
            case 2: StartCoroutine(MeleeApproachAttack(1.5f)); break;
            case 3: StartCoroutine(MeleeApproachAttack(1.8f)); break;
            case 4: StartCoroutine(RunAway()); break;
        }
    }

    // 근접 추적 후 공격 (플레이어 감지되었을 때만)
    private IEnumerator MeleeApproachAttack(float speedMultiplier = 1f)
    {
        if (!_detectPlayer.detect) yield break;

        Vector3 targetPos = _player.transform.position;
        float speed = statHandler.MoveSpeed * speedMultiplier;
        float duration = 1.0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            elapsed += Time.deltaTime;

            // 이동 중 충돌 체크
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1.2f);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    _playerController.TakeDamaged();
                    Debug.Log("플레이어 충돌 시 즉시 데미지!");
                    yield break; // 데미지 주고 즉시 종료
                }
            }

            yield return null;
        }

        yield return new WaitForSeconds(0.5f); // 도달 실패 시 약간의 후딜
    }

    // 플레이어로부터 도망가는 행동, 실패 시 반격
    private IEnumerator RunAway()
    {
        Debug.Log("보스 도망 시작");

        Vector2 awayDirection = (transform.position - _player.transform.position).normalized;
        Vector3 targetPos = transform.position + (Vector3)awayDirection * 5f;
        Vector3 lastPos = transform.position;

        float duration = 2f;
        float time = 0f;
        float moveSpeed = statHandler.MoveSpeed * 2;

        while (time < duration)
        {
            time += Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // 이동 거리 짧거나 벽에 막히면
        float movedDist = Vector3.Distance(transform.position, lastPos);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, awayDirection, 0.5f, LayerMask.GetMask("Ground"));

        if (movedDist < 1f || hit.collider != null)
        {
            int failChoice = Random.Range(0, 2); // 0 = 반격, 1 = 회피

            if (failChoice == 0)
            {
                Debug.Log("도망 실패 → 근접 반격 선택");
                yield return StartCoroutine(MeleeApproachAttack(3f));
            }
            else
            {
                Debug.Log("도망 실패 → 빠른 회피 선택");
                yield return StartCoroutine(QuickEvade());
            }
        }

        yield return new WaitForSeconds(0.5f);
    }
    // 도망 실패 시 빠르게 회피하는 행동
    private IEnumerator QuickEvade()
    {
        Debug.Log("도망 실패 → 플레이어 방향 ±90도 회피");

        float dashSpeed = statHandler.MoveSpeed * 6f;
        float duration = 0.5f;

        Vector2 playerDir = (_player.transform.position - transform.position).normalized;
        Vector2 evadeDir = Vector2.zero;

        int attempt = 0;
        const int maxAttempts = 20;

        while (attempt < maxAttempts)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            float angle = Vector2.Angle(playerDir, randomDir);

            if (angle <= 90f) // 플레이어를 향해 ±90도 범위
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, randomDir, 0.5f, LayerMask.GetMask("Ground"));
                if (hit.collider == null)
                {
                    evadeDir = randomDir;
                    break;
                }
            }

            attempt++;
        }

        if (evadeDir == Vector2.zero)
        {
            evadeDir = playerDir;
            Debug.LogWarning("회피 방향 실패 → 플레이어 방향으로 강제 이동");
        }

        Debug.Log($"최종 회피 방향(±90도): {evadeDir}");

        float time = 0f;
        while (time < duration)
        {
            transform.position += (Vector3)(evadeDir * dashSpeed * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);
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
        Vector2 offset = Random.insideUnitCircle.normalized * 3f;
        Vector3 spawnPos = transform.position + (Vector3)offset;
        spawnPos.z = -2f;

        if (spawnEffectPrefab != null)
        {
            GameObject effect = Instantiate(spawnEffectPrefab, spawnPos, Quaternion.identity);
            effect.transform.localScale = Vector3.one * 0.3f;
            Destroy(effect, 1.5f); // 일정 시간 후 삭제
        }

        GameObject bug = Instantiate(bugPrefab, spawnPos, Quaternion.identity);
        summonedBugs.Add(bug); // 소환된 벌레 리스트에 추가

        Debug.Log($"벌레 소환 위치: {spawnPos}");

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
        summonInterval = 0.1f;   // 소환 간격 짧게
        maxBugCount = 16;        // 최대 소환 수 증가
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
        foreach (GameObject bug in summonedBugs)
        {
            if (bug != null)
                Destroy(bug);
        }
        summonedBugs.Clear();

        _die.ExecuteDeathSequence();
        // 소환된 벌레 전부 제거
       

    }

    // 투사체 충돌 처리
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 15 || collision.gameObject.layer == 16)
        {
            statHandler.TakeDamage(collision.gameObject.layer == 15 ? _playerController.GetPower() : _playerController.GetPower() * 3);
            SoundManager.PlayClip("BossDamageSound");

            if (statHandler.CurrentHP <= 0)
            {
                StopAllCoroutines();
                OnDeath();
            }
            if (!_playerController.IsSniper() || collision.gameObject.layer == 16)
            {
                Destroy(collision.gameObject);
            }
        }
    }

    private void OnDisable()
    {
        EventManager.Instance.UnregisterEvent<GameObject>("InitPlayerSpawned", GetPlayerPosition);
    }
}
