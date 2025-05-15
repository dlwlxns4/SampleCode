/// 2025-05-14 

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T I => _instance;
    private static T _instance = null;
    
    public virtual void Awake()
    {
        if(_instance == null)
        {
            _instance = this as T;
            Init();
            DontDestroyOnLoad(this);
        }
        else
        {
        }
    }

    public abstract UniTask Init();
}


public class Framework : Singleton<Framework>
{
    public UISystem UI => _uiSystem;
    public ResourceSystem Resource => _resourceSystem;
    public MapSystem Map => _mapSystem;
    
    private UISystem _uiSystem;
    private ResourceSystem _resourceSystem;
    private MapSystem _mapSystem;
    
    private List<ISystem> _systems = new List<ISystem>();
    
    public override async UniTask Init()
    {
        _resourceSystem = this.AddComponent<ResourceSystem>();
        _uiSystem = this.AddComponent<UISystem>();
        _mapSystem = this.AddComponent<MapSystem>();
        
        _systems.Add(_resourceSystem);
        _systems.Add(_uiSystem);
        _systems.Add(_mapSystem);
        
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

    private void OnDestroy()
    {
        ReleaseAll();
    }
}
