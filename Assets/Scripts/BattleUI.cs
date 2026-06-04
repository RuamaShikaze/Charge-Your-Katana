using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    public static BattleUI Instance;

    [Header("左手电性图标")]
    public Image leftElementIcon;
    public Sprite positiveSprite;
    public Sprite negativeSprite;

    [Header("右手武器图标")]
    public Image rightWeaponIcon;
    public Sprite katanaSprite;
    public Sprite shurikenSprite;

    [Header("目标圈预设（挂敌人身上）")]
    public GameObject targetCirclePrefab;

    private GameObject _currentTargetCircle;
    private Transform _currentTarget;

    private void Awake()
    {
        Instance = this;
        HideAll();
    }

    public void HideAll()
    {
        HideElement();
        HideWeapon();
        ClearTargetCircle();
    }

    // --------------------
    // 左手电性
    // --------------------
    public void ShowElement(ElementType type)
    {
        leftElementIcon.gameObject.SetActive(true);

        if (type == ElementType.PositiveElectricity)
            leftElementIcon.sprite = positiveSprite;
        else
            leftElementIcon.sprite = negativeSprite;
    }

    public void HideElement()
    {
        leftElementIcon.gameObject.SetActive(false);
    }

    // --------------------
    // 右手武器
    // --------------------
    public void ShowWeapon(WeaponType type)
    {
        rightWeaponIcon.gameObject.SetActive(true);

        if (type == WeaponType.Katana)
            rightWeaponIcon.sprite = katanaSprite;
        else
            rightWeaponIcon.sprite = shurikenSprite;
    }

    public void HideWeapon()
    {
        rightWeaponIcon.gameObject.SetActive(false);
    }

    // --------------------
    // 目标圈：真正贴在敌人身上
    // --------------------
    // --------------------
    // 目标圈：自动适配敌人大小
    // --------------------
    public void SetTarget(Transform target)
    {
        ClearTargetCircle();

        if (target == null) return;
        _currentTarget = target;

        // 创建圈 → 挂到目标身上
        _currentTargetCircle = Instantiate(targetCirclePrefab, target);
        _currentTargetCircle.transform.localPosition = Vector3.zero;
        _currentTargetCircle.SetActive(true);

        // ======================================================================
        // ✅ 核心：自动获取敌人碰撞体/渲染器边界，自动缩放圈大小
        // ======================================================================
        float finalSize = 1.2f; // 留白边距，让圈比敌人大一点点
        Renderer rend = target.GetComponent<Renderer>();
        Collider col = target.GetComponent<Collider>();

        if (rend != null)
        {
            Bounds b = rend.bounds;
            float sizeX = b.extents.x * 2f;
            float sizeY = b.extents.y * 2f;
            float sizeZ = b.extents.z * 2f;
            float maxDim = Mathf.Max(sizeX, sizeY, sizeZ);
            finalSize *= maxDim;
        }
        else if (col != null)
        {
            Bounds b = col.bounds;
            float sizeX = b.extents.x * 2f;
            float sizeY = b.extents.y * 2f;
            float sizeZ = b.extents.z * 2f;
            float maxDim = Mathf.Max(sizeX, sizeY, sizeZ);
            finalSize *= maxDim;
        }

        // 应用自适应缩放
        _currentTargetCircle.transform.localScale = Vector3.one * finalSize;

        // 让圈永远面朝相机（第一人称最舒服）
        _currentTargetCircle.AddComponent<FaceCamera>();
    }

    public void ClearTargetCircle()
    {
        if (_currentTargetCircle != null)
            Destroy(_currentTargetCircle);

        _currentTarget = null;
    }

    // --------------------
    // 攻击动画（右手轻微前推）
    // --------------------
    public void PlayAttackAnim()
    {
        StopAllCoroutines();
        StartCoroutine(Anim_Attack());
    }

    private System.Collections.IEnumerator Anim_Attack()
    {
        RectTransform rt = rightWeaponIcon.rectTransform;
        Vector2 orig = rt.anchoredPosition;
        Vector2 push = orig + new Vector2(-20, 20);

        float t = 0;
        while (t < 0.15f)
        {
            t += Time.deltaTime;
            rt.anchoredPosition = Vector2.Lerp(orig, push, t / 0.15f);
            yield return null;
        }

        t = 0;
        while (t < 0.15f)
        {
            t += Time.deltaTime;
            rt.anchoredPosition = Vector2.Lerp(push, orig, t / 0.15f);
            yield return null;
        }
    }
}