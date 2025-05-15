using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UISample : UIUnit
{
    public Button ButtonTestUI;
    public Button ButtonDeleteTestUI;
    public Button ButtonMapChange;
    public Button ButtonExitGame;

    private eMapType _mapType = eMapType.Sample2;
    
    public override async UniTask Initialize()
    {
        SetDelegate();

        await UniTask.CompletedTask;
    }

    public override void Release()
    {
        ButtonTestUI.onClick.RemoveAllListeners();
        ButtonDeleteTestUI.onClick.RemoveAllListeners();
        ButtonMapChange.onClick.RemoveAllListeners();
        ButtonExitGame.onClick.RemoveAllListeners();
        
        base.Release();
    }

    #region Delegate
    private void LoadTestUI()
    {
        Framework.I.UI.LoadUI(eUIType.Test, isMapDependency: true).Forget();
    }

    private void DeleteTestUI()
    {
        Framework.I.UI.DeleteUI(eUIType.Test);
    }

    private void LoadMapChangeUI()
    {
        Framework.I.Map.LoadMap(_mapType).Forget();
        _mapType = _mapType == eMapType.Sample2 ? eMapType.Sample : eMapType.Sample2;
    }

    private void ExitGame()
    {
        Framework.I.GameExit();
    }
    #endregion
    
    private void SetDelegate()
    {
        ButtonTestUI.onClick.AddListener(LoadTestUI);
        ButtonDeleteTestUI.onClick.AddListener(DeleteTestUI);
        ButtonMapChange.onClick.AddListener(LoadMapChangeUI);
        ButtonExitGame.onClick.AddListener(ExitGame);
    }
}
