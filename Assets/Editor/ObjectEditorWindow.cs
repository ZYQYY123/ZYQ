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
    // 添加其他你需要保存的属性

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

        // 显示当前选择的物体
        foreach (var obj in selectedObjects)
        {
            EditorGUILayout.ObjectField(obj, typeof(GameObject), true);
        }

        // 添加/移除按钮
        if (GUILayout.Button("Add Selected Objects"))
        {
            AddSelectedObjects();
        }

        if (GUILayout.Button("Clear Selection"))
        {
            selectedObjects.Clear();
        }

        EditorGUILayout.Space();

        // 保存/加载按钮
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

        // 创建要保存的数据
        List<ObjectInfo> objectsToSave = selectedObjects
            .Select(obj => new ObjectInfo(obj))
            .ToList();

        // 序列化为JSON
        string jsonData = JsonUtility.ToJson(new SerializationWrapper<ObjectInfo>(objectsToSave), true);

        // 写入文件
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

        // 读取文件
        string jsonData = File.ReadAllText(savePath);

        // 反序列化
        var wrapper = JsonUtility.FromJson<SerializationWrapper<ObjectInfo>>(jsonData);
        List<ObjectInfo> loadedObjects = wrapper.items;

        // 清空当前选择
        selectedObjects.Clear();

        // 加载物体
        foreach (var objInfo in loadedObjects)
        {
            // 从预制体路径加载
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

    // 辅助类用于序列化列表
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