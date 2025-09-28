using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public int seed;
    public int columns = 6;
    public int minNodesPerColumn = 2;
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
            var nodeList = new List<MapNodeData>();

            int nodeCount;

            if (col == 0 || col == columns - 1)
            {
                nodeCount = 1;
            }
            else
            {
                nodeCount = Random.Range(minNodesPerColumn, maxNodesPerColumn + 1);
            }

            for (int i = 0; i < nodeCount; i++)
            {
                var node = new MapNodeData();
                node.id = nodeId++;

                if(col == 0)
                {
                    node.type = NodeType.Start;
                }
                else if (col == columns - 1)
                {
                    node.type = NodeType.Boss;
                }
                else
                {
                    node.type = GetNodeType(col);
                }

                if(node.type == NodeType.Fight || node.type == NodeType.Boss)
                {
                    node.fightIndex = (col * 997 + i * 571 + seed) % totalFights;
                }

                nodeList.Add(node);
            }

            map.columns.Add(nodeList);
            
        }

        if(map.columns.Count > 1)
        {
            foreach (var node in map.columns[1])
            {
                node.type = NodeType.Fight;
            }
        }

        

        //Zliczanie wszystkich nodeów poza Start i Boss
        List<MapNodeData> allMiddleNodes = new List<MapNodeData>();
        for (int i = 1; i < columns - 1; i++)
        {
            allMiddleNodes.AddRange(map.columns[i]);
        }

        //Wymuszenie obecnoœci sklepu
        if(!allMiddleNodes.Exists(n => n.type == NodeType.Shop))
        {
            var randomNode = allMiddleNodes[Random.Range(0, allMiddleNodes.Count)];
            randomNode.type = NodeType.Shop;
        }

        //Wymuszenie obecnoœci Eventu
        if(!allMiddleNodes.Exists(n => n.type == NodeType.Event))
        {
            var randomNode = allMiddleNodes[Random.Range(0, allMiddleNodes.Count)];
            randomNode.type = NodeType.Event;
        }

        //Wymuszenie obecnosci min. columns-2 walk
        int requiredFights = columns - 2;
        int currentFights = allMiddleNodes.Count(n => n.type == NodeType.Fight);

        while(currentFights < requiredFights)
        {
            var candidate = allMiddleNodes.Find(n => n.type != NodeType.Fight && n.type != NodeType.Shop && n.type != NodeType.Event);
            if (candidate == null)
            {
                break;
            }
            candidate.type = NodeType.Fight;
            currentFights++;
        }



        for (int col = 0; col < map.columns.Count - 1; col++)
        {
            var currentColumn = map.columns[col];
            var nextColumn = map.columns[col + 1];

            foreach(var current in currentColumn)
            {
                int connections = Random.Range(1, Mathf.Min(3, nextColumn.Count + 1));
                var used = new HashSet<int>();

                for (int i = 0; i < connections; i++)
                {
                    int index;
                    do
                    {
                        index = Random.Range(0, nextColumn.Count);
                    } while (!used.Add(index));

                    current.connectedTo.Add(nextColumn[index].id);
                }
            }

            foreach(var target in nextColumn)
            {
                bool isConnected = currentColumn.Any(node => node.connectedTo.Contains(target.id));
                if(!isConnected)
                {
                    var randomSource = currentColumn[Random.Range(0, currentColumn.Count)];
                    randomSource.connectedTo.Add(target.id);
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
        if (roll < 0.75f)
        {
            return NodeType.Fight;
            
        }

        if (roll < 0.85f)
        {
            return NodeType.Shop;
        }

        return NodeType.Event;
    }

}
