using System.Collections.Generic;

[System.Serializable]

public class MapData
{
    public int seed;
    public List<List<MapNodeData>> columns = new List<List<MapNodeData>>();
}