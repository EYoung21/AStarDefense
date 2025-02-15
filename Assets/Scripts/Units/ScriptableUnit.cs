using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Scriptable Unit")]

public class ScriptableUnit : ScriptableObject
{
    public Faction Faction;
    public BaseUnit UnitPrefab;
}

public enum Faction {
    Turret = 0,
    Block = 1,
    Enemy = 2,
}