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

            int nodeCount = (col == 0 || col == columns - 1)
                ? 1
                : Random.Range(minNodesPerColumn, maxNodesPerColumn + 1);
            

            for (int i = 0; i < nodeCount; i++)
            {
                var node = new MapNodeData();
                node.id = nodeId++;
                node.columnIndex = col;

                if(col == 0)
                {
                    node.type = NodeType.Start;
                }
                else if (col == columns - 1)
                {
                    node.type = NodeType.Boss;
                    node.fightIndex = -1;
                }
                else
                {
                    node.type = GetNodeType(col);

                    if (node.type == NodeType.Fight)
                    {
                        if (node.type == NodeType.Fight)
                            node.fightIndex = -1;
                    }
                }

                nodeList.Add(node);

            }


            map.columns.Add(nodeList);
            
        }

        



        List<MapNodeData> allMiddleNodes = new List<MapNodeData>();
        for (int i = 1; i < columns - 1; i++)
            allMiddleNodes.AddRange(map.columns[i]);


        if(!allMiddleNodes.Exists(n => n.type == NodeType.Shop))
        {
            var randomNode = allMiddleNodes[Random.Range(0, allMiddleNodes.Count)];
            randomNode.type = NodeType.Shop;
        }

        if (!allMiddleNodes.Exists(n => n.type == NodeType.Event))
        {
            var randomNode = allMiddleNodes[Random.Range(0, allMiddleNodes.Count)];
            randomNode.type = NodeType.Event;
        }

        


        for(int col = 0; col < map.columns.Count - 1; col++)
        {
            var currentColumn = map.columns[col];
            var nextColumn = map.columns[col + 1];

            for (int currentIndex = 0; currentIndex < currentColumn.Count; currentIndex++)
            {
                var current = currentColumn[currentIndex];

                List<int> candidateIndices = GetNeighborCandidateIndices(currentIndex, currentColumn.Count, nextColumn.Count);

                int connections = Random.Range(1, Mathf.Min(3, candidateIndices.Count) + 1);

                var shuffled = candidateIndices.OrderBy(_ => Random.value).ToList();

                for(int i = 0; i < connections; i++)
                {
                    int targetIndex = shuffled[i];
                    current.connectedTo.Add(nextColumn[targetIndex].id);
                }

            }

            for(int targetIndex = 0; targetIndex < nextColumn.Count; targetIndex++)
            {
                int targetId = nextColumn[targetIndex].id;
                bool isConnected = currentColumn.Any(node => node.connectedTo.Contains(targetId));

                if(!isConnected)
                {
                    int closestSourceIndex = GetClosestSourceIndex(targetIndex, currentColumn.Count, nextColumn.Count);
                    currentColumn[closestSourceIndex].connectedTo.Add(targetId);
                }
            }
        }

        return map;

        
    }

    private NodeType GetNodeType(int col)
    {
        if (col == 0) return NodeType.Start;
        if (col == columns - 1) return NodeType.Boss;

        float roll = Random.value;
        if (roll < 0.75f) return NodeType.Fight;
        if (roll < 0.85f) return NodeType.Shop;
        return NodeType.Event;
    }

    private List<int> GetNeighborCandidateIndices(int currentIndex, int currentCount, int nextCount)
    {
        int mappedIndex = MapIndexBetweenColumns(currentIndex, currentCount, nextCount);

        HashSet<int> result = new HashSet<int>();

        for(int offset = -1; offset <= 1; offset++)
        {
            int candidate = mappedIndex + offset;
            if (candidate >= 0 && candidate < nextCount)
                result.Add(candidate);
        }

        if (result.Count == 0)
            result.Add(Mathf.Clamp(mappedIndex, 0, nextCount - 1));

        return result.OrderBy(i => i).ToList();
    }

    private int MapIndexBetweenColumns(int index, int fromCount, int toCount)
    {
        if (fromCount <= 1)
            return 0;

        float t = index / (float)(fromCount - 1);
        return Mathf.RoundToInt(t * (toCount - 1));
    }

    private int GetClosestSourceIndex(int targetIndex, int currentCount, int nextCount)
    {
        int bestIndex = 0;
        float bestDistance = float.MaxValue;

        for(int currentIndex = 0; currentIndex < currentCount; currentIndex++)
        {
            int mappedIndex = MapIndexBetweenColumns(currentIndex, currentCount, nextCount);
            float distance = Mathf.Abs(mappedIndex - targetIndex);

            if(distance < bestDistance)
            {
                bestDistance = distance;
                bestIndex = currentIndex;
            }

        }

        return bestIndex;

    }
    

}
