using UnityEngine;

[CreateAssetMenu(menuName = "Map/Event", fileName = "Event_")]
public class MapEventDef : ScriptableObject
{
    public string eventId;
    public bool hideAfterResolve = true;

    [TextArea] public string title;
    [TextArea] public string body;
    public Sprite illustration;

    [Range(0, 1)] public float commonness = 1f;
    public int minColumn = 0;
    public int maxColumn = 999;

    public EventOption[] options;

}

[System.Serializable]
public class EventOption
{
    public string label;
    [TextArea] public string description;
    public Sprite icon;

    public int minMaterials;
    public int minCash;

    public int deltaMaterials;
    public int deltaCash;

    public bool addModifier;
    public RunModifierId modifier;
    public int modifierStacks = 1;

    public bool unlocksUnit = false;
    public UnitId unitToUnlock;

    public bool resolveEventOnPick = false;
    public string resolveFlagOverride;

    [TextArea] public string resultText;
    public Sprite resultIllustration;
}