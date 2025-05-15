using System;
using UnityEngine;

public class CharacterControllerBase : MonoBehaviour
{
    private CharacterModel _model;
    private CharacterView _view;

    public void Initialize(eCharacterType characterType)
    {
        _model = new CharacterModel();
        _model.CharacterType = characterType;
        
        _view = this.GetComponent<CharacterView>();
        
        _view.RegisterAnimationCallback(eAnimationType.idle, IdleCallback);   
        _view.RegisterAnimationCallback(eAnimationType.defaultattack, DefaultAttackCallback);   
        _view.RegisterAnimationCallback(eAnimationType.run, RunCallback);   
    }

    public void PlayAnimation(eAnimationType animationType)
    {
        _view.PlayAnimation(animationType);
    }
    
    public void Release()
    {
        _view.Release();
        
        Destroy(gameObject);
    }
    
    #region AniamtionCallback

    protected virtual void IdleCallback()
    {
        Debug.Log($"{_model.CharacterType} is Completed Idle Action.");
    }

    protected virtual void DefaultAttackCallback()
    {
        Debug.Log($"{_model.CharacterType} is Completed DefaultAttack Action.");
    }

    protected virtual void RunCallback()
    {
        Debug.Log($"{_model.CharacterType} is Completed Walk Action.");
    }

    #endregion
}
