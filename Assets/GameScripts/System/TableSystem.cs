/// 2025-05-16

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum eTableType
{
    CharacterData,
}

public class TableSystem : ISystem
{
    private Dictionary<eTableType, ITableContainer> _tableDic = new Dictionary<eTableType, ITableContainer>();
    
    public override async UniTask<bool> Initialize()
    {
        try
        {
            _tableDic[eTableType.CharacterData] = new TableCharacterContainer();
            
            await LoadTable();
            _isInit = true;

            return _isInit;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
    }

    public async UniTask LoadTable()
    {
        foreach (eTableType type in Enum.GetValues(typeof(eTableType)))
        {
            var tablePack = await Framework.I.Resource.LoadResource<TextAsset>(eResourceType.Table, type.ToString());

            if (_tableDic.TryGetValue(type, out ITableContainer tableContainer) == false)
            {
                Debug.LogError($"Table {type} Container is not found!");
                return;
            }
            
            if (tablePack != null)
            {
                tableContainer.Deserialize(tablePack.text);
            }
            else
            {
                Debug.LogError($"Table {type} Data is not found!");
            }
        }
    }

    public ITableContainer GetTableContainer(eTableType tableType)
    {
        if (_tableDic.TryGetValue(tableType, out ITableContainer tableContainer))
        {
            return tableContainer;
        }

        return null;
    }
}
