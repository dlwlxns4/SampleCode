/// 2025-05-14

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum eUIType
{
    Sample,
    Test,
}

public class UISystem : ISystem
{
    private GameObject _uiRoot;

    private List<UIUnit> _uiList = new List<UIUnit>();
    private List<UIUnit> _mapDepencencyUIList = new List<UIUnit>();
    
    public override async UniTask<bool> Initialize()
    {
        SetCanvas();

        _isInit = true;

        await UniTask.CompletedTask;
        
        return _isInit;
    }

    public override void Release()
    {
        foreach (var uiUnit in _uiList)
        {
            uiUnit.Release();
        }
    }

    public override void ReleaseMapDependency()
    {
        foreach (var uiUnit in _mapDepencencyUIList)
        {
            uiUnit.Release();
        }

        _mapDepencencyUIList.Clear();
    }

    public async UniTask<UIUnit> LoadUI(eUIType uiType, bool isMapDependency = false, bool isMultiple = false)
    {
        if (isMultiple == false)
        {
            var ui = GetUI(uiType);
            if (ui != null)
            {
                Debug.Log($"UISystem LoadUI type : {uiType} is Already Exist.");
                return ui;
            }
        }
        
        var uiObject = await Framework.I.Resource.InstantiateResourceAsync(eResourceType.UI, uiType.ToString(), _uiRoot.transform);
        
        var uiUnit = uiObject.GetComponent<UIUnit>();
        if (uiUnit != null)
        {
            await uiUnit.Initialize();
            
            if(isMapDependency)
            {
                _mapDepencencyUIList.Add(uiUnit);
            }
            else
            {
                _uiList.Add(uiUnit);
            }
        }
        else
        {
            Debug.LogError($"uiUnit is Null! Type : {uiType}");
        }

        return uiUnit;
    }

    public UIUnit GetUI(eUIType uiType)
    {
        UIUnit uiUnit = null;
        foreach (var ui in _uiList)
        {
            if (ui.UIType == uiType)
            {
                uiUnit = ui;
                break;
            }
        }

        foreach (var ui in _mapDepencencyUIList)
        {
            if (ui.UIType == uiType)
            {
                uiUnit = ui;
                break;
            }
        }
        
        return uiUnit;
    }
    
    public void DeleteUI(eUIType uiType)
    {
        foreach (var uiUnit in _uiList)
        {
            if(uiUnit.UIType != uiType)
                continue;
            
            uiUnit.Release();
        }
        
        foreach (var uiUnit in _mapDepencencyUIList)
        {
            if(uiUnit.UIType != uiType)
                continue;
            
            uiUnit.Release();
        }
        
        _uiList.RemoveAll(x=>x.UIType == uiType);
        _mapDepencencyUIList.RemoveAll(x=>x.UIType == uiType);
    }

    private void SetCanvas()
    {
        _uiRoot = new GameObject();
        _uiRoot.name = "UIRoot";
        _uiRoot.SetParent(this);
        
        var canvas = _uiRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<GraphicRaycaster>();
    }
}
