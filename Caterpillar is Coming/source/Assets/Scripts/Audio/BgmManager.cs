using UnityEngine;

// 全局背景音乐：
// 用 RuntimeInitializeOnLoadMethod 在任意场景启动时自动创建常驻 BGM 对象，
// 循环播放 Resources/Audio 下的音乐；跨场景切换、按 R 重开场景都不会中断。
public static class BgmManager
{
    // 音乐路径（Resources/Audio/ 下的文件，不带扩展名）
    private const string ClipPath = "Audio/Pixel-Paradise-Demo";

    private static AudioSource source;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoStart()
    {
        // 创建常驻对象（切场景不会销毁）
        GameObject go = new GameObject("[BGM]");
        Object.DontDestroyOnLoad(go);

        source = go.AddComponent<AudioSource>();
        source.loop = true;
        source.playOnAwake = false;
        source.volume = 0.5f;

        AudioClip clip = Resources.Load<AudioClip>(ClipPath);
        if (clip != null)
        {
            source.clip = clip;
            source.Play();
        }
        else
        {
            Debug.LogWarning("[BGM] 没有找到音乐：" + ClipPath);
        }
    }

    // 后面想调音量/静音可以这样调用：
    // BgmManager.SetVolume(0.3f);  BgmManager.Mute(true);  BgmManager.Stop();
    public static void SetVolume(float v)
    {
        if (source != null) source.volume = Mathf.Clamp01(v);
    }

    public static void Mute(bool mute)
    {
        if (source != null) source.mute = mute;
    }

    public static void Stop()
    {
        if (source != null) source.Stop();
    }
}
