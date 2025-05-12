using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Boss_2 : BaseController
{
    [SerializeField] private GameObject _player;
    [SerializeField] private BossTriggerColider triggerColider;
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
    protected override void Awake()
    {
        base.Awake();
        //player = GameObject.Find("Player"); << 플레이어 찾기
        EventManager.Instance.RegisterEvent<GameObject>("InitPlayerSpawned", InitPlayerSpawned);
        triggerColider = GetComponentInChildren<BossTriggerColider>();
    }
    protected override void Update()
    {
        if (_player != null)
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
                    pattern_B_Cooltime = 40;
                    patternCycleSec = 0;
                }
                break;

            case 2:
                if (pattern_C_Cooltime == 0)
                {
                    isPattern = true;
                    pattern_C = true;
                    pattern_C_Cooltime = 20;
                    patternCycleSec = 0;
                }
                break;
            case 3:
                if (pattern_D_Cooltime == 0)
                {
                    isPattern = true;
                    pattern_D = true;
                    pattern_D_Cooltime = 25;
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
            _rigidbody.MovePosition(new Vector2(0, 11.5f)); // 맵 중앙 이동
            if (patternTime > patternCycleSec && patternTime < 9.1f)
            {
                Vector3 rotateVec = new Vector3(0, 0, patternTime * 60);
                Vector2 sizevec = new Vector2(1.5f, 16);
                GameObject warning = Instantiate(warningSign_Square, new Vector2(0, 11.5f), transform.rotation);
                warning.transform.eulerAngles = rotateVec;

                warning.GetComponent<WarningSign>().SetSizeVec(sizevec);
                warning.GetComponent<WarningSign>().SetWarning_Destroy_Time(1.4f, 0.2f);
                warning.GetComponent<WarningSign>().SetPlayer(_player, _playerController);
                patternCycleSec += 0.25f;
            }
            patternTime += Time.deltaTime;
            if (patternTime > 12)
            {
                pattern_A = false;
                isPattern = false;
                patternTime = 0;
                patternCycleSec = 0;
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
            _rigidbody.MovePosition(new Vector2(0, 11.5f)); // 맵 중앙 이동
            if (patternTime > patternCycleSec && patternTime < 9.1f)
            {
                Vector3 rotateVec = new Vector3(0, 0, -patternTime * 60);
                Vector2 sizevec = new Vector2(1.5f, 16);
                GameObject warning = Instantiate(warningSign_Square, new Vector2(0, 11.5f), transform.rotation);
                warning.transform.eulerAngles = rotateVec;

                warning.GetComponent<WarningSign>().SetSizeVec(sizevec);
                warning.GetComponent<WarningSign>().SetWarning_Destroy_Time(1.4f, 0.2f);
                warning.GetComponent<WarningSign>().SetPlayer(_player, _playerController);
                patternCycleSec += 0.25f;
            }
            patternTime += Time.deltaTime;
            if (patternTime > 11)
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
            if (patternTime == 0)
                _rigidbody.MovePosition(new Vector2(Random.Range(-5, 6), Random.Range(7.5f, 15f))); // 맵 어딘가로 랜덤 이동
            if (patternTime > patternCycleSec && patternTime < 3.5f)
            {
                Vector2 sizevec = new Vector2(2f, 11);
                Vector2 posVec = new Vector2(-6 + patternCycleSec * 4, 11.5f);
                GameObject warning = Instantiate(warningSign_Square, posVec, transform.rotation);
                warning.GetComponent<WarningSign>().SetSquare_Vertical();
                warning.GetComponent<WarningSign>().SetSizeVec(sizevec);
                warning.GetComponent<WarningSign>().SetWarning_Destroy_Time(1.5f, 0.2f);
                warning.GetComponent<WarningSign>().SetPlayer(_player, _playerController);
                patternCycleSec += 0.5f;
            }
            patternTime += Time.deltaTime;
            if (patternTime > 6)
            {
                pattern_C = false;
                isPattern = false;
                patternCycleSec = 0;
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
            patternTime += Time.deltaTime;
            if (patternTime < 1)
            {
                _rigidbody.MovePosition(new Vector2(Random.Range(-5, 6), Random.Range(7.5f, 15f))); // 맵 어딘가로 랜덤 이동
                patternTime = 1;
                Vector2 CenterVec = new Vector2(Random.Range(-5, 6), Random.Range(7.5f, 15f)); // 생존 구역 맵 어딘가로 순간이동
                for (int i = 0; i < 4; i++)
                {
                    Vector2 sizevec = new Vector2(15f, 15f);
                    Vector2 caseVec = Vector2.zero;
                    switch (i)
                    {
                        case 0:
                            caseVec = new Vector2(CenterVec.x - 9, CenterVec.y + 6); break;
                        case 1:
                            caseVec = new Vector2(CenterVec.x + 6, CenterVec.y + 9); break;
                        case 2:
                            caseVec = new Vector2(CenterVec.x + 9, CenterVec.y + -6); break;
                        case 3:
                            caseVec = new Vector2(CenterVec.x - 6, CenterVec.y + -9); break;

                    }
                    GameObject warning = Instantiate(warningSign_Square, caseVec, transform.rotation);
                    warning.GetComponent<WarningSign>().SetSizeVec(sizevec);
                    warning.GetComponent<WarningSign>().SetWarning_Destroy_Time(3f, 0.2f);
                    warning.GetComponent<WarningSign>().SetPlayer(_player, _playerController);
                }
            }
            if (patternTime > 4)
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
}
