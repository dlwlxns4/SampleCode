using System.Collections.Generic;
using Newtonsoft.Json;

public class CharacterData
{
    public int ID { get; set; }

    public int HP { get; set; }

    public int SP { get; set; }

    public int ATTACK { get; set; }

    public int DEFENSE { get; set; }

}

public class TableCharacterContainer : TableListContainer<CharacterData>
{
    public override void Deserialize(string json)
    {
        var result = JsonConvert.DeserializeObject<List<CharacterData>>(json);
        _container = result;
    }

    public CharacterData GetData(int id)
    {
        return _container.Find(x=>x.ID == id);
    }
}
