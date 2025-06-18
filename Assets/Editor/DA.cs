using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;

public class DA : EditorWindow
{
    private static string saveFilePath = "Assets/SelectedObjectInfo.txt";
    private GUIStyle saveButtonStyle;
    private GUIStyle loadButtonStyle;

    [MenuItem("Tools/DA Object Saver")]
    public static void ShowWindow()
    {
        var window = GetWindow<DA>();
        window.titleContent = new GUIContent("DA Object Saver");
        window.minSize = new Vector2(300, 150);
    }

    void OnEnable()
    {
        // 初始化按钮样式
        saveButtonStyle = new GUIStyle(EditorStyles.miniButton)
        {
            normal = { textColor = Color.green },
            fontStyle = FontStyle.Bold,
            padding = new RectOffset(15, 15, 5, 5)
        };

        loadButtonStyle = new GUIStyle(EditorStyles.miniButton)
        {
            normal = { textColor = Color.blue },
            fontStyle = FontStyle.Bold,
            padding = new RectOffset(15, 15, 5, 5)
        };
    }

    void OnGUI()
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("DA Object Saver", EditorStyles.boldLabel);
        EditorGUILayout.Space(20);

        // 保存加载按钮区域
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("DA Save", saveButtonStyle, GUILayout.Width(100), GUILayout.Height(30)))
        {
            DASaveObjectInfo();
        }

        if (GUILayout.Button("DA Load", loadButtonStyle, GUILayout.Width(100), GUILayout.Height(30)))
        {
            DALoadObjectInfo();
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox("保存路径: " + saveFilePath, MessageType.Info);
    }

    static void DASaveObjectInfo()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("DA Save Failed", "No object selected", "OK");
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("DA_OBJECT_DATA_START");
        sb.AppendLine("name:" + selected.name);
        sb.AppendLine("position:" + Vector3ToString(selected.transform.position));
        sb.AppendLine("rotation:" + Vector3ToString(selected.transform.eulerAngles));
        sb.AppendLine("scale:" + Vector3ToString(selected.transform.localScale));
        sb.AppendLine("DA_OBJECT_DATA_END");

        try
        {
            File.WriteAllText(saveFilePath, sb.ToString());
            AssetDatabase.Refresh();
            Debug.Log("[DA] Object info saved to: " + saveFilePath);
            EditorUtility.DisplayDialog("DA Save Success", "Object data saved!", "OK");
        }
        catch (System.Exception e)
        {
            Debug.LogError("[DA] Save failed: " + e.Message);
            EditorUtility.DisplayDialog("DA Save Failed", e.Message, "OK");
        }
    }

    static void DALoadObjectInfo()
    {
        if (!File.Exists(saveFilePath))
        {
            EditorUtility.DisplayDialog("DA Load Failed", "Save file not found", "OK");
            return;
        }

        try
        {
            string[] lines = File.ReadAllLines(saveFilePath);
            var data = new System.Collections.Generic.Dictionary<string, string>();

            bool readingData = false;
            foreach (string line in lines)
            {
                if (line == "DA_OBJECT_DATA_START")
                {
                    readingData = true;
                    continue;
                }
                if (line == "DA_OBJECT_DATA_END")
                    break;

                if (readingData && line.Contains(":"))
                {
                    string[] parts = line.Split(':');
                    if (parts.Length >= 2)
                    {
                        data[parts[0]] = parts[1];
                    }
                }
            }

            if (data.Count == 0)
            {
                EditorUtility.DisplayDialog("DA Load Failed", "No valid data in file", "OK");
                return;
            }

            // 获取或创建对象
            GameObject targetObj = Selection.activeGameObject;
            bool isNewObject = false;
            if (targetObj == null)
            {
                targetObj = new GameObject(data["name"]);
                isNewObject = true;
            }

            // 应用变换数据
            Undo.RecordObject(targetObj.transform, "DA Load Transform");
            targetObj.transform.position = StringToVector3(data["position"]);
            targetObj.transform.eulerAngles = StringToVector3(data["rotation"]);
            targetObj.transform.localScale = StringToVector3(data["scale"]);

            if (isNewObject)
            {
                Selection.activeGameObject = targetObj;
            }

            Debug.Log("[DA] Object data loaded from: " + saveFilePath);
            EditorUtility.DisplayDialog("DA Load Success", "Object data loaded!", "OK");
        }
        catch (System.Exception e)
        {
            Debug.LogError("[DA] Load failed: " + e.Message);
            EditorUtility.DisplayDialog("DA Load Failed", e.Message, "OK");
        }
    }

    static string Vector3ToString(Vector3 vec)
    {
        return string.Format("{0:F3},{1:F3},{2:F3}", vec.x, vec.y, vec.z);
    }

    static Vector3 StringToVector3(string str)
    {
        string[] parts = str.Split(',');
        return new Vector3(
            float.Parse(parts[0]),
            float.Parse(parts[1]),
            float.Parse(parts[2]));
    }
}