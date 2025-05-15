using Cysharp.Threading.Tasks;

public class MapSample : MapUnit
{
    public override async UniTask Initialize()
    {
        await Framework.I.UI.LoadUI(eUIType.Sample);
    }
}
