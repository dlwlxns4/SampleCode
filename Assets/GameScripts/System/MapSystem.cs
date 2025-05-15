using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum eMapType
{
    Sample,
    Sample2,
}


public class MapSystem : ISystem
{
    private MapUnit _currentMap;
    
    public override async UniTask<bool> Initialize()
    {
        await LoadMap(eMapType.Sample);
        
        _isInit = true;
        
        return _isInit;
    }

    public async UniTask LoadMap(eMapType mapType)
    {
        if (_currentMap != null)
        {
            _currentMap.Release();
        }
        
        var sceneInstance = await Framework.I.Resource.LoadSceneResource(eResourceType.Map, mapType.ToString());
        if (sceneInstance.Scene.IsValid() == false)
        {
            Debug.LogError($"Scene {mapType} is not Valid");
            return;
        }
        
        List<GameObject> rootObjects = new List<GameObject>();
        sceneInstance.Scene.GetRootGameObjects(rootObjects);

        foreach (GameObject obj in rootObjects)
        {
            MapUnit mapUnit = obj.GetComponentInChildren<MapUnit>(true);
            if (mapUnit != null)
            {
                _currentMap = mapUnit;
                break;
            }
        }
        
        Framework.I.ReleaseMapDependency();
        
        await _currentMap.Initialize();
    }
}
