using UnityEditor;
using UnityEngine;

public class BasicEditorWindow : EditorWindow
{
    // 添加菜单项
    [MenuItem("编辑器/Basic Editor")]
    public static void ShowWindow()
    {
        // 创建窗口实例并设置标题
        var window = GetWindow<BasicEditorWindow>();
        window.titleContent = new GUIContent("Basic Editor");

        // 设置窗口尺寸限制
        window.minSize = new Vector2(300, 200);
        window.maxSize = new Vector2(800, 400);

        // 可选：设置初始位置和大小
        window.position = new Rect(100, 100, 400, 300);
    }

    // 窗口GUI内容
    void OnGUI()
    {
        GUILayout.Label("欢迎使用 Basic Editor", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("这是一个基础编辑器窗口示例\n最小尺寸: 300x200\n最大尺寸: 800x400", MessageType.Info);

        // 添加更多自定义UI元素...
    }
}