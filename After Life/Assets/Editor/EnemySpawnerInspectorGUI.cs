using UnityEditor;

[CustomEditor(typeof(EnemySpawner))]
public class EnemySpawnerInspectorGUI : Editor
{
    // The various categories the editor will display the variables in 
    public enum DisplayCategory
    {
        VS757, BLANK1, BLANK2
    }

    // The enum field that will determine what variables to display in the Inspector
    public DisplayCategory categoryToDisplay;

    // The function that makes the custom editor work
    public override void OnInspectorGUI()
    {
        // Display the enum popup in the inspector
        categoryToDisplay = (DisplayCategory) EditorGUILayout.EnumPopup("Display", categoryToDisplay);

        // Create a space to separate this enum popup from other variables 
        EditorGUILayout.Space(); 
        
        // Switch statement to handle what happens for each category
        switch (categoryToDisplay)
        {
            case DisplayCategory.VS757:
                DisplayVS757Info(); 
                break;

            case DisplayCategory.BLANK1:
                DisplayBLANK1Info();
                break;

            case DisplayCategory.BLANK2:
                DisplayBLANK2Info();
                break; 

        }
        serializedObject.ApplyModifiedProperties();
    }

    // When the categoryToDisplay enum is at "VS757"
    void DisplayVS757Info()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("shouldSpawnVS757"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("vs757"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("vs757SpawnPoints"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("vsController"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("vfxController"));
    }

    // When the categoryToDisplay enum is at "BLANK1"
    void DisplayBLANK1Info()
    {
    }

    // When the categoryToDisplay enum is at "BLANK2"
    void DisplayBLANK2Info()
    {
    }
}
