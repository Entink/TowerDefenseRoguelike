using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager I { get; private set; }

    [Header("Mixer")]
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private AudioMixerGroup musicGroup;

    [Header("Clips")]
    [SerializeField] private AudioClip mainBaseLoop;
    [SerializeField] private AudioClip fightLoop;
    [SerializeField] private AudioClip victoryLoop;

    [Header("Fade")]
    [SerializeField, Range(0.1f, 5f)] private float fadeTime = 1.2f;

    private AudioSource a, b;
    private AudioSource active;

    private void Awake()
    {
        if(I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        a = gameObject.AddComponent<AudioSource>();
        b = gameObject.AddComponent<AudioSource>();
        a.loop = b.loop = true;
        a.playOnAwake = b.playOnAwake = false;
        a.outputAudioMixerGroup = musicGroup;
        b.outputAudioMixerGroup = musicGroup;

        active = a;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        if (s.name == "MainBaseScene") Play(mainBaseLoop);
        else if (s.name == "FightScene") Play(fightLoop);
        else if (s.name == "VictoryScene") Play(victoryLoop);
    }

    public void Play(AudioClip clip)
    {
        if (clip == null) return;
        var from = active;
        var to = (active == a) ? b : a;

        to.clip = clip;
        to.volume = 0f;
        to.Play();

        StartCoroutine(Crossfade(from, to, fadeTime));
        active = to;
    }

    private IEnumerator Crossfade(AudioSource from, AudioSource to, float t)
    {
        float time = 0f;

        while(time < t)
        {
            time += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(time / t);
            to.volume = k;
            from.volume = 1f - k;
            yield return null;
        }
        from.Stop();
        to.volume = 1f;
    }

    public void SetVolume01(string exposedParam, float value01)
    {
        float db = (value01 <= 0.0001f) ? -80f : Mathf.Lerp(-30f, 0f, Mathf.Log10(Mathf.Lerp(0.0001f, 1f, value01)) * 1f + 1f);
        masterMixer.SetFloat(exposedParam, db);
    }

    public float GetVolume01(string exposedParam)
    {
        if (!masterMixer.GetFloat(exposedParam, out float db)) return 1f;
        return Mathf.Clamp01(Mathf.InverseLerp(-30f, 0f, db));
    }

}
