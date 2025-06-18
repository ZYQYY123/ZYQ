using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Codice.CM.Common;
using System;


[Serializable]
public class ObjectInfo
{
    public string name;
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public string prefabPath;
    // �����������Ҫ���������

    public ObjectInfo(GameObject obj)
    {
        name = obj.name;
        position = obj.transform.position;
        rotation = obj.transform.eulerAngles;
        scale = obj.transform.localScale;
        prefabPath = UnityEditor.PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
    }
}
public class ObjectEditorWindow : EditorWindow
{
    private List<GameObject> selectedObjects = new List<GameObject>();
    private string savePath = "Assets/SelectedObjectInfo.txt";

    [MenuItem("Tools/Object Editor")]
    public static void ShowWindow()
    {
        GetWindow<ObjectEditorWindow>("Object Editor");
    }

    void OnGUI()
    {
        GUILayout.Label("Selected Objects", EditorStyles.boldLabel);

        // ��ʾ��ǰѡ�������
        foreach (var obj in selectedObjects)
        {
            EditorGUILayout.ObjectField(obj, typeof(GameObject), true);
        }

        // ���/�Ƴ���ť
        if (GUILayout.Button("Add Selected Objects"))
        {
            AddSelectedObjects();
        }

        if (GUILayout.Button("Clear Selection"))
        {
            selectedObjects.Clear();
        }

        EditorGUILayout.Space();

        // ����/���ذ�ť
        if (GUILayout.Button("Save to File"))
        {
            SaveToFile();
        }

        if (GUILayout.Button("Load from File"))
        {
            LoadFromFile();
        }
    }

    void AddSelectedObjects()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            if (!selectedObjects.Contains(obj))
            {
                selectedObjects.Add(obj);
            }
        }
    }

    void SaveToFile()
    {
        if (selectedObjects.Count == 0)
        {
            EditorUtility.DisplayDialog("Error", "No objects selected to save!", "OK");
            return;
        }

        // ����Ҫ���������
        List<ObjectInfo> objectsToSave = selectedObjects
            .Select(obj => new ObjectInfo(obj))
            .ToList();

        // ���л�ΪJSON
        string jsonData = JsonUtility.ToJson(new SerializationWrapper<ObjectInfo>(objectsToSave), true);

        // д���ļ�
        File.WriteAllText(savePath, jsonData);

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Success", $"Objects saved to {savePath}", "OK");
    }

    void LoadFromFile()
    {
        if (!File.Exists(savePath))
        {
            EditorUtility.DisplayDialog("Error", "Save file not found!", "OK");
            return;
        }

        // ��ȡ�ļ�
        string jsonData = File.ReadAllText(savePath);

        // �����л�
        var wrapper = JsonUtility.FromJson<SerializationWrapper<ObjectInfo>>(jsonData);
        List<ObjectInfo> loadedObjects = wrapper.items;

        // ��յ�ǰѡ��
        selectedObjects.Clear();

        // ��������
        foreach (var objInfo in loadedObjects)
        {
            // ��Ԥ����·������
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(objInfo.prefabPath);
            if (prefab != null)
            {
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                instance.name = objInfo.name;
                instance.transform.position = objInfo.position;
                instance.transform.eulerAngles = objInfo.rotation;
                instance.transform.localScale = objInfo.scale;

                selectedObjects.Add(instance);
            }
            else
            {
                Debug.LogWarning($"Prefab not found at path: {objInfo.prefabPath}");
            }
        }

        EditorUtility.DisplayDialog("Success", $"Loaded {loadedObjects.Count} objects", "OK");
    }

    // �������������л��б�
    [Serializable]
    private class SerializationWrapper<T>
    {
        public List<T> items;

        public SerializationWrapper(List<T> items)
        {
            this.items = items;
        }
    }
}