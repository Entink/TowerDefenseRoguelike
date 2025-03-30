using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public int seed;
    public int columns = 6;
    public int minNodesPerColumn = 1;
    public int maxNodesPerColumn = 3;

    [Header("Fight Settings")]
    public int totalFights = 10;

    public MapData GenerateMap()
    {
        Random.InitState(seed);

        MapData map = new MapData();
        map.seed = seed;

        int nodeId = 0;

        for (int col = 0; col < columns; col++)
        {
            int nodeCount = Random.Range(minNodesPerColumn, maxNodesPerColumn + 1);
            var nodeList = new List<MapNodeData>();

            for (int i = 0; i < nodeCount; i++)
            {
                var node = new MapNodeData();
                node.id = nodeId++;
                node.type = GetNodeType(col);
                node.fightIndex = (col * 997 + i * 571 + seed) % totalFights;

                nodeList.Add(node);
            }

            map.columns.Add(nodeList);
        }

        for (int col = 0; col < map.columns.Count - 1; col++)
        {
            foreach (var current in map.columns[col])
            {
                var nextColumn = map.columns[col + 1];

                int connections = Random.Range(1, Mathf.Min(3, nextColumn.Count + 1));
                var used = new HashSet<int>();

                for (int c = 0; c < connections; c++)
                {
                    int index;
                    do
                    {
                        index = Random.Range(0, nextColumn.Count);
                    } while (!used.Add(index));

                    current.connectedTo.Add(nextColumn[index].id);
                }
            }
        }

        return map;
    }


    private NodeType GetNodeType(int col)
    {
        if (col == 0)
        {
            return NodeType.Start;
            
        }

        if(col == columns - 1)
        {
            return NodeType.Boss;
        }

        float roll = Random.value;
        if (roll < 0.6f)
        {
            return NodeType.Fight;
            
        }

        if (roll < 0.8f)
        {
            return NodeType.Shop;
        }

        return NodeType.Event;
    }

}
