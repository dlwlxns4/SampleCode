using System.Collections.Generic;

public class ITableContainer
{
    public virtual void Deserialize(string json)
    {
        
    }
}

public class TableListContainer<T> : ITableContainer
{
    protected List<T> _container;

    public IReadOnlyList<T> GetTable() => _container;

    public override void Deserialize(string json)
    {
        
    }
}

public class TableDicContainer<T> : ITableContainer
{
    protected Dictionary<int, T> _container;
    
    public IReadOnlyDictionary<int, T> GetTable() => _container;

    public override void Deserialize(string json)
    {
        
    }
}