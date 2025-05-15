/// 2025-05-14

using Cysharp.Threading.Tasks;
using UnityEngine;

public class UIUnit : MonoBehaviour
{
    public eUIType UIType;
    
    public virtual async UniTask Initialize()
    {
        await UniTask.CompletedTask;
    }

    public virtual void Release()
    {
        if (gameObject == null)
            return;
        
        Destroy(gameObject);
    }
}
