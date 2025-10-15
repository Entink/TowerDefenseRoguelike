using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial/Profile")]
public class TutorialProfile : ScriptableObject
{
    public int tutorialSeed = 123456;
    public int tutorialFightIndex = 0;

    public List<UnitId> unitWhiteList = new List<UnitId>() { UnitId.Warrior };
}