using UnityEngine;

public class Boss_1 : BaseController
{
    private GameObject _player;
    private PlayerController _playerController;

    [SerializeField] private float moveSpeed;

    [SerializeField] private GameObject warningSign_Circle;
    [SerializeField] private GameObject warningSign_Square;

    [SerializeField] private bool detectPlayer;
    [SerializeField] private float moveCooltime;

    [SerializeField] private bool isPattern;
    [SerializeField] private float patternTime;

    [SerializeField] private bool pattern_A;
    [SerializeField] private float pattern_A_Cooltime;

    [SerializeField] private bool pattern_B;
    [SerializeField] private float pattern_B_Cooltime;
    protected override void Awake()
    {
        base.Awake();
        //player = GameObject.Find("Player"); << 플레이어 찾기
        EventManager.Instance.RegisterEvent<GameObject>("InitPlayerSpawned", InitPlayerSpawned);
    }
    protected override void Update()
    {
        if (!detectPlayer)
        {
            // 플레이어를 감지하지 못하는 경우 어슬렁거리는 기능
            Move_NotNearPlayer();
        }
        else
        {
            // 플레이어를 감지했을 경우 각종 움직이거나 하는 함수 출력
            // 특정 패턴이 진행 중일때 무분별한 행동 함수 호출를 막기 위한 조건문
            if (!isPattern)
            {
                RandomPattern();
                Move_NearPlayer();
            }
        }
        Pattern_A();
        Pattern_B();
        if (moveCooltime > 0)
            moveCooltime -= Time.deltaTime;
        else
            moveCooltime = 0;

        lookDirection = movementDirection;
        base.Update();
    }
    protected override void Movment(Vector2 direction)
    {
        direction = direction * moveSpeed; // 속도 조절은 여기서

        _rigidbody.velocity = direction;
    }
    protected override void Rotate(Vector2 direction)
    {
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bool isLeft = Mathf.Abs(rotZ) > 90f;

        if (direction.x != 0)
            characterRenderer.flipX = isLeft;

    }

    private void RandomPattern()
    {
        int randomInt = Random.Range(0, 5);
        switch (randomInt)
        {
            case 0:
                if (pattern_A_Cooltime == 0)
                {
                    // 불리언 변수를 참으로 함으로써 패턴 A 함수 호출
                    isPattern = true;
                    pattern_A = true;
                    Debug.Log("패턴 A 실행");
                    pattern_A_Cooltime = 8;
                }
                break;
            case 1:
                if (pattern_B_Cooltime == 0)
                {
                    isPattern = true;
                    pattern_B = true;
                    Debug.Log("패턴 B 실행");
                    pattern_B_Cooltime = 12;
                }
                break;
        }
    }
    private void Move_NotNearPlayer()
    {
        Vector2 randomDirection = movementDirection;
        if (moveCooltime == 0)
        {
            randomDirection.x = (int)Random.Range(-1, 2);
            randomDirection.y = (int)Random.Range(-1, 2);
            moveCooltime = 2f;
        }
        Debug.Log(randomDirection);
        movementDirection = randomDirection;
    }
    private void Move_NearPlayer()
    {
        Vector2 moveVec;
        if (Mathf.Abs(_player.transform.position.x - transform.position.x) > 1)
            moveVec.x = _player.transform.position.x - transform.position.x;
        else
            moveVec.x = 0;
        if (Mathf.Abs(_player.transform.position.y - transform.position.y) > 1)
            moveVec.y = _player.transform.position.y - transform.position.y;
        else
            moveVec.y = 0;

        movementDirection = moveVec.normalized;
    }
    private void Pattern_A()
    {
        if (pattern_A)
        {
            movementDirection = Vector2.zero;
            if (patternTime == 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 playerNearVec = new Vector2(_player.transform.position.x + Random.Range(-1f, 2f), _player.transform.position.y + Random.Range(-1f, 2f));
                    GameObject warning = Instantiate(warningSign_Circle, playerNearVec, transform.rotation);
                    Vector2 sizevec = new Vector2(3, 3);
                    warning.GetComponent<WarningSign>().SetSizeVec(sizevec);
                    warning.GetComponent<WarningSign>().SetWarning_Destroy_Time(Random.Range(1.5f, 2.0f), 0.2f);
                }
            }
            patternTime += Time.deltaTime;
            if (patternTime > 3)
            {
                pattern_A = false;
                isPattern = false;
                patternTime = 0;
            }

        }
        if (pattern_A_Cooltime > 0)
            pattern_A_Cooltime -= Time.deltaTime;
        else
            pattern_A_Cooltime = 0;
    }
    private void Pattern_B()
    {
        if (pattern_B)
        {
            movementDirection = Vector2.zero;
            patternTime += Time.deltaTime;
            if (patternTime > 1 && patternTime < 2)
            {
                patternTime = 2;
                transform.position = _player.transform.position;
                GameObject warning = Instantiate(warningSign_Circle, transform.position, transform.rotation);
                Vector2 sizevec = new Vector2(8, 8);
                warning.GetComponent<WarningSign>().SetSizeVec(sizevec);
                warning.GetComponent<WarningSign>().SetWarning_Destroy_Time(3f, 0.2f);
            }
            if (patternTime > 6)
            {
                pattern_B = false;
                isPattern = false;
                patternTime = 0;
            }
        }
        if (pattern_B_Cooltime > 0)
            pattern_B_Cooltime -= Time.deltaTime;
        else
            pattern_B_Cooltime = 0;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == _player)
        {
            detectPlayer = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == _player)
        {
            detectPlayer = false;
        }
    }

    void InitPlayerSpawned(GameObject player)
    {
        _player = player;
        _playerController = _player.GetComponent<PlayerController>();
    }
}
