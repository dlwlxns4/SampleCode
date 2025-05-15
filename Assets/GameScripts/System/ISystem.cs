/// 2025-05-14


using Cysharp.Threading.Tasks;
using UnityEngine;

public class ISystem : MonoBehaviour
{
    protected bool _isInit = false; 
    
    public virtual async UniTask<bool> Initialize()
    {
        await UniTask.CompletedTask;
        
        return _isInit;
    }

    public virtual void Release()
    {
        
    }

    public virtual void ReleaseMapDependency()
    {
        
    }
}
