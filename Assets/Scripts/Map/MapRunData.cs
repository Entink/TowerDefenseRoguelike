public static class MapRunData
{
    public static MapData currentMap;
    public static int currentSeed;
    public static MapNodeData currentNode;

    public static void Reset()
    {
        currentMap = null;
        currentSeed = 0;
        currentNode = null;
    }


}