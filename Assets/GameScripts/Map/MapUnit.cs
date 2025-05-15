using Cysharp.Threading.Tasks;
using UnityEngine;

public enum eMapState
{
    None,
    Loading,
    Stop,
    Release,
    Done,
}

public class MapUnit : MonoBehaviour
{
    public Camera MapCamera;
    
    public virtual UniTask Initialize()
    {
        return UniTask.CompletedTask;
    }

    public virtual void SettingCamera()
    {
        
    }
    
    public virtual void Release()
    {

    }
}
