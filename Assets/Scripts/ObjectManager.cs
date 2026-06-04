using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject shockWavePrefab; // 这里会出现一个框

    void Awake()
    {
        ElementReaction.ElementTypes.shockWavePrefab = shockWavePrefab;
    }
}