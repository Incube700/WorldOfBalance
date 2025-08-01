using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WorldOfBalance.Audio;

namespace WorldOfBalance.UI
{
    /// <summary>
    /// Менеджер настроек игры для управления громкостью, чувствительностью и разрешением экрана.
    /// Сохраняет настройки между сессиями игры.
    /// </summary>
    public class SettingsManager : MonoBehaviour
    {
        [Header("Audio Settings")]
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private TextMeshProUGUI musicVolumeText;
        [SerializeField] private TextMeshProUGUI sfxVolumeText;
        
        [Header("Gameplay Settings")]
        [SerializeField] private Slider mouseSensitivitySlider;
        [SerializeField] private TextMeshProUGUI mouseSensitivityText;
        [SerializeField] private Toggle invertYAxisToggle;
        
        [Header("Graphics Settings")]
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private TMP_Dropdown qualityDropdown;
        
        [Header("UI References")]
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private Button backButton;
        [SerializeField] private Button resetToDefaultsButton;
        
        // Default values
        private const float DEFAULT_MUSIC_VOLUME = 0.7f;
        private const float DEFAULT_SFX_VOLUME = 1f;
        private const float DEFAULT_MOUSE_SENSITIVITY = 1f;
        private const bool DEFAULT_INVERT_Y = false;
        
        // Available resolutions
        private Resolution[] availableResolutions;
        
        void Start()
        {
            InitializeSettings();
            LoadSettings();
            SetupUICallbacks();
        }
        
        void InitializeSettings()
        {
            // Initialize resolution dropdown
            if (resolutionDropdown != null)
            {
                availableResolutions = Screen.resolutions;
                resolutionDropdown.ClearOptions();
                
                for (int i = 0; i < availableResolutions.Length; i++)
                {
                    string resolutionString = $"{availableResolutions[i].width} x {availableResolutions[i].height}";
                    resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(resolutionString));
                }
                
                resolutionDropdown.RefreshShownValue();
            }
            
            // Initialize quality dropdown
            if (qualityDropdown != null)
            {
                qualityDropdown.ClearOptions();
                string[] qualityNames = QualitySettings.names;
                
                for (int i = 0; i < qualityNames.Length; i++)
                {
                    qualityDropdown.options.Add(new TMP_Dropdown.OptionData(qualityNames[i]));
                }
                
                qualityDropdown.RefreshShownValue();
            }
        }
        
        void SetupUICallbacks()
        {
            // Audio sliders
            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
                
            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            
            // Gameplay settings
            if (mouseSensitivitySlider != null)
                mouseSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChanged);
                
            if (invertYAxisToggle != null)
                invertYAxisToggle.onValueChanged.AddListener(OnInvertYAxisChanged);
            
            // Graphics settings
            if (resolutionDropdown != null)
                resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
                
            if (fullscreenToggle != null)
                fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
                
            if (qualityDropdown != null)
                qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
            
            // Buttons
            if (backButton != null)
                backButton.onClick.AddListener(CloseSettings);
                
            if (resetToDefaultsButton != null)
                resetToDefaultsButton.onClick.AddListener(ResetToDefaults);
        }
        
        /// <summary>
        /// Открывает панель настроек
        /// </summary>
        public void OpenSettings()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(true);
            }
            
            // Play sound effect
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.OnButtonClick();
            }
        }
        
        /// <summary>
        /// Закрывает панель настроек и сохраняет изменения
        /// </summary>
        public void CloseSettings()
        {
            SaveSettings();
            
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }
            
            // Play sound effect
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.OnButtonClick();
            }
        }
        
        void OnMusicVolumeChanged(float value)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.MusicVolume = value;
            }
            
            if (musicVolumeText != null)
            {
                musicVolumeText.text = $"{(value * 100):F0}%";
            }
        }
        
        void OnSFXVolumeChanged(float value)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SFXVolume = value;
            }
            
            if (sfxVolumeText != null)
            {
                sfxVolumeText.text = $"{(value * 100):F0}%";
            }
        }
        
        void OnMouseSensitivityChanged(float value)
        {
            if (mouseSensitivityText != null)
            {
                mouseSensitivityText.text = $"{value:F1}";
            }
            
            // Apply to tank controllers if needed
            ApplyMouseSensitivity(value);
        }
        
        void OnInvertYAxisChanged(bool value)
        {
            // Apply to tank controllers if needed
            ApplyInvertYAxis(value);
        }
        
        void OnResolutionChanged(int index)
        {
            if (index >= 0 && index < availableResolutions.Length)
            {
                Resolution resolution = availableResolutions[index];
                Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            }
        }
        
        void OnFullscreenChanged(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }
        
        void OnQualityChanged(int index)
        {
            QualitySettings.SetQualityLevel(index);
        }
        
        void ApplyMouseSensitivity(float sensitivity)
        {
            // Find all tank controllers and apply sensitivity
            TankController[] tanks = FindObjectsOfType<TankController>();
            foreach (var tank in tanks)
            {
                // Apply sensitivity if tank has the property
                // This would need to be implemented in TankController
            }
        }
        
        void ApplyInvertYAxis(bool invert)
        {
            // Apply Y-axis inversion to controls
            // This would need to be implemented in input controllers
        }
        
        /// <summary>
        /// Сбрасывает все настройки к значениям по умолчанию
        /// </summary>
        public void ResetToDefaults()
        {
            // Audio settings
            if (musicVolumeSlider != null)
                musicVolumeSlider.value = DEFAULT_MUSIC_VOLUME;
                
            if (sfxVolumeSlider != null)
                sfxVolumeSlider.value = DEFAULT_SFX_VOLUME;
            
            // Gameplay settings
            if (mouseSensitivitySlider != null)
                mouseSensitivitySlider.value = DEFAULT_MOUSE_SENSITIVITY;
                
            if (invertYAxisToggle != null)
                invertYAxisToggle.isOn = DEFAULT_INVERT_Y;
            
            // Graphics settings
            if (fullscreenToggle != null)
                fullscreenToggle.isOn = Screen.fullScreen;
                
            if (qualityDropdown != null)
                qualityDropdown.value = QualitySettings.GetQualityLevel();
            
            Debug.Log("SettingsManager: Reset to defaults");
            
            // Play sound effect
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.OnButtonClick();
            }
        }
        
        /// <summary>
        /// Сохраняет настройки в PlayerPrefs
        /// </summary>
        public void SaveSettings()
        {
            // Audio settings
            if (musicVolumeSlider != null)
                PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
                
            if (sfxVolumeSlider != null)
                PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
            
            // Gameplay settings
            if (mouseSensitivitySlider != null)
                PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivitySlider.value);
                
            if (invertYAxisToggle != null)
                PlayerPrefs.SetInt("InvertYAxis", invertYAxisToggle.isOn ? 1 : 0);
            
            // Graphics settings
            if (resolutionDropdown != null)
                PlayerPrefs.SetInt("ResolutionIndex", resolutionDropdown.value);
                
            if (fullscreenToggle != null)
                PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);
                
            if (qualityDropdown != null)
                PlayerPrefs.SetInt("QualityLevel", qualityDropdown.value);
            
            PlayerPrefs.Save();
            Debug.Log("SettingsManager: Settings saved");
        }
        
        /// <summary>
        /// Загружает настройки из PlayerPrefs
        /// </summary>
        public void LoadSettings()
        {
            // Audio settings
            if (musicVolumeSlider != null)
            {
                float musicVolume = PlayerPrefs.GetFloat("MusicVolume", DEFAULT_MUSIC_VOLUME);
                musicVolumeSlider.value = musicVolume;
                OnMusicVolumeChanged(musicVolume);
            }
            
            if (sfxVolumeSlider != null)
            {
                float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", DEFAULT_SFX_VOLUME);
                sfxVolumeSlider.value = sfxVolume;
                OnSFXVolumeChanged(sfxVolume);
            }
            
            // Gameplay settings
            if (mouseSensitivitySlider != null)
            {
                float sensitivity = PlayerPrefs.GetFloat("MouseSensitivity", DEFAULT_MOUSE_SENSITIVITY);
                mouseSensitivitySlider.value = sensitivity;
                OnMouseSensitivityChanged(sensitivity);
            }
            
            if (invertYAxisToggle != null)
            {
                bool invertY = PlayerPrefs.GetInt("InvertYAxis", DEFAULT_INVERT_Y ? 1 : 0) == 1;
                invertYAxisToggle.isOn = invertY;
                OnInvertYAxisChanged(invertY);
            }
            
            // Graphics settings
            if (resolutionDropdown != null)
            {
                int resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", availableResolutions.Length - 1);
                if (resolutionIndex < availableResolutions.Length)
                {
                    resolutionDropdown.value = resolutionIndex;
                }
            }
            
            if (fullscreenToggle != null)
            {
                bool fullscreen = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;
                fullscreenToggle.isOn = fullscreen;
                OnFullscreenChanged(fullscreen);
            }
            
            if (qualityDropdown != null)
            {
                int qualityLevel = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
                qualityDropdown.value = qualityLevel;
                OnQualityChanged(qualityLevel);
            }
            
            Debug.Log("SettingsManager: Settings loaded");
        }
    }
}