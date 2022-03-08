using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
public class Set_Prefab : EditorWindow
{

    private Vector3 pos;
    private GameObject prefab;
    private List<GameObject> list;
    private string objName;

    [MenuItem("GameObject/SetPrefab")]
    static void init()
    {
        EditorWindow.GetWindow<Set_Prefab>("Set_Prefab");
    }

    public GameObject[] objects;
    private void OnGUI()
    {
        prefab = EditorGUILayout.ObjectField("object", prefab, typeof(GameObject), true) as GameObject;


        if (GUILayout.Button("SetList"))
        {
            objects = SelectedObject.Create();
            if (objects[0].transform.parent.parent.name == "City")
            {
                Harbor c = prefab.GetComponent<Harbor>();
                foreach (var obj in objects)
                {
                    c.aroundCity.Add(obj.GetComponent<BordObject>());
                }
            }
        }

        objName = EditorGUILayout.TextField("ObjectName",objName);

        if (GUILayout.Button("ChangeName"))
        {
            objects = SelectedObject.Create();
            foreach (var obj in objects)
            {
                obj.name = objName;
            }
        }
    }
}
public class SelectedObject : Editor
{
    public static GameObject[] Create()
    {
        return Selection.gameObjects;
    }
}