using System.Collections.Generic;
using Newtonsoft.Json;

public class MessageData
{
}

public class TableMessageContainer : TableListContainer<MessageData>
{
    public override void Deserialize(string json)
    {
        var result = JsonConvert.DeserializeObject<List<MessageData>>(json);
    }
}
