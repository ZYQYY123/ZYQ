using UnityEditor;
using UnityEngine;

public class MyCustomEditorWindow : EditorWindow
{
    // ��Ӳ˵���
    [MenuItem("�༭��/�Զ���༭��")]
    public static void ShowWindow()
    {
        // ��������ʵ��
        GetWindow<MyCustomEditorWindow>("�Զ���༭��");
    }

    // ����GUI����
    void OnGUI()
    {
        GUILayout.Label("�Զ���༭��������", EditorStyles.boldLabel);

        if (GUILayout.Button("�����"))
        {
            Debug.Log("��ť�����!");
        }

        // ��Ӹ����Զ���UIԪ��...
    }
}