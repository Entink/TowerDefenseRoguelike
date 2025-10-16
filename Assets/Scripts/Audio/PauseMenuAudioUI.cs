using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenuAudioUI : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider master;
    [SerializeField] private Slider music;
    [SerializeField] private Slider sfx;
    [SerializeField] private Slider ui;

    [Header("Percent text")]
    [SerializeField] private TextMeshProUGUI masterPct;
    [SerializeField] private TextMeshProUGUI musicPct;
    [SerializeField] private TextMeshProUGUI sfxPct;
    [SerializeField] private TextMeshProUGUI uiPct;


    const string K_Master = "vol_master";
    const string K_Music = "vol_music";
    const string K_Sfx = "vol_sfx";
    const string K_UI = "vol_ui";

    private void Awake()
    {
        ApplyAll();
        UpdateLabels();
    }


    private void OnEnable()
    {
        master.value = PlayerPrefs.GetFloat(K_Master, 0.8f);
        music.value = PlayerPrefs.GetFloat(K_Music, 0.8f);
        sfx.value = PlayerPrefs.GetFloat(K_Sfx, 0.9f);
        ui.value = PlayerPrefs.GetFloat(K_UI, 0.8f);

        ApplyAll();
        UpdateLabels();

        master.onValueChanged.AddListener(OnChanged);
        music.onValueChanged.AddListener(OnChanged);
        sfx.onValueChanged.AddListener(OnChanged);
        ui.onValueChanged.AddListener(OnChanged);
    }

    private void OnDisable()
    {
        master.onValueChanged.RemoveListener(OnChanged);
        music.onValueChanged.RemoveListener(OnChanged);
        sfx.onValueChanged.RemoveListener(OnChanged);
        ui.onValueChanged.RemoveListener(OnChanged);
    }

    private void OnChanged(float _)
    {
        ApplyAll();
        UpdateLabels();
        PlayerPrefs.SetFloat(K_Master, master.value);
        PlayerPrefs.SetFloat(K_Music, music.value);
        PlayerPrefs.SetFloat(K_Sfx, sfx.value);
        PlayerPrefs.SetFloat(K_UI, ui.value);
        PlayerPrefs.Save();
    }

    public void ApplyAll()
    {
        if (MusicManager.I == null) return;
        MusicManager.I.SetVolume01("MasterVolume", master.value);
        MusicManager.I.SetVolume01("MusicVolume", music.value);
        MusicManager.I.SetVolume01("SFXVolume", sfx.value);
        MusicManager.I.SetVolume01("UIVolume", ui.value);
    }

    private void UpdateLabels()
    {
        masterPct.text = Mathf.RoundToInt(master.value * 100f) + "%";
        musicPct.text = Mathf.RoundToInt(music.value * 100f) + "%";
        sfxPct.text = Mathf.RoundToInt(sfx.value * 100f) + "%";
        uiPct.text = Mathf.RoundToInt(ui.value * 100f) + "%";
    }
}
