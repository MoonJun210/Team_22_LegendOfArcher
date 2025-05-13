using UnityEngine;

public class Boss_1 : BaseController
{
    [SerializeField] private GameObject _player;
    [SerializeField] private BossTriggerColider triggerColider;
    private PlayerController _playerController;

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

    [SerializeField] private bool pattern_C;
    [SerializeField] private float pattern_C_Cooltime;

    [SerializeField] private bool pattern_D;
    [SerializeField] private float pattern_D_Cooltime;

    [SerializeField] private bool pattern_E;
    [SerializeField] private float pattern_E_Cooltime;

    StatHandler _statHandler;
    DieExplosion _diceExplosion;
    private bool isDead;
    protected override void Awake()
    {
        base.Awake();
        //player = GameObject.Find("Player"); << 플레이어 찾기
        EventManager.Instance.RegisterEvent<GameObject>("InitPlayerSpawned", InitPlayerSpawned);
        triggerColider = GetComponentInChildren<BossTriggerColider>();
        _statHandler = GetComponent<StatHandler>();
        _diceExplosion = GetComponent<DieExplosion>();
    }
    protected override void Update()
    {
        if (isDead) return;
        if (_player != null)
        {
            if (!detectPlayer)
            {
                // 플레이어를 감지하지 못하는 경우 어슬렁거리는 기능
                Move_NotNearPlayer();
                // 감지를 못해도 패턴 쿨타임이 차면 패턴 실행
                if (!isPattern)
                    RandomPattern();
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
        }
        Pattern_A();
        Pattern_B();
        Pattern_C();
        Pattern_D();
        detectPlayer = triggerColider.GetDetectPlayer();
        if (moveCooltime > 0)
            moveCooltime -= Time.deltaTime;
        else
            moveCooltime = 0;

        lookDirection = movementDirection;
        base.Update();
    }
    protected override void Movment(Vector2 direction)
    {
        direction = direction * _statHandler.MoveSpeed; // 속도 조절은 여기서

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
        int randomInt = Random.Range(0, 4);
        switch (randomInt)
        {
            case 0:
                if (pattern_A_Cooltime == 0)
                {
                    isPattern = true;
                    pattern_A = true;
                    pattern_A_Cooltime = 10;
                }
                break;
            case 1:
                if (pattern_B_Cooltime == 0)
                {
                    isPattern = true;
                    pattern_B = true;
                    pattern_B_Cooltime = 20;
                }
                break;
            case 2:
                if (pattern_C_Cooltime == 0)
                {
                    isPattern = true;
                    pattern_C = true;
                    pattern_C_Cooltime = 30;
                }
                break;
            case 3:
                if (pattern_D_Cooltime == 0)
                {
                    isPattern = true;
                    pattern_D = true;
                    pattern_D_Cooltime = 30;
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
            Move_NotNearPlayer();
            if (patternTime == 0)
            {
                _rigidbody.MovePosition(new Vector2(Random.Range(-5, 6), Random.Range(7.5f, 15f))); // 맵 어딘가로 랜덤 이동
                for (int i = 0; i < 5; i++)
                {
                    Vector2 playerNearVec = new Vector2(_player.transform.position.x + Random.Range(-2f, 3f), _player.transform.position.y + Random.Range(-2f, 3f));
                    GameObject warning = Instantiate(warningSign_Circle, playerNearVec, transform.rotation);
                    Vector2 sizevec = new Vector2(3, 3);
                    warning.GetComponent<WarningSign>().SetSizeVec(sizevec);
                    warning.GetComponent<WarningSign>().SetWarning_Destroy_Time(Random.Range(1.5f, 2.0f), 0.2f);
                    warning.GetComponent<WarningSign>().SetPlayer(_player, _playerController);
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
                _rigidbody.MovePosition(_player.transform.position);
                GameObject warning = Instantiate(warningSign_Circle, _player.transform.position, transform.rotation);
                Vector2 sizevec = new Vector2(8, 8);
                warning.GetComponent<WarningSign>().SetSizeVec(sizevec);
                warning.GetComponent<WarningSign>().SetWarning_Destroy_Time(2f, 0.2f);
                warning.GetComponent<WarningSign>().SetPlayer(_player, _playerController);
            }
            if (patternTime > 5)
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
    private void Pattern_C()
    {
        if (pattern_C)
        {
            movementDirection = Vector2.zero;
            _rigidbody.MovePosition(new Vector2(0, 11)); // 맵 중앙 이동
            patternTime += Time.deltaTime;
            if (patternTime > 0.5 && patternTime < 1)
            {
                patternTime = 1;
                for (int i = 0; i < 2; i++)
                {
                    Vector2 wsVec = Vector2.zero;
                    switch (i)
                    {
                        case 0: wsVec = new Vector2(-4, 11.5f); break;
                        case 1: wsVec = new Vector2(4, 11.5f); break;
                    }
                    GameObject warning = Instantiate(warningSign_Square, wsVec, transform.rotation);
                    Vector2 sizevec = new Vector2(6, 11);
                    warning.GetComponent<WarningSign>().SetSquare_Vertical();
                    warning.GetComponent<WarningSign>().SetSizeVec(sizevec);
                    warning.GetComponent<WarningSign>().SetWarning_Destroy_Time(2f, 0.2f);
                    warning.GetComponent<WarningSign>().SetPlayer(_player, _playerController);
                }
            }
            if (patternTime > 3.5)
            {
                pattern_C = false;
                isPattern = false;
                patternTime = 0;
            }
        }
        if (pattern_C_Cooltime > 0)
            pattern_C_Cooltime -= Time.deltaTime;
        else
            pattern_C_Cooltime = 0;
    }
    private void Pattern_D()
    {
        if (pattern_D)
        {
            movementDirection = Vector2.zero;
            _rigidbody.MovePosition(new Vector2(0, 11)); // 맵 중앙 이동
            patternTime += Time.deltaTime;
            if (patternTime > 0.5 && patternTime < 1)
            {
                patternTime = 1;
                for (int i = 0; i < 2; i++)
                {
                    Vector2 wsVec = Vector2.zero;
                    switch (i)
                    {
                        case 0: wsVec = new Vector2(0, 14.5f); break;
                        case 1: wsVec = new Vector2(0, 8.0f); break;
                    }
                    GameObject warning = Instantiate(warningSign_Square, wsVec, transform.rotation);
                    Vector2 sizevec = new Vector2(14, 5);
                    warning.GetComponent<WarningSign>().SetSquare_Vertical();
                    warning.GetComponent<WarningSign>().SetSizeVec(sizevec);
                    warning.GetComponent<WarningSign>().SetWarning_Destroy_Time(2f, 0.2f);
                    warning.GetComponent<WarningSign>().SetPlayer(_player, _playerController);
                }
            }
            if (patternTime > 3.5)
            {
                pattern_D = false;
                isPattern = false;
                patternTime = 0;
            }
        }
        if (pattern_D_Cooltime > 0)
            pattern_D_Cooltime -= Time.deltaTime;
        else
            pattern_D_Cooltime = 0;
    }

    void InitPlayerSpawned(GameObject player)
    {
        _player = player;
        _playerController = _player.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 15)
        {
            _statHandler.TakeDamage(_playerController.GetPower());
            Destroy(collision.gameObject);
            // 죽음 처리
            if(_statHandler.CurrentHP <= 0 && !isDead)
            {
                _diceExplosion.ExecuteDeathSequence();
                isDead = true;
                movementDirection = Vector2.zero;
            }
        }
    }
}
