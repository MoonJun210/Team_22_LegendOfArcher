using UnityEngine;

public class Boss_1 : BaseController
{
    [SerializeField] private GameObject player;

    [SerializeField] private bool detectPlayer;
    [SerializeField] private float moveCooltime;
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
            Move_NearPlayer();
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
        direction = direction * 1f; // 속도 조절은 여기서

        _rigidbody.velocity = direction;
    }
    protected override void Rotate(Vector2 direction)
    {
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bool isLeft = Mathf.Abs(rotZ) > 90f;

        if (direction.x != 0)
            characterRenderer.flipX = isLeft;

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
