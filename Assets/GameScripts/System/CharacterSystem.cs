/// 2025-05-16

using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum eCharacterType
{
    Nemu = 1,
}

public class CharacterSystem : ISystem
{
    private Dictionary<eCharacterType ,CharacterControllerBase> _characterDic = new Dictionary<eCharacterType ,CharacterControllerBase>();
    
    private GameObject _characterRoot;
    
    public override async UniTask<bool> Initialize()
    {
        _isInit = true;
        _characterRoot = new GameObject("CharacterRoot");
        _characterRoot.SetParent(gameObject);

        await UniTask.CompletedTask;
        
        return _isInit;
    }

    public override void Release()
    {
        foreach (var controller in _characterDic.Values)
        {
            controller.Release();
        }
        _characterDic.Clear();
    }

    public override void ReleaseMapDependency()
    {
        foreach (var controller in _characterDic.Values)
        {
            controller.Release();
        }
        
        _characterDic.Clear();
    }

    public async UniTask SpawnCharacter(eCharacterType characterType)
    {
        if (_characterDic.ContainsKey(characterType))
        {
            Debug.Log("Character is already spawned!");
            return;
        }
        
        var characterObject = await Framework.I.Resource.InstantiateResourceAsync(eResourceType.Character, characterType.ToString(), true, _characterRoot.transform);
        if (characterObject == null)
        {
            Debug.LogError($"CharacterSystem SpawnCharacter {characterType} Object Is NUll");
            return;
        }
        
        var controller = characterObject.GetComponent<CharacterControllerBase>();
        if (controller == null)
        {
            Debug.LogError($"CharacterSystem SpawnCharacter {characterType} Controller Is NUll");
            return;
        }

        controller.Initialize(characterType);
        _characterDic.Add(characterType, controller);        
    }

    public CharacterControllerBase GetCharacterContoller(eCharacterType characterType)
    {
        _characterDic.TryGetValue(characterType, out CharacterControllerBase character);
        
        return character;
    }

    public void RemoveCharacterController(eCharacterType characterType)
    {
        _characterDic.Remove(characterType);
    }
}
