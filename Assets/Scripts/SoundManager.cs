using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("全局音效音量")]
    [Range(0f, 1f)] public float masterVolume = 1f;

    [Header("音效资源")]
    public AudioClip katanaSwing;
    public AudioClip shurikenThrow;
    public AudioClip shockWaveExplode;
    public AudioClip shockWaveExplore;
    public AudioClip katanaHum;
    public AudioClip shurikenFlash;
    public AudioClip electricChoosen;
    public AudioClip katanaChoosen;
    public AudioClip shurikenChoosen;
    public AudioClip bgmClip;

    private AudioSource _audioSource;
    private AudioSource bgmSource;

    void Awake()
    {
        Instance = this;
        _audioSource = GetComponent<AudioSource>();

        if (Instance != null && Instance != this) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //跨场景保留BGM管理器
        }
        //获取/新建BGM音源
        bgmSource = GetComponent<AudioSource>();
        if (bgmSource == null) bgmSource = gameObject.AddComponent<AudioSource>();

        //BGM关键设置
        bgmSource.loop = true;    //开启自动循环
        bgmSource.volume = masterVolume;
        bgmSource.playOnAwake = false;
    }

    public void PlayBGM()
    {
        if (bgmClip == null) return;
        bgmSource.clip = bgmClip;
        bgmSource.Play();
    }
    //暂停BGM
    public void PauseBGM() => bgmSource.Pause();
    //停止BGM
    public void StopBGM() => bgmSource.Stop();
    //单独控制BGM音量
    public void SetBGMVolume(float vol) => bgmSource.volume = Mathf.Clamp01(vol);
    /// <summary>在世界坐标播放3D音效</summary>
    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, pos, masterVolume);
    }

    public void PlayKatanaSound(Vector3 pos)
    {
        PlaySound(katanaSwing, pos);
        PlaySound(katanaHum, pos);
    }

    public void PlayShurikenSound(Vector3 pos)
    {
        PlaySound(shurikenThrow, pos);
        PlaySound(shurikenFlash, pos);
    }

    public void PlayShockWaveSound(Vector3 pos)
    {
        PlaySound(shockWaveExplode, pos);
        PlaySound(shockWaveExplore, pos);
    }

    public void PlayElectricChoosen(Vector3 pos)
    {
        PlaySound(electricChoosen, pos);
    }

    public void PlayKatanaChoosen(Vector3 pos)
    {
        PlaySound(katanaChoosen, pos);
    }

    public void PlayShurikenChoosen(Vector3 pos)
    {
        PlaySound(shurikenChoosen, pos);
    }
}