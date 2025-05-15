/// 2025-05-16

using System;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;

public enum eAnimationType
{
    idle,
    run,
    defaultattack,
}

public class CharacterView : MonoBehaviour
{
    public SkeletonAnimation _SkeletonAnimation;

    private Dictionary<eAnimationType, Action> _animationCallbacks = new();

    public void Release()
    {
        
    }
    
    public void PlayAnimation(eAnimationType type, bool loop = false)
    {
        string animName = type.ToString(); 
        var entry = _SkeletonAnimation.AnimationState.SetAnimation(0, animName, loop);

        if (entry != null && _animationCallbacks.TryGetValue(type, out var callback))
        {
            entry.Complete += (TrackEntry trackEntry) =>
            {
                if (callback != null)
                    callback.Invoke();
            };
        }
    }

    public void RegisterAnimationCallback(eAnimationType type, Action callback)
    {
        _animationCallbacks.Add(type, callback);
    }
}
