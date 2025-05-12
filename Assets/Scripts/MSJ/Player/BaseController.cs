using UnityEngine;

public class BaseController : MonoBehaviour
{
    protected Rigidbody2D _rigidbody;

    [SerializeField] protected SpriteRenderer characterRenderer;

    protected Vector2 movementDirection = Vector2.zero;
    public Vector2 MovementDirection { get { return movementDirection; } }

    protected Vector2 lookDirection = Vector2.zero;
    public Vector2 LookDirection { get { return lookDirection; } }



    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
 
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        HandleAction();
        Rotate(lookDirection);
    }

    protected virtual void FixedUpdate()
    {
        Movment(movementDirection);
        
    }

    protected virtual void HandleAction()
    {

    }

    protected virtual void Movment(Vector2 direction)
    {
        direction = direction * 5f; // 속도 조절은 여기서

        _rigidbody.velocity = direction;
    }

    protected virtual void Rotate(Vector2 direction)
    {
        float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bool isLeft = Mathf.Abs(rotZ) > 90f;

        characterRenderer.flipX = isLeft;

    }

   
}
