/// 2025-05-14

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITest : UIUnit
{
    public Button ButtonLocalize;
    public Button ButtonSpawnCharacter;
    public Button ButtonIdle;
    public Button ButtonRun;
    public Button ButtonDefaultAttack;

    public GameObject GOAnimationPanel;
    
    public RawImage RawImageCharacter;
    public Camera CameraCharacter;
    
    public TextMeshProUGUI TMPLocalize;
    
    private List<SystemLanguage> _languages = new List<SystemLanguage>() { SystemLanguage.Korean, SystemLanguage.Japanese, SystemLanguage.English };
    
    private SystemLanguage _currentLanguage = SystemLanguage.Korean;
    private int _currentLanguageIndex = -1;
    private RenderTexture _renderTexture;
    
    public override UniTask Initialize()
    {
        SetDelegate();
        
        _currentLanguage = Framework.I.Language;
        int foundIndex = _languages.IndexOf(_currentLanguage);
        _currentLanguageIndex = foundIndex >= 0 ? foundIndex : 0;

        // RenderTexture 생성
        Vector2 size = RawImageCharacter.rectTransform.rect.size;
        int width = Mathf.RoundToInt(size.x);
        int height = Mathf.RoundToInt(size.y);
        _renderTexture = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32);
        _renderTexture.Create();

        
        return UniTask.CompletedTask;
    }
    
    public override void Release()
    {
        ButtonLocalize.onClick.RemoveAllListeners();
        ButtonSpawnCharacter.onClick.RemoveAllListeners();

        CameraCharacter.targetTexture = null;
        
        _renderTexture.Release();
        Destroy(_renderTexture);
        
        base.Release();
    }

    #region Delegate
    private void ChangeLanguage()
    {
        _currentLanguageIndex = (_currentLanguageIndex + 1) % _languages.Count;
        _currentLanguage = _languages[_currentLanguageIndex];

        // 예시: 외부 시스템에도 적용
        Framework.I.SetLanguage(_currentLanguage);
        
        TMPLocalize.SetText(_currentLanguage.ToString());
    }

    private void SpawnCharacter()
    {
        SpawnCharacterAsync().Forget();
    }

    private async UniTask SpawnCharacterAsync()
    {
        await Framework.I.Character.SpawnCharacter(eCharacterType.Nemu);
        var controller = Framework.I.Character.GetCharacterContoller(eCharacterType.Nemu);
        if (controller != null)
        {
            // 캐릭터 앞 방향으로 위치 계산
            Vector3 forward = controller.transform.forward;
            Vector3 cameraPos = controller.transform.position + forward * - 10f + Vector3.up * 2f;

            // 카메라 위치 & 회전
            CameraCharacter.transform.position = cameraPos;
            CameraCharacter.transform.LookAt(controller.transform.position); // 시선도 위쪽 보정
            
            GOAnimationPanel.SetActive(true);
        }
        
        CameraCharacter.targetTexture = _renderTexture;
        RawImageCharacter.texture = _renderTexture;
    }

    private void IdleCharacter()
    {
        var controller = Framework.I.Character.GetCharacterContoller(eCharacterType.Nemu);
        controller.PlayAnimation(eAnimationType.idle);
    }

    private void RunCharacter()
    {
        var controller = Framework.I.Character.GetCharacterContoller(eCharacterType.Nemu);
        controller.PlayAnimation(eAnimationType.run);
    }
    
    private void DefaultAttackCharacter()
    {
        var controller = Framework.I.Character.GetCharacterContoller(eCharacterType.Nemu);
        controller.PlayAnimation(eAnimationType.defaultattack);
    }
    #endregion

    private void SetDelegate()
    {
        ButtonLocalize.onClick.AddListener(ChangeLanguage);
        ButtonSpawnCharacter.onClick.AddListener(SpawnCharacter);
        ButtonIdle.onClick.AddListener(IdleCharacter);
        ButtonRun.onClick.AddListener(RunCharacter);
        ButtonDefaultAttack.onClick.AddListener(DefaultAttackCharacter);
    }
}
