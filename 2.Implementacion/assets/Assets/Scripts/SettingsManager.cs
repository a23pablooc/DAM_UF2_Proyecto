using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Gestiona la configuración del juego
/// Implementa el patrón observer
/// </summary>
public class SettingsManager : MonoBehaviour
{
    [SerializeField] private UIControl uiControl;

    [SerializeField] private AudioMixer audioMixer;
    
    [SerializeField] private Slider sldVolume;
    [SerializeField] private Toggle tglFullScreen;
    [SerializeField] private Slider sldQuality;

    private const string KeyVolume = "Volume";
    private const string KeyFullScreen = "FullScreen";
    private const string KeyQuality = "Quality";

    private float _volume;
    private bool _fullScreen;
    private int _quality;

    private void Awake()
    {
        uiControl.OnFullScreenToggleValueChanged += OnFullScreenChanged;
        uiControl.OnVolumeSliderValueChanged += OnVolumeChanged;
        uiControl.OnQualitySliderValueChanged += OnQualityChanged;
    }

    private void Start()
    {
        LoadSettings();
        ApplySettings();
    }

    private void OnDestroy()
    {
        uiControl.OnFullScreenToggleValueChanged -= OnFullScreenChanged;
        uiControl.OnVolumeSliderValueChanged -= OnVolumeChanged;
        uiControl.OnQualitySliderValueChanged -= OnQualityChanged;
    }

    private void OnApplicationQuit()
    {
        SaveSettings();
    }

    private void LoadSettings()
    {
        _volume = PlayerPrefs.GetFloat(KeyVolume, -40f);
        _fullScreen = PlayerPrefs.GetInt(KeyFullScreen, 1) == 1;
        _quality = PlayerPrefs.GetInt(KeyFullScreen, 1);

        sldVolume.value = _volume;
        tglFullScreen.isOn = _fullScreen;
        sldQuality.value = _quality;
    }

    private void ApplySettings()
    {
        audioMixer.SetFloat(KeyVolume, _volume);
        Screen.fullScreen = _fullScreen;
        QualitySettings.SetQualityLevel(_quality);
    }

    private void OnFullScreenChanged(bool fullScreen)
    {
        _fullScreen = fullScreen;
        Screen.fullScreen = fullScreen;
        SaveSettings();
    }

    private void OnQualityChanged(float quality)
    {
        if ((int)quality is < 0 or > 2) return;
        _quality = (int)quality;
        QualitySettings.SetQualityLevel((int)quality);
        SaveSettings();
    }

    private void OnVolumeChanged(float volume)
    {
        if (volume is < -80 or > 0) return;
        _volume = volume;
        audioMixer.SetFloat(KeyVolume, volume);
        SaveSettings();
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat(KeyVolume, _volume);
        PlayerPrefs.SetInt(KeyFullScreen, _fullScreen ? 1 : 0);
        PlayerPrefs.SetInt(KeyQuality, _quality);
    }
}