/// 2025-05-14

using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITest : UIUnit
{
    public Button ButtonLocalize;
    public TextMeshProUGUI TMPLocalize;
    
    private List<SystemLanguage> _languages = new List<SystemLanguage>() { SystemLanguage.Korean, SystemLanguage.Japanese, SystemLanguage.English };
    
    private SystemLanguage _currentLanguage = SystemLanguage.Korean;
    private int _currentLanguageIndex = -1;
    
    public override UniTask Initialize()
    {
        SetDelegate();
        
        _currentLanguage = Framework.I.Language;
        int foundIndex = _languages.IndexOf(_currentLanguage);
        _currentLanguageIndex = foundIndex >= 0 ? foundIndex : 0;
        
        return UniTask.CompletedTask;
    }

    public void ChangeLanguage()
    {
        _currentLanguageIndex = (_currentLanguageIndex + 1) % _languages.Count;
        _currentLanguage = _languages[_currentLanguageIndex];

        // 예시: 외부 시스템에도 적용
        Framework.I.SetLanguage(_currentLanguage);
        
        TMPLocalize.SetText(_currentLanguage.ToString());
    }
    
    public override void Release()
    {
        ButtonLocalize.onClick.RemoveAllListeners();
        
        base.Release();
    }

    private void SetDelegate()
    {
        ButtonLocalize.onClick.AddListener(ChangeLanguage);
    }
}
