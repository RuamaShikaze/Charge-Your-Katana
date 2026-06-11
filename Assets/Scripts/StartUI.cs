using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartUI : MonoBehaviour
{
    [Header("开始按钮")]
    public Button startBtn;

    private void Awake()
    {
        // 绑定按钮点击事件
        if (startBtn != null)
        {
            startBtn.onClick.AddListener(GoToSelectLevel);
        }
    }

    /// 点击开始 → 跳转选关界面
    public void GoToSelectLevel()
    {
        SceneManager.LoadScene("SelectLevel");
    }
}