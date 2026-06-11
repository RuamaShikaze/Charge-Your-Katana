using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexFloorMesh : MonoBehaviour
{
    private Mesh _hexMesh;

    void Awake()
    {
        BuildHexMesh();
    }

    void BuildHexMesh()
    {
        _hexMesh = new Mesh();
        _hexMesh.name = "HexFloorMesh";
        GetComponent<MeshFilter>().mesh = _hexMesh;

        // 顶点：中心 + 6个外圈点
        Vector3[] vertices = new Vector3[7];
        vertices[0] = Vector3.zero;
        float angleStep = Mathf.PI * 2f / 6f;

        for (int i = 0; i < 6; i++)
        {
            float rad = angleStep * i;
            vertices[i + 1] = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad));
        }

        // UV（补上UV，解决黑屏/不显示）
        Vector2[] uvs = new Vector2[7];
        uvs[0] = new Vector2(0.5f, 0.5f);
        for (int i = 0; i < 6; i++)
        {
            float rad = angleStep * i;
            uvs[i + 1] = new Vector2(0.5f + Mathf.Cos(rad) * 0.5f, 0.5f + Mathf.Sin(rad) * 0.5f);
        }

        // 三角索引（顺时针面朝向，保证正面可见）
        int[] triangles = new int[18];
        for (int i = 0; i < 6; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = (i + 1) % 6 + 1;
        }

        // 赋值网格数据
        _hexMesh.vertices = vertices;
        _hexMesh.uv = uvs;
        _hexMesh.triangles = triangles;

        _hexMesh.RecalculateNormals();
        _hexMesh.RecalculateBounds();
    }
}