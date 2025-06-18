using UnityEditor;
using UnityEngine;

public class BasicEditorWindow : EditorWindow
{
    // ��Ӳ˵���
    [MenuItem("�༭��/Basic Editor")]
    public static void ShowWindow()
    {
        // ��������ʵ�������ñ���
        var window = GetWindow<BasicEditorWindow>();
        window.titleContent = new GUIContent("Basic Editor");

        // ���ô��ڳߴ�����
        window.minSize = new Vector2(300, 200);
        window.maxSize = new Vector2(800, 400);

        // ��ѡ�����ó�ʼλ�úʹ�С
        window.position = new Rect(100, 100, 400, 300);
    }

    // ����GUI����
    void OnGUI()
    {
        GUILayout.Label("��ӭʹ�� Basic Editor", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("����һ�������༭������ʾ��\n��С�ߴ�: 300x200\n���ߴ�: 800x400", MessageType.Info);

        // ��Ӹ����Զ���UIԪ��...
    }
}