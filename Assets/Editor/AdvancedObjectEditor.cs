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
        window.titleContent = new GUIContent("高级对象编辑器");
        window.minSize = new Vector2(350, 400);
        window.Show();
    }

    void OnEnable()
    {
        // 初始化自定义样式
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
        // 绘制背景
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
        EditorGUILayout.LabelField("没有选择对象", headerStyle);
        EditorGUILayout.Space(10);

        EditorGUILayout.HelpBox("请在场景或层次结构中选择一个游戏对象", MessageType.Info);

        EditorGUILayout.Space(20);
        if (GUILayout.Button("选择主摄像机", GUILayout.Height(30)))
        {
            Selection.activeGameObject = Camera.main?.gameObject;
        }
    }

    void DrawSelectedObjectUI(GameObject obj)
    {
        // 对象标题
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField(obj.name, headerStyle);
        EditorGUILayout.Space(10);

        // 对象引用字段
        EditorGUILayout.BeginHorizontal(sectionStyle);
        EditorGUILayout.LabelField("对象引用:", GUILayout.Width(80));
        EditorGUILayout.ObjectField(obj, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();

        // 变换组件
        showTransform = EditorGUILayout.Foldout(showTransform, "变换属性", true, EditorStyles.foldoutHeader);
        if (showTransform)
        {
            EditorGUILayout.BeginVertical(sectionStyle);
            DrawTransformControls(obj.transform);
            EditorGUILayout.EndVertical();
        }

        // 组件列表
        showComponents = EditorGUILayout.Foldout(showComponents, "组件列表", true, EditorStyles.foldoutHeader);
        if (showComponents)
        {
            EditorGUILayout.BeginVertical(sectionStyle);
            DrawComponentList(obj);
            EditorGUILayout.EndVertical();
        }

        // 自定义按钮区域
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("重置变换", componentStyle))
        {
            Undo.RecordObject(obj.transform, "Reset Transform");
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
        }
        if (GUILayout.Button("聚焦对象", componentStyle))
        {
            SceneView.lastActiveSceneView.FrameSelected();
        }
        EditorGUILayout.EndHorizontal();
    }

    void DrawTransformControls(Transform transform)
    {
        // 位置
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("位置", GUILayout.Width(60));
        Vector3 newPosition = EditorGUILayout.Vector3Field("", transform.localPosition);
        if (newPosition != transform.localPosition)
        {
            Undo.RecordObject(transform, "Change Position");
            transform.localPosition = newPosition;
        }
        EditorGUILayout.EndHorizontal();

        // 旋转
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("旋转", GUILayout.Width(60));
        Vector3 newEulerAngles = EditorGUILayout.Vector3Field("", transform.localEulerAngles);
        if (newEulerAngles != transform.localEulerAngles)
        {
            Undo.RecordObject(transform, "Change Rotation");
            transform.localEulerAngles = newEulerAngles;
        }
        EditorGUILayout.EndHorizontal();

        // 缩放
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("缩放", GUILayout.Width(60));
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

            // 获取或初始化折叠状态
            if (!foldoutStates.ContainsKey(obj))
            {
                foldoutStates[obj] = false;
            }

            // 组件折叠按钮
            foldoutStates[obj] = EditorGUILayout.Foldout(foldoutStates[obj], comp.GetType().Name, true);

            // 组件引用字段
            EditorGUILayout.ObjectField(comp, comp.GetType(), true, GUILayout.Width(150));

            // 聚焦按钮
            if (GUILayout.Button("◎", GUILayout.Width(25)))
            {
                EditorGUIUtility.PingObject(comp);
            }

            EditorGUILayout.EndHorizontal();

            // 显示组件详情
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