using UnityEngine;

public class Boss_3 : BaseController
{
    [SerializeField] private GameObject _player;
    [SerializeField] private BossTriggerColider triggerColider;
    [SerializeField] private GameObject flashPtc;
    private PlayerController _playerController;

    [SerializeField] private float moveSpeed;

    [SerializeField] private GameObject warningSign_Circle;
    [SerializeField] private GameObject warningSign_Square;

    [SerializeField] private bool detectPlayer;
    [SerializeField] private float moveCooltime;

    [SerializeField] private bool isPattern;
    [SerializeField] private float patternTime;
    [SerializeField] private float patternCycleSec;// 반복적인 패턴 사용할 때 쓰는 주기 변수 (초 단위)

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
    Animator _animator;
    private bool isDead;
    protected override void Awake()
    {
        base.Awake();
        //player = GameObject.Find("Player"); << 플레이어 찾기
        EventManager.Instance.RegisterEvent<GameObject>("InitPlayerSpawned", InitPlayerSpawned);
        triggerColider = GetComponentInChildren<BossTriggerColider>();
        _statHandler = GetComponent<StatHandler>();
        _diceExplosion = GetComponent<DieExplosion>();
        _animator = GetComponent<Animator>();
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
    private void LateUpdate()
    {
        if (!isDead)
        {
            _animator.SetBool("isDead", false);
            if (_rigidbody.velocity.magnitude != 0)
                _animator.SetBool("isMove", true);
            else
                _animator.SetBool("isMove", false);
            if (isPattern)
                _animator.SetBool("isPattern", true);
            else
                _animator.SetBool("isPattern", false);
        }
        else
            _animator.SetBool("isDead", true);
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
        int randomInt = Random.Range(0, 4);
        switch (randomInt)
        {
            case 0:
                if (pattern_A_Cooltime == 0)
                {
                    isPattern = true;
                    pattern_A = true;
                    pattern_A_Cooltime = 40;
                    patternCycleSec = 0;
                }
                break;
            case 1:
                if (pattern_B_Cooltime == 0)
                {
                    isPattern = true;
                    pattern_B = true;
                    pattern_B_Cooltime = 20;
                    patternCycleSec = 0;
                }
                break;

            case 2:
                if (pattern_C_Cooltime == 0)
                {
                    isPattern = true;
                    pattern_C = true;
                    pattern_C_Cooltime = 30;
                    patternCycleSec = 0;
                }
                break;
            case 3:
                if (pattern_D_Cooltime == 0)
                {
                    isPattern = true;
                    pattern_D = true;
                    pattern_D_Cooltime = 20;
                    patternCycleSec = 0;
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
                _rigidbody.MovePosition(new Vector2(Random.Range(-43.5f, -39.5f), Random.Range(-7.5f, -21.5f))); // 맵 어딘가로 랜덤 이동
                GameObject ptc = Instantiate(flashPtc, transform.position, transform.rotation);
                _animator.SetFloat("patternFrame", 0);
            }
            if (patternTime > patternCycleSec && patternTime < 6.1f)
            {
                Vector3 atkVec = new Vector2(_player.transform.position.x + Random.Range(-1.0f, 1.0f), _player.transform.position.y + Random.Range(-1.0f, 1.0f));
                Vector2 sizevec = new Vector2(2f, 2f);
                GameObject warning = Instantiate(warningSign_Circle, atkVec, transform.rotation);

                warning.GetComponent<WarningSign>().SetSizeVec(sizevec);
                warning.GetComponent<WarningSign>().SetWarning_Destroy_Time(1.1f, 0.2f);
                warning.GetComponent<WarningSign>().SetPlayer(_player, _playerController);
                patternCycleSec += 0.3f;
                _animator.SetFloat("patternFrame", 1);
            }
            if (patternTime > patternCycleSec - 0.15f)
                _animator.SetFloat("patternFrame", 0);
            patternTime += Time.deltaTime;
            if (patternTime > 8)
            {
                pattern_A = false;
                isPattern = false;
                patternTime = 0;
                patternCycleSec = 0;
                _animator.SetFloat("patternFrame", 0);
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
            Move_NotNearPlayer();
            if (patternTime == 0)
            {
                _rigidbody.MovePosition(new Vector2(Random.Range(-43.5f, -39.5f), Random.Range(-7.5f, -21.5f))); // 맵 어딘가로 랜덤 이동
                GameObject ptc = Instantiate(flashPtc, transform.position, transform.rotation);
                _animator.SetFloat("patternFrame", 1);
            }
            if (patternTime > patternCycleSec && patternTime < 3.1f)
            {
                float fireX = _player.transform.position.x - transform.position.x + Random.Range(-2f, 2f);
                float fireY = _player.transform.position.y - transform.position.y + Random.Range(-2f, 2f);
                Vector2 fireVec = new Vector2(fireX, fireY).normalized;
                float firePower = Random.Range(1f, 8f);
                float sizeXY = Random.Range(1f, 3f);
                GameObject warning = Instantiate(warningSign_Circle, transform.position, transform.rotation);
                Vector2 sizevec = new Vector2(sizeXY, sizeXY);
                warning.GetComponent<WarningSign>().SetSizeVec(sizevec);
                warning.GetComponent<WarningSign>().SetWarning_Destroy_Time(Random.Range(1.5f, 2.5f), 0.2f);
                warning.GetComponent<WarningSign>().SetPlayer(_player, _playerController);
                warning.GetComponent<Rigidbody2D>().velocity = fireVec * firePower;
                patternCycleSec += 0.15f;
            }
            patternTime += Time.deltaTime;
            if (patternTime > 5)
            {
                pattern_B = false;
                isPattern = false;
                patternTime = 0;
                _animator.SetFloat("patternFrame", 0);
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
            Move_NotNearPlayer();
            if (patternTime == 0)
            {
                _animator.SetFloat("patternFrame", 0);
                GameObject ptc = Instantiate(flashPtc, transform.position, transform.rotation);
                _rigidbody.MovePosition(new Vector2(-41.5f, -14.5f)); // 맵 중앙 이동
                float fireX = Random.Range(-0.5f, 0.5f);
                float fireY = Random.Range(-1.5f, 1.5f);
                float firePower = 1.2f;
                Vector2 fireVec = new Vector2(fireX, fireY).normalized;
                Vector2 sizevec = new Vector2(20f, 20f);
                GameObject warning = Instantiate(warningSign_Circle, new Vector2(-41.5f, -14.5f), transform.rotation);
                warning.GetComponent<WarningSign>().SetSizeVec(sizevec);
                warning.GetComponent<WarningSign>().SetWarning_Destroy_Time(4f, 0.2f);
                warning.GetComponent<WarningSign>().SetPlayer(_player, _playerController);
                warning.GetComponent<Rigidbody2D>().velocity = fireVec * firePower;
            }
            patternTime += Time.deltaTime;
            if (patternTime > 4)
                _animator.SetFloat("patternFrame", 1);
            if (patternTime > 6)
            {
                pattern_C = false;
                isPattern = false;
                patternCycleSec = 0;
                patternTime = 0;
                _animator.SetFloat("patternFrame", 0);
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
            Move_NotNearPlayer();
            if (patternTime == 0)
            {
                _rigidbody.MovePosition(new Vector2(Random.Range(-43.5f, -39.5f), Random.Range(-7.5f, -21.5f))); // 맵 어딘가로 랜덤 이동
                GameObject ptc = Instantiate(flashPtc, transform.position, transform.rotation);
                _animator.SetFloat("patternFrame", 0);
            }
            if (patternTime > patternCycleSec && patternTime < 7.1f)
            {
                Vector3 rotateVec = new Vector3(0, 0, Random.Range(0, 360));
                Vector3 atkVec = new Vector2(Random.Range(-43.5f, -39.5f), Random.Range(-7.5f, -21.5f));
                Vector2 sizevec = new Vector2(30f, 0.5f);
                GameObject warning = Instantiate(warningSign_Circle, atkVec, transform.rotation);
                warning.transform.eulerAngles = rotateVec;
                warning.GetComponent<WarningSign>().SetSizeVec(sizevec);
                warning.GetComponent<WarningSign>().SetWarning_Destroy_Time(1.5f, 0.2f);
                warning.GetComponent<WarningSign>().SetPlayer(_player, _playerController);
                patternCycleSec += 0.25f;
                _animator.SetFloat("patternFrame", 1);
            }
            if (patternTime > patternCycleSec - 0.12f)
                _animator.SetFloat("patternFrame", 0);
            patternTime += Time.deltaTime;
            if (patternTime > 8)
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
        if (collision.gameObject.layer == 15 || collision.gameObject.layer == 16)
        {
            _statHandler.TakeDamage(collision.gameObject.layer == 15 ? _playerController.GetPower() : _playerController.GetPower() * 3);
            SoundManager.PlayClip("BossDamageSound");

            if (!_playerController.IsSniper() || collision.gameObject.layer == 16)
            {
                Destroy(collision.gameObject);
            }
            // 죽음 처리
            if (_statHandler.CurrentHP <= 0 && !isDead)
            {
                _diceExplosion.ExecuteDeathSequence();
                isDead = true;
                movementDirection = Vector2.zero;
            }
        }
    }

    private void OnDisable()
    {
        EventManager.Instance.UnregisterEvent<GameObject>("InitPlayerSpawned", InitPlayerSpawned);

    }
}
