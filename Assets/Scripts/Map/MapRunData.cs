public static class MapRunData
{
    
    public static MapData currentMap;
    public static int currentSeed;
    public static MapNodeData currentNode;
    public static int pendingNodeId = -1;
    public static int nodeToMarkVisited = -1;

    public static int currentAct = 1;

    public static void Reset()
    {
        currentMap = null;
        currentSeed = 0;
        currentNode = null;
    }


}