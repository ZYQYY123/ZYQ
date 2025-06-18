using UnityEditor;
using UnityEngine;

public class ObjectDependentEditor : EditorWindow
{
    [MenuItem("编辑器/Object Dependent Editor")]
    public static void ShowWindow()
    {
        var window = GetWindow<ObjectDependentEditor>();
        window.titleContent = new GUIContent("对象依赖编辑器");
        window.minSize = new Vector2(300, 200);
    }

    void OnGUI()
    {
        // 获取当前选择的游戏对象
        GameObject selectedObject = Selection.activeGameObject;

        // 如果没有选择任何对象
        if (selectedObject == null)
        {
            EditorGUILayout.HelpBox("No object selected", MessageType.Warning);
            DrawSelectionPrompt();
            return;
        }

        // 如果选择了对象，显示对象信息
        DrawObjectInfo(selectedObject);
    }

    void DrawSelectionPrompt()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("请执行以下操作之一:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("1. 在场景视图中选择一个游戏对象");
        EditorGUILayout.LabelField("2. 在层次结构窗口中选择一个游戏对象");

        EditorGUILayout.Space();
        if (GUILayout.Button("刷新选择状态"))
        {
            // 强制重绘窗口
            Repaint();
        }
    }

    void DrawObjectInfo(GameObject obj)
    {
        EditorGUILayout.LabelField("当前选择的对象:", EditorStyles.boldLabel);
        EditorGUILayout.ObjectField("Selected Object", obj, typeof(GameObject), true);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("对象信息:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"名称: {obj.name}");
        EditorGUILayout.LabelField($"位置: {obj.transform.position}");
        EditorGUILayout.LabelField($"子对象数量: {obj.transform.childCount}");

        // 添加更多对象信息展示...
    }

    // 当选择变化时自动更新窗口
    void OnSelectionChange()
    {
        Repaint();
    }
}