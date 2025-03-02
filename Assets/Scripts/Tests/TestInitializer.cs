using UnityEngine;

//this script will automatically create a GameObject with the TestManager component
//when the scene starts
[DefaultExecutionOrder(-100)] //make sure this runs before other scripts
public class TestInitializer : MonoBehaviour
{
    private static bool initialized = false;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (initialized) return;
        
        Debug.Log("TestInitializer - Creating test components");
        
        //create a GameObject to hold our test components
        GameObject testObject = new GameObject("_TestManager");
        DontDestroyOnLoad(testObject);
        
        //add the TestManager component
        testObject.AddComponent<TestManager>();
        
        initialized = true;
    }
} 