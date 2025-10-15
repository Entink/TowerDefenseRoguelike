using UnityEngine;
using UnityEngine.Audio;

public class SfxManager : MonoBehaviour
{
    public static SfxManager I { get; private set; }

    [Header("UI")]
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup uiGroup;
    [SerializeField] private AudioSource uiSource;
    [SerializeField] private AudioClip uiClick;
    [Range(0f, 1f)] public float buttonVolume = 0.9f;


    [Header("Units (shared)")]
    public AudioClip unitAttack;
    public AudioClip unitHit;
    public AudioClip unitDeath;
    [Range(0f, 1f)] public float unitVolume = 0.9f;
    public Vector2 randomPitch = new Vector2(0.95f, 1.05f);

    private void Awake()
    {
        if(I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        if(uiSource == null)
        {
            uiSource = gameObject.AddComponent<AudioSource>();
            uiSource.playOnAwake = false;
            uiSource.spatialBlend = 0f;
            uiSource.outputAudioMixerGroup = uiGroup;
        }
    }

    public void PlayUiClick()
    {
        if (uiClick != null) uiSource.PlayOneShot(uiClick, buttonVolume);
    }

    public void PlayWorldOneShot(AudioClip clip, Vector3 worldPos, float volume = 1f)
    {
        if (clip == null) return;

        float pitch = Random.RandomRange(randomPitch.x, randomPitch.y);

        var go = new GameObject("Sfx_OneShot");
        var src = go.AddComponent<AudioSource>();
        src.outputAudioMixerGroup = sfxGroup;

        src.spatialBlend = 0f;
        src.pitch = pitch;
        src.PlayOneShot(clip, volume);
        Destroy(go, clip.length / Mathf.Max(0.01f, pitch));
    }
}
