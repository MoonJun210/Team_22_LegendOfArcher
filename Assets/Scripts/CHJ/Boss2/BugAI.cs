using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;



public class BugAI : MonoBehaviour
{
    private enum State { Wander, Charge, Stop }
    private State currentState = State.Wander;

    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float chargeSpeed = 6f;
    [SerializeField] private float directionChangeInterval = 1.5f;
    [SerializeField] private float maxDistanceFromBoss = 6f;
    [SerializeField] private ProjectileWarningShooter shooter;
    [SerializeField] private float warningDelay = 1.2f;
    [SerializeField] private GameObject warningSign_Circle;

    private Rigidbody2D _rigidbody;
    private Vector2 moveDirection;
    private float directionTimer;

    private Transform bossTransform;
    private GameObject player;

    private Vector2 targetPosition;
    private Vector2 chargeDirection;
    private bool isCharging = false;

    DetectPlayer _detectPlayer;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _detectPlayer = GetComponentInChildren<DetectPlayer>();
    }

    private void Start()
    {
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
                if (directionTimer <= 0f)
                {
                    PickRandomDirection();
                    directionTimer = directionChangeInterval;
                }

                _rigidbody.velocity = moveDirection * moveSpeed;
            }
            else
            {
                Vector2 returnDir = (bossTransform.position - transform.position).normalized;
                _rigidbody.velocity = returnDir * (moveSpeed * 0.5f);
            }

            TryAutoCharge();
        }
        else if (currentState == State.Charge)
        {
            _rigidbody.velocity = chargeDirection * chargeSpeed;

            if (Vector2.Distance(transform.position, targetPosition) < 0.2f)
            {
                _rigidbody.velocity = Vector2.zero;
                currentState = State.Stop;
                isCharging = false;

                StartCoroutine(Explode());
            }
        }
    }

    private void TryAutoCharge()
    {
        if (Boss2Controller.Phase >= 3 && !isCharging && _detectPlayer.detect)
        {
            StartCharge();
        }
    }

    public void StartCharge()
    {
        if (isCharging || player == null || shooter == null)
            return;

        isCharging = true;
        currentState = State.Stop;
        _rigidbody.velocity = Vector2.zero;

        targetPosition = player.transform.position;
        chargeDirection = (targetPosition - (Vector2)transform.position).normalized;

        StartCoroutine(ChargeWithWarning());
        StartCoroutine(SelfDestructTimer(3f));
    }
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

    private void PickRandomDirection()
    {
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        moveDirection = new Vector2(x, y).normalized;
    }

    private bool IsInsideBossRange()
    {
        return bossTransform != null &&
               Vector2.Distance(transform.position, bossTransform.position) <= maxDistanceFromBoss;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
      
        if (collision.gameObject.layer == 15)
        {
            Destroy(collision.gameObject);
            Destroy(this.gameObject);
        }
    }


    private IEnumerator ChargeWithWarning()
    {
        // 경고선 표시
        StartCoroutine(shooter.Fire(
    transform.position,
    chargeDirection,
    delay: 1f,
    lineLength: 20f,
    lineWidth: 0.5f,
    fireProjectile: false
));

        // 1초 후 돌진
        yield return new WaitForSeconds(1f);
        currentState = State.Charge;
    }
    private IEnumerator Explode()
    {
        // 자폭 경고존 시각적으로 생성
        if (warningSign_Circle != null)
        {
            GameObject warning = Instantiate(warningSign_Circle, transform.position + Vector3.forward * -1, Quaternion.identity);
            warning.transform.localScale = Vector3.one * 3f;
            warning.transform.SetParent(transform);
            Destroy(warning, 1.2f);
        }

        // 자폭까지 잠깐 기다리는 연출용 대기
        yield return new WaitForSeconds(1f);

        // 실제 자폭 범위 처리
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 3f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<PlayerController>()?.TakeDamaged();
            }
        }

        Destroy(gameObject);
    }
}
