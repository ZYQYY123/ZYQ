using UnityEditor;
using UnityEngine;

public class ObjectDependentEditor : EditorWindow
{
    [MenuItem("�༭��/Object Dependent Editor")]
    public static void ShowWindow()
    {
        var window = GetWindow<ObjectDependentEditor>();
        window.titleContent = new GUIContent("���������༭��");
        window.minSize = new Vector2(300, 200);
    }

    void OnGUI()
    {
        // ��ȡ��ǰѡ�����Ϸ����
        GameObject selectedObject = Selection.activeGameObject;

        // ���û��ѡ���κζ���
        if (selectedObject == null)
        {
            EditorGUILayout.HelpBox("No object selected", MessageType.Warning);
            DrawSelectionPrompt();
            return;
        }

        // ���ѡ���˶�����ʾ������Ϣ
        DrawObjectInfo(selectedObject);
    }

    void DrawSelectionPrompt()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("��ִ�����²���֮һ:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("1. �ڳ�����ͼ��ѡ��һ����Ϸ����");
        EditorGUILayout.LabelField("2. �ڲ�νṹ������ѡ��һ����Ϸ����");

        EditorGUILayout.Space();
        if (GUILayout.Button("ˢ��ѡ��״̬"))
        {
            // ǿ���ػ洰��
            Repaint();
        }
    }

    void DrawObjectInfo(GameObject obj)
    {
        EditorGUILayout.LabelField("��ǰѡ��Ķ���:", EditorStyles.boldLabel);
        EditorGUILayout.ObjectField("Selected Object", obj, typeof(GameObject), true);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("������Ϣ:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"����: {obj.name}");
        EditorGUILayout.LabelField($"λ��: {obj.transform.position}");
        EditorGUILayout.LabelField($"�Ӷ�������: {obj.transform.childCount}");

        // ��Ӹ��������Ϣչʾ...
    }

    // ��ѡ��仯ʱ�Զ����´���
    void OnSelectionChange()
    {
        Repaint();
    }
}