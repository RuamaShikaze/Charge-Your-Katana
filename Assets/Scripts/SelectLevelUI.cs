using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour
{
    [Header("关卡按钮")]
    public Button testLevelBtn;
    [Header("返回按钮")]
    public Button backBtn;

    private void Awake()
    {
        // 绑定点击事件
        if (testLevelBtn != null)
            testLevelBtn.onClick.AddListener(EnterTestLevel);
        if (backBtn != null)
            backBtn.onClick.AddListener(BackToStart);
    }

    /// 进入Test战斗关卡
    public void EnterTestLevel()
    {
        SceneManager.LoadScene("Test");
    }

    /// 返回开始界面
    public void BackToStart()
    {
        SceneManager.LoadScene("Start");
    }
}