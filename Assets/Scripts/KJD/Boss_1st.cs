using UnityEngine;

public class Boss_1 : BaseController
{
    [SerializeField] private GameObject player;
    [SerializeField] private float moveSpeed;

    [SerializeField] private GameObject warningSign_Circle;
    [SerializeField] private GameObject warningSign_Square;

    [SerializeField] private bool detectPlayer;
    [SerializeField] private float moveCooltime;

    [SerializeField] private bool isPattern;
    [SerializeField] private float patternTime;

    [SerializeField] private bool pattern_A;
    [SerializeField] private float pattern_A_Cooltime;
    protected override void Awake()
    {
        base.Awake();
        //player = GameObject.Find("Player"); << 플레이어 찾기
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
            Pattern_A();
        }
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
                    //패턴 A 함수 호출
                    isPattern = true;
                    pattern_A = true;
                    Debug.Log("패턴 A 실행");
                    pattern_A_Cooltime = 5;
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
        if (Mathf.Abs(player.transform.position.x - transform.position.x) > 1)
            moveVec.x = player.transform.position.x - transform.position.x;
        else
            moveVec.x = 0;
        if (Mathf.Abs(player.transform.position.y - transform.position.y) > 1)
            moveVec.y = player.transform.position.y - transform.position.y;
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
                    Vector2 playerNearVec = new Vector2(player.transform.position.x + Random.Range(-1f, 2f), player.transform.position.y + Random.Range(-1f, 2f));
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            detectPlayer = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            detectPlayer = false;
        }
    }
}
