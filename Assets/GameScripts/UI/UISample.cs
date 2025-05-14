using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UISample : UIUnit
{
    public Button ButtonTestUI;
    public Button ButtonDeleteTestUI;
    public Button ButtonMapChange;
    public Button ButtonExitGame;


    public override void Release()
    {
        ButtonTestUI.onClick.RemoveAllListeners();
    }

    #region MyRegion

    private void LoadTestUI()
    {
        Framework.I.UI.LoadUI(eUIType.Test).Forget();
    }
        
    #endregion
    
    private void SetDelegate()
    {
        ButtonTestUI.onClick.AddListener(LoadTestUI);
    }
}
