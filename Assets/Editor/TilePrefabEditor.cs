using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
public class TilePrefabEditor : EditorWindow
{
    private static TilePrefabScriptableObject m_Data;
    private static List<GameObject> m_Prefabs;
    private static int m_Count;
    private static string[] m_EnumArray;
    private const string path = "Assets/Resources/ScriptableObjects/";
    private const string objectName = "TilePrefabs.asset";

    [MenuItem("CustomEditor/TilePrefabManager")]
    static void ShowWindow()
    {
        TilePrefabEditor currentWindow = (TilePrefabEditor)GetWindow(typeof(TilePrefabEditor));
        currentWindow.Show();
        m_EnumArray = System.Enum.GetNames(typeof(TileType));
        m_Count = m_EnumArray.Length;
    }

    private static void LoadScriptableObject()
    {

        m_Data = AssetDatabase.LoadAssetAtPath<TilePrefabScriptableObject>(path + objectName);
        if (!m_Data)
        {
            m_Data = CreateInstance<TilePrefabScriptableObject>();
            m_Data.m_TileTypes = new List<TileType>();
            m_Data.m_GameObjects = new List<GameObject>();
            AssetDatabase.CreateAsset(m_Data, path + objectName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    private void OnEnable()
    {
        LoadScriptableObject();
        m_Prefabs = m_Data.m_GameObjects;
    }

    private void OnGUI()
    {
        for(int i = 0; i < m_Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(m_EnumArray[i]));
            m_Prefabs[i] = EditorGUILayout.ObjectField(m_Prefabs[i], typeof(GameObject), false) as GameObject;
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button(new GUIContent("Apply")) && m_Data)
        {
            m_Data.m_GameObjects.Clear();
            m_Data.m_TileTypes.Clear();
            for (int i = 0; i < m_Prefabs.Count; i++)
            {
                if (m_Prefabs[i] != null)
                {
                    m_Data.m_TileTypes.Add((TileType)System.Enum.Parse(typeof(TileType), m_EnumArray[i]));
                    m_Data.m_GameObjects.Add(m_Prefabs[i]);
                }
            }
            EditorUtility.SetDirty(m_Data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}