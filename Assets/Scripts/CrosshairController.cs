using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    public static CrosshairController Instance;

    [Header("准星UI")]
    public GameObject crosshairUI;

    private void Awake()
    {
        Instance = this;
        Hide(); // 默认隐藏
    }

    // 显示准星
    public void Show()
    {
        crosshairUI.SetActive(true);
    }

    // 隐藏准星
    public void Hide()
    {
        crosshairUI.SetActive(false);
    }
}