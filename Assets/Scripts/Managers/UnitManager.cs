using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    void Awake() {
        Instance = this;
    }
}
