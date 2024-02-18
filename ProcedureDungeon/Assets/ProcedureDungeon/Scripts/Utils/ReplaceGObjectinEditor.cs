using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ReplaceGObjectinEditor : EditorWindow
{
    public GameObject prefabIntercambio;
    
    [MenuItem("Tools/Replace Object for Other")]
    private static void Replace()
    {
        // Obtén el cubo seleccionado en el Hierarchy
        GameObject selectedObject = Selection.activeGameObject;

        //if (selectedObject != null && selectedObject.GetComponent<MeshFilter>() != null && selectedObject.GetComponent<MeshFilter>().sharedMesh.name == "Cube")
        if (selectedObject != null)
        {
            GameObject[] gameObjects = ChildrenController.GetChildren(selectedObject);
            for(int i = 0; i < gameObjects.Length; i++)
            {
                Change(gameObjects[i]);
            }                        
        }
        else
        {
            Debug.LogWarning("Please select a cube GameObject to replace.");
        }
    }
    static void Change(GameObject go)
    {
        GameObject spherePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ProcedureDungeon/Prefabs/Dungeon/_Partes/Techo.prefab");
        GameObject newSphere = PrefabUtility.InstantiatePrefab(spherePrefab) as GameObject;

        // Copia la transformación del cubo al objeto de la esfera
        newSphere.transform.position = go.transform.position;
        newSphere.transform.rotation = go.transform.rotation;
        newSphere.transform.localScale = go.transform.localScale;
        newSphere.transform.parent = go.transform.parent;

        // Destruye el cubo original
        DestroyImmediate(go);

        Debug.Log("Cube replaced with sphere.");
    }
}
