using UnityEngine;

public class HexFloorGenerator : MonoBehaviour
{
    [Header("地板预制体")]
    public GameObject floorPrefab;
    [Header("网格范围")]
    public int range = 8;
    [Header("六边形尺寸(和 HexGridManager.hexSize 保持一致)")]
    public float hexSize = 1.8f;

    void Awake()
    {
        if (HexGridManager.Instance != null)
        {
            hexSize = HexGridManager.Instance.hexSize;
        }
        GenerateFloor();
    }

    void GenerateFloor()
    {
        Debug.Log("开始生成地板");
        for (int q = -range; q <= range; q++)
        {
            for (int r = Mathf.Max(-range, -q - range); r <= Mathf.Min(range, -q + range); r++)
            {
                HexCoord coord = new HexCoord(q, r);
                Vector3 worldPos = HexGridManager.Instance.HexToWorld(coord);
                // 抬高Y轴，防止遮挡
                worldPos.y = 0.02f;

                GameObject floor = Instantiate(floorPrefab, worldPos, Quaternion.identity, transform);
                floor.name = $"HexFloor_{q}_{r}";
                // 统一缩放
                floor.transform.localScale = new Vector3(hexSize, 0.01f, hexSize);
            }
        }
    }
}