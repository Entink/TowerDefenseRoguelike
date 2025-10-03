using System.Collections.Generic;

[System.Serializable]

public class MapNodeData
{
    public int id;
    public NodeType type;
    public List<int> connectedTo = new List<int>();
    public int fightIndex;

    public bool wasVisisted = false;

    public int columnIndex;
}