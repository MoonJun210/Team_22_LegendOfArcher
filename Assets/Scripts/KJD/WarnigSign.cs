using UnityEngine;

public class WarningSign : MonoBehaviour
{
    [SerializeField] private GameObject maxSize;
    [SerializeField] private SpriteRenderer maxSizeSprite;
    [SerializeField] private GameObject currentSize;
    [SerializeField] private SpriteRenderer currentSizeSprite;

    [SerializeField] private Vector2 sizeVec;

    [SerializeField] private bool circle;
    [SerializeField] private bool square;
    [SerializeField] private bool square_Vertical;

    [SerializeField] private float warningTimeFloat;
    private float warningTime;

    [SerializeField] private bool isDestroy;
    [SerializeField] private float destroyTimeFloat;
    private float destroyTime;

    void Start()
    {
        // 보스 오브젝트 스크립트에서도 사이즈 조정이 가능하기 위해 start로 설정.
        transform.localScale = sizeVec;

        currentSize.transform.localScale = Vector2.zero;
        if (square_Vertical)
            currentSize.transform.localScale = new Vector2(1, 0);
    }
    void Update()
    {
        warningTime += Time.deltaTime;
        if (!isDestroy)
        {
            if (warningTime < warningTimeFloat)
            {
                if (circle || square)
                {
                    currentSize.transform.localScale = new Vector2(warningTime / warningTimeFloat, warningTime / warningTimeFloat);
                }
                else if (square_Vertical)
                {
                    currentSize.transform.localScale = new Vector2(currentSize.transform.localScale.x, warningTime / warningTimeFloat);
                }
            }
            else
            {
                currentSize.transform.localScale = maxSize.transform.localScale;
                destroyTime = destroyTimeFloat;
                isDestroy = true;
            }
        }
        else
        {
            if (destroyTime > 0)
            {
                destroyTime -= Time.deltaTime;
                maxSizeSprite.color = new Vector4(maxSizeSprite.color.r, maxSizeSprite.color.g, maxSizeSprite.color.b, 0.5f * destroyTime / destroyTimeFloat);
                currentSizeSprite.color = new Vector4(currentSizeSprite.color.r, currentSizeSprite.color.g, currentSizeSprite.color.b, 0.5f * destroyTime / destroyTimeFloat);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetSizeVec(Vector2 vec)
    {
        sizeVec = vec;
    }
    public void SetCircle()
    {
        circle = true;
        square = false;
        square_Vertical = false;
    }
    public void SetSquare()
    {
        circle = false;
        square = true;
        square_Vertical = false;
    }
    public void SetSquare_Vertical()
    {
        circle = false;
        square = false;
        square_Vertical = true;
    }
    public void SetWarning_Destroy_Time(float warning, float destroy)
    {
        warningTimeFloat = warning;
        destroyTimeFloat = destroy;
    }
}
