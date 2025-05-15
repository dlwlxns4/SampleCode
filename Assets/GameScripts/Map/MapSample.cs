using Cysharp.Threading.Tasks;
using UnityEngine;

public class MapSample : MapUnit
{
    public override async UniTask Initialize()
    {
        await Framework.I.UI.LoadUI(eUIType.Sample);
    }
}
