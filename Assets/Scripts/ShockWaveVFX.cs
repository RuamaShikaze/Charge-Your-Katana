using UnityEngine;

public class ShockWaveVFX : MonoBehaviour
{
    [Header("扩散范围（可外部调用设置）")]
    public float maxRadius = 8f;       // 视觉最大范围
    public float expandSpeed = 14f;    // 扩散速度
    public float fadeSpeed = 3.5f;     // 消失速度

    private float currentRadius;
    private Material mat;
    private Color baseColor;

    void Awake()
    {
        mat = GetComponent<MeshRenderer>().material;
        baseColor = mat.color;
        currentRadius = 0f;
    }

    void Update()
    {
        // 扩散
        if (currentRadius < maxRadius)
        {
            currentRadius += expandSpeed * Time.deltaTime;
            transform.localScale = Vector3.one * currentRadius;
        }
        else
        {
            // 淡出
            baseColor.a -= fadeSpeed * Time.deltaTime;
            mat.color = baseColor;
            if (baseColor.a <= 0) Destroy(gameObject);
        }
    }

    // 外部设置颜色（公开接口）
    public void SetColor(Color color)
    {
        mat.color = color;
        baseColor = color;
    }

    // 外部设置扩散范围（公开接口 ✅）
    public void SetRadius(float radius)
    {
        maxRadius = radius;
    }
}