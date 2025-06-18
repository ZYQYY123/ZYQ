using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class AdvancedObjectEditor : EditorWindow
{
    private Vector2 scrollPosition;
    private bool showTransform = true;
    private bool showComponents = true;
    private GUIStyle headerStyle;
    private GUIStyle sectionStyle;
    private GUIStyle componentStyle;
    private Dictionary<GameObject, bool> foldoutStates = new Dictionary<GameObject, bool>();

    [MenuItem("Tools/Advanced Object Editor")]
    public static void ShowWindow()
    {
        var window = GetWindow<AdvancedObjectEditor>();
        window.titleContent = new GUIContent("�߼�����༭��");
        window.minSize = new Vector2(350, 400);
        window.Show();
    }

    void OnEnable()
    {
        // ��ʼ���Զ�����ʽ
        headerStyle = new GUIStyle(EditorStyles.largeLabel)
        {
            fontSize = 18,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = new Color(0.9f, 0.9f, 0.9f) }
        };

        sectionStyle = new GUIStyle(EditorStyles.helpBox)
        {
            padding = new RectOffset(10, 10, 10, 10),
            margin = new RectOffset(5, 5, 5, 5)
        };

        componentStyle = new GUIStyle(EditorStyles.miniButton)
        {
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(15, 5, 5, 5)
        };
    }

    void OnGUI()
    {
        // ���Ʊ���
        EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), new Color(0.2f, 0.2f, 0.2f));

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        GameObject selected = Selection.activeGameObject;

        if (selected == null)
        {
            DrawNoSelectionUI();
        }
        else
        {
            DrawSelectedObjectUI(selected);
        }

        EditorGUILayout.EndScrollView();
    }

    void DrawNoSelectionUI()
    {
        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("û��ѡ�����", headerStyle);
        EditorGUILayout.Space(10);

        EditorGUILayout.HelpBox("���ڳ������νṹ��ѡ��һ����Ϸ����", MessageType.Info);

        EditorGUILayout.Space(20);
        if (GUILayout.Button("ѡ���������", GUILayout.Height(30)))
        {
            Selection.activeGameObject = Camera.main?.gameObject;
        }
    }

    void DrawSelectedObjectUI(GameObject obj)
    {
        // �������
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField(obj.name, headerStyle);
        EditorGUILayout.Space(10);

        // ���������ֶ�
        EditorGUILayout.BeginHorizontal(sectionStyle);
        EditorGUILayout.LabelField("��������:", GUILayout.Width(80));
        EditorGUILayout.ObjectField(obj, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();

        // �任���
        showTransform = EditorGUILayout.Foldout(showTransform, "�任����", true, EditorStyles.foldoutHeader);
        if (showTransform)
        {
            EditorGUILayout.BeginVertical(sectionStyle);
            DrawTransformControls(obj.transform);
            EditorGUILayout.EndVertical();
        }

        // ����б�
        showComponents = EditorGUILayout.Foldout(showComponents, "����б�", true, EditorStyles.foldoutHeader);
        if (showComponents)
        {
            EditorGUILayout.BeginVertical(sectionStyle);
            DrawComponentList(obj);
            EditorGUILayout.EndVertical();
        }

        // �Զ��尴ť����
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("���ñ任", componentStyle))
        {
            Undo.RecordObject(obj.transform, "Reset Transform");
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
        }
        if (GUILayout.Button("�۽�����", componentStyle))
        {
            SceneView.lastActiveSceneView.FrameSelected();
        }
        EditorGUILayout.EndHorizontal();
    }

    void DrawTransformControls(Transform transform)
    {
        // λ��
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("λ��", GUILayout.Width(60));
        Vector3 newPosition = EditorGUILayout.Vector3Field("", transform.localPosition);
        if (newPosition != transform.localPosition)
        {
            Undo.RecordObject(transform, "Change Position");
            transform.localPosition = newPosition;
        }
        EditorGUILayout.EndHorizontal();

        // ��ת
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("��ת", GUILayout.Width(60));
        Vector3 newEulerAngles = EditorGUILayout.Vector3Field("", transform.localEulerAngles);
        if (newEulerAngles != transform.localEulerAngles)
        {
            Undo.RecordObject(transform, "Change Rotation");
            transform.localEulerAngles = newEulerAngles;
        }
        EditorGUILayout.EndHorizontal();

        // ����
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("����", GUILayout.Width(60));
        Vector3 newScale = EditorGUILayout.Vector3Field("", transform.localScale);
        if (newScale != transform.localScale)
        {
            Undo.RecordObject(transform, "Change Scale");
            transform.localScale = newScale;
        }
        EditorGUILayout.EndHorizontal();
    }

    void DrawComponentList(GameObject obj)
    {
        foreach (Component comp in obj.GetComponents<Component>())
        {
            EditorGUILayout.BeginHorizontal();

            // ��ȡ���ʼ���۵�״̬
            if (!foldoutStates.ContainsKey(obj))
            {
                foldoutStates[obj] = false;
            }

            // ����۵���ť
            foldoutStates[obj] = EditorGUILayout.Foldout(foldoutStates[obj], comp.GetType().Name, true);

            // ��������ֶ�
            EditorGUILayout.ObjectField(comp, comp.GetType(), true, GUILayout.Width(150));

            // �۽���ť
            if (GUILayout.Button("��", GUILayout.Width(25)))
            {
                EditorGUIUtility.PingObject(comp);
            }

            EditorGUILayout.EndHorizontal();

            // ��ʾ�������
            if (foldoutStates[obj])
            {
                EditorGUILayout.BeginVertical("Box");
                Editor editor = Editor.CreateEditor(comp);
                editor.OnInspectorGUI();
                EditorGUILayout.EndVertical();
            }
        }
    }

    void OnSelectionChange() => Repaint();
    void OnFocus() => Repaint();
    void OnHierarchyChange() => Repaint();
}