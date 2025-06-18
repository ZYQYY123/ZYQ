using UnityEditor;
using UnityEngine;

public class MyCustomEditorWindow : EditorWindow
{
    // 添加菜单项
    [MenuItem("编辑器/自定义编辑器")]
    public static void ShowWindow()
    {
        // 创建窗口实例
        GetWindow<MyCustomEditorWindow>("自定义编辑器");
    }

    // 窗口GUI内容
    void OnGUI()
    {
        GUILayout.Label("自定义编辑器主界面", EditorStyles.boldLabel);

        if (GUILayout.Button("点击我"))
        {
            Debug.Log("按钮被点击!");
        }

        // 添加更多自定义UI元素...
    }
}