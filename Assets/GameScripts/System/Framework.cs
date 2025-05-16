/// 2025-05-14 

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T I => _instance;
    private static T _instance = null;
    
    public virtual void Awake()
    {
        if(_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(this);
            Init();
        }
        else
        {
        }
    }

    public abstract UniTask Init();
}


public class Framework : Singleton<Framework>
{
    public SystemLanguage Language => _language;
    public UISystem UI => _uiSystem;
    public ResourceSystem Resource => _resourceSystem;
    public MapSystem Map => _mapSystem;
    public CharacterSystem Character => _characterSystem;
    public TableSystem Table => _tableSystem;
    
    private UISystem _uiSystem;
    private ResourceSystem _resourceSystem;
    private MapSystem _mapSystem;
    private CharacterSystem _characterSystem;
    private TableSystem _tableSystem;
    
    private List<ISystem> _systems = new List<ISystem>();
    private SystemLanguage _language = SystemLanguage.Korean;
    
    public override async UniTask Init()
    {
        _resourceSystem = this.AddComponent<ResourceSystem>();
        _uiSystem = this.AddComponent<UISystem>();
        _mapSystem = this.AddComponent<MapSystem>();
        _characterSystem = this.AddComponent<CharacterSystem>();
        _tableSystem = this.AddComponent<TableSystem>();
        
        _systems.Add(_resourceSystem);
        _systems.Add(_uiSystem);
        _systems.Add(_mapSystem);
        _systems.Add(_characterSystem);
        _systems.Add(_tableSystem);

        foreach (var system in _systems)
        {
            if (await system.Initialize() == false)
            {
                Debug.LogError($"Framework init failed! : {system.GetType()} is Failed");
                break;
            }
        }
    }

    public void GameExit()
    {
        ReleaseAll();
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    public void ReleaseAll()
    {
        foreach (var system in _systems)
        {
            system.Release();
        }
    }

    public void ReleaseMapDependency()
    {
        foreach (var system in _systems)
        {
            system.ReleaseMapDependency();
        }
    }

    private string ConvertSystemLanguageToLocaleCode(SystemLanguage language)
    {
        switch (language)
        {
            case SystemLanguage.Korean: return "ko-KR";
            case SystemLanguage.English: return "en";
            case SystemLanguage.Japanese: return "ja";
            case SystemLanguage.ChineseSimplified: return "zh";
            default: return "en";
        }
    }
    
    public void SetLanguage(SystemLanguage language)
    {
        _language = language;
        string localeCode = ConvertSystemLanguageToLocaleCode(language);
        Locale newLocale = LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier(localeCode));
        if (newLocale != null)
        {
            LocalizationSettings.SelectedLocale = newLocale;
            Debug.Log($"Changed to locale: {localeCode}");
        }
        else
        {
            Debug.LogWarning($"Locale not found for code: {localeCode}");
        }
    }
}
