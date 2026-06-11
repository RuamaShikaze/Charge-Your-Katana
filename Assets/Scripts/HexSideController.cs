using UnityEngine;

// 挂载到地板预制体，动态修改多边形边数
public class HexSideController : MonoBehaviour
{
    [Header("多边形边数")]
    [Range(3, 12)]
    public int customSideCount = 6;

    [Header("边框样式")]
    public Color sideColor = Color.cyan;
    public float sideWidth = 0.08f;

    private LineRenderer _lineRenderer;
    private float _hexSize;

    private void Awake()
    {
        // 自动添加线渲染组件
        _lineRenderer = GetComponent<LineRenderer>();
        if (_lineRenderer == null)
            _lineRenderer = gameObject.AddComponent<LineRenderer>();

        // 基础配置
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startColor = sideColor;
        _lineRenderer.endColor = sideColor;
        _lineRenderer.startWidth = sideWidth;
        _lineRenderer.endWidth = sideWidth;
        _lineRenderer.useWorldSpace = false;
        _lineRenderer.loop = true;

        // 读取网格尺寸
        if (transform.parent != null)
        {
            HexFloorGenerator gen = transform.parent.GetComponent<HexFloorGenerator>();
            _hexSize = gen != null ? gen.hexSize : HexGridManager.Instance.hexSize;
        }
        else
        {
            _hexSize = HexGridManager.Instance.hexSize;
        }

        UpdatePolygonShape();
    }

    /// <summary>根据边数重绘多边形边框</summary>
    public void UpdatePolygonShape()
    {
        if (_lineRenderer == null) return;

        _lineRenderer.positionCount = customSideCount;
        float angleStep = 360f / customSideCount;

        for (int i = 0; i < customSideCount; i++)
        {
            float rad = Mathf.Deg2Rad * (angleStep * i - 30f);
            float x = _hexSize * Mathf.Cos(rad);
            float z = _hexSize * Mathf.Sin(rad);
            _lineRenderer.SetPosition(i, new Vector3(x, 0f, z));
        }
    }

    // 编辑器改参数实时刷新
    private void OnValidate()
    {
        if (_lineRenderer != null)
            UpdatePolygonShape();
    }

    // 外部设置尺寸
    public void SetHexSize(float size)
    {
        _hexSize = size;
        UpdatePolygonShape();
    }
}