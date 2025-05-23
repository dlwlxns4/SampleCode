/// 2025-05-14

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

public enum eResourceType
{
    UI,
    Map,
    Character,
    Table,
    None,
}

public class ManageHandle
{
    public bool IsMapDependency;
    public AsyncOperationHandle Handle;

    public ManageHandle(bool isMapDependency, AsyncOperationHandle handle)
    {
        IsMapDependency = isMapDependency;
        Handle = handle;
    }
}

public class ResourceSystem : ISystem
{
    private Dictionary<eResourceType, Dictionary<string, IResourceLocation>> _resourceLocations = new Dictionary<eResourceType, Dictionary<string, IResourceLocation>>();
    private Dictionary<string, ManageHandle> _loadedAssets = new Dictionary<string, ManageHandle>();
    
    public override async UniTask<bool> Initialize()
    {
        try
        {
            await LoadLocation();

            _isInit = true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            _isInit = false;
        }

        return _isInit;
    }

    public override void Release()
    {
        _resourceLocations.Clear();
        foreach (var handle in _loadedAssets)
        {
            if (handle.Value.Handle.IsValid() == false)
                continue;

            if (handle.Value.Handle.Result is SceneInstance)
            {
                var sceneHandle = handle.Value.Handle.Convert<SceneInstance>();

                if (!sceneHandle.IsDone)
                    sceneHandle.WaitForCompletion();

                var unloadHandle = Addressables.UnloadSceneAsync(sceneHandle);
                if (!unloadHandle.IsDone)
                    unloadHandle.WaitForCompletion();

                continue;
            }

            Addressables.Release(handle.Value.Handle);
        }
        _loadedAssets.Clear();
    }

    public override void ReleaseMapDependency()
    {
        List<string> handleKeys = new List<string>();
        foreach (var handle in _loadedAssets)
        {
            if (handle.Value.IsMapDependency)
            {
                handleKeys.Add(handle.Key);
                Addressables.Release(handle.Value.Handle);
            }            
        }

        foreach (var handleKey in handleKeys)
        {
            _loadedAssets.Remove(handleKey);
        }

        Resources.UnloadUnusedAssets();
    }

    private async UniTask LoadLocation()
    {
        try
        {
            foreach (eResourceType type in Enum.GetValues(typeof(eResourceType)))
            {
                if(type == eResourceType.None)
                    continue;
                
                var locationHandle = await Addressables.LoadResourceLocationsAsync(type.ToString());

                Dictionary<string, IResourceLocation> locationDic = new Dictionary<string, IResourceLocation>();
                
                foreach (var location in locationHandle)
                {
                    string path = location.PrimaryKey;
                    int extensionIndex = path.IndexOf('.');

                    path = path.Substring(0, extensionIndex);

                    locationDic.TryAdd(path, location);
                }
                
                _resourceLocations.Add(type, locationDic);
            }
        }
        catch
        {
            Debug.Log("ResourceSystem : LoadLocation is Failed");
            throw;
        }
    }

    public void ReleaseHandle(eResourceType resourceType, string location)
    {
        var key = $"{resourceType.ToString()}/{location}";

        if (_loadedAssets.TryGetValue(key, out var manageHandle))
        {
            _loadedAssets.Remove(key);
            Addressables.Release(manageHandle.Handle);
        }
        else
        {
            Debug.Log($"ResourceSystem : ReleaseHandle key is Not Found {key}");
        }            
    }
    
    /// <summary>
    /// Object 타입만 변환 가능. 
    /// </summary>
    public async UniTask<T> LoadResource<T>(eResourceType type, string location, bool isMapDependency = false)
    {
        var loadedResourceResult = IsLoadedHandle(type, location);
        if (loadedResourceResult.Item1)
        {
            return (T)loadedResourceResult.Item2.Result;
        }
       
        var result = IsResourceContain(type, location);
        if (result.Item1)
        {
            AsyncOperationHandle<T> resourceHandle = Addressables.LoadAssetAsync<T>(result.Item2);
            
            //Loaded Handle Update
            if (_loadedAssets.ContainsKey($"{type.ToString()}/{location}") == false)
            {
                ManageHandle manageHandle = new ManageHandle(isMapDependency, resourceHandle);
                _loadedAssets.Add($"{type.ToString()}/{location}", manageHandle);
            }
            
            if (resourceHandle.IsValid())
            {
                await resourceHandle;
            }
            
            return resourceHandle.Result;
        }
        else
        {
            throw new Exception($"Resource not found: Type={type}, Location={location}");
        }    
    }

    public async UniTask<SceneInstance> LoadSceneResource(eResourceType type, string location, bool isMapDependency = false)
    {
        var loadedResourceResult = IsLoadedHandle(type, location);
        if (loadedResourceResult.Item1)
        {
            return (SceneInstance)loadedResourceResult.Item2.Result;
        }
        
       
        var result = IsResourceContain(type, location);
        if (result.Item1)
        {
            AsyncOperationHandle<SceneInstance> resourceHandle = Addressables.LoadSceneAsync(result.Item2);
            
            //Loaded Handle Update
            if (_loadedAssets.ContainsKey($"{type.ToString()}/{location}") == false)
            {
                ManageHandle manageHandle = new ManageHandle(isMapDependency, resourceHandle);
                _loadedAssets.Add($"{type.ToString()}/{location}", manageHandle);
            }
            
            if (resourceHandle.IsValid())
            {
                await resourceHandle;
            }
            
            return resourceHandle.Result;
        }
        else
        {
            return default;
        }    
    }
    
    public async UniTask<GameObject> InstantiateResourceAsync(eResourceType type, string location, bool isMapDependency, Transform parent = null)
    {
        var resultObject = await LoadResource<GameObject>(type, location, isMapDependency);
        
        if (resultObject == null)
        {
            Debug.LogError("ResourceSystem InstantiateResourceAsync is Failed!");            
            return null;
        }
        
        return Instantiate(resultObject, parent);
    }

    public async UniTask LoadHandle(eResourceType type, string location, bool isMapDependency = false)
    {
        var loadedResourceResult = IsLoadedHandle(type, location);
        if (loadedResourceResult.Item1)
        {
            return;
        }
       
        var result = IsResourceContain(type, location);
        if (result.Item1)
        {
            AsyncOperationHandle<GameObject> resourceHandle = Addressables.LoadAssetAsync<GameObject>(result.Item2);
            
            //Loaded Handle Update
            if (_loadedAssets.ContainsKey($"{type.ToString()}/{location}") == false)
            {
                ManageHandle manageHandle = new ManageHandle(isMapDependency, resourceHandle);
                _loadedAssets.Add($"{type.ToString()}/{location}", manageHandle);
            }
            
            if (resourceHandle.IsValid())
            {
                await resourceHandle;
            }
        }
        else
        {
            throw new Exception($"Resource not found: Type={type}, Location={location}");
        }    
    }
    
    public GameObject InstantiateResource(eResourceType type, string location, Transform parent = null)
    {
        var resultObject = IsResourceContain(type, location);

        if (resultObject.Item1 == false)
            return null;

        var loadedResult = IsLoadedHandle(type, location);
        if (loadedResult.Item1 == false)
            return null;

        return Instantiate((GameObject)loadedResult.Item2.Result, parent);
    }

    private (bool, AsyncOperationHandle) IsLoadedHandle(eResourceType type, string location)
    {
        string handleLocation = $"{type.ToString()}/{type.ToString()}{location}";
        
        if (_loadedAssets.TryGetValue(handleLocation, out var manageHandle))
        {
            if (manageHandle.Handle.IsValid() && manageHandle.Handle.Status == AsyncOperationStatus.Succeeded)
            {
                return (true, manageHandle.Handle);
            }
            else
            {
                return (false, manageHandle.Handle);
            }
        }
        else
        {
            return (false, default);
        }
    }
    
    private (bool, IResourceLocation) IsResourceContain(eResourceType type, string location)
    {
        string addressableLocation = $"Assets/GameDatas/{type.ToString()}/{type.ToString()}{location}";
        
        if (_resourceLocations.TryGetValue(type, out Dictionary<string, IResourceLocation> locationDic))
        {
            if (locationDic.TryGetValue(addressableLocation, out IResourceLocation resourceLocation))
            {
                return (true, resourceLocation);
            }
            else
            {
                Debug.Log($"ResourceSystem : LoadResource : {addressableLocation} is Failed");                
                return (false, null);
            }
        }
        else
        {
            Debug.Log($"ResourceSystem : LoadResource Type \"{type}\" is Failed");
            return (false, null);
        }            
    }
}
