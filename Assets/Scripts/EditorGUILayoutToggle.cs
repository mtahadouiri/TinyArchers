using UnityEngine;
using UnityEditor;

public class EditorGUILayoutToggle : UnityEditor.EditorWindow
{
    bool road1 ;
    bool road2 ;
    bool road3 ;

    [MenuItem("Editor/Road selector")]
    static void Init()
    {
        EditorGUILayoutToggle window = (EditorGUILayoutToggle)EditorWindow.GetWindow(typeof(EditorGUILayoutToggle), true, "Selector");

        window.Show();
    }

    void Awake()
    {
        road1 = PlayerPrefs.GetInt("road1") != 0;
        road2 = PlayerPrefs.GetInt("road2") != 0;
        road3 = PlayerPrefs.GetInt("road3") != 0;
    }

    void OnGUI()
    {
        EditorGUILayout.TextArea("Roads are arranged from top top to bottom in the map.");

        road1 = EditorGUILayout.Toggle("Road 1", road1);

        road2 = EditorGUILayout.Toggle("Road 2", road2);

        road3 = EditorGUILayout.Toggle("Road 3", road3);


        if (GUILayout.Button("Save"))
        {
            if (road1)
                PlayerPrefs.SetInt("road1", 1);
            else
                PlayerPrefs.SetInt("road1", 0);

            if (road2)
                PlayerPrefs.SetInt("road2", 1);
            else
                PlayerPrefs.SetInt("road2", 0);

            if (road3)
                PlayerPrefs.SetInt("road3", 1);
            else
                PlayerPrefs.SetInt("road3", 0);
            this.Close();
        }
    }
}