/// 2025-05-14

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum eUIType
{
    Sample,
    Test,
}

public class UISystem : ISystem
{
    private GameObject _uiRoot;

    private List<UIUnit> _uiList = new List<UIUnit>();
    
    public override async UniTask<bool> Initialize()
    {
        SetCanvas();

        _isInit = true;
        
        return _isInit;
    }

    public async UniTask<UIUnit> LoadUI(eUIType uiType, bool isAutoRelease = true)
    {
        var uiObject = await Framework.I.Resource.InstantiateResourceAsync(eResourceType.UI, uiType.ToString(), _uiRoot.transform, isAutoRelease);
        
        var uiUnit = uiObject.GetComponent<UIUnit>();
        if (uiUnit != null)
        {
            await uiUnit.Initialize();
            _uiList.Add(uiUnit);
        }
        else
        {
            Debug.LogError($"uiUnit is Null! Type : {uiType}");
        }

        return uiUnit;
    }

    private void SetCanvas()
    {
        _uiRoot = new GameObject();
        _uiRoot.name = "UIRoot";
        _uiRoot.SetParent(this);
        
        var canvas = _uiRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;        
    }
}
