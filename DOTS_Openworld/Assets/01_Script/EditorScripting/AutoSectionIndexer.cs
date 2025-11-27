using Streaming.SceneManagement.SectionMetadata;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

public class AutoSectionIndexer : EditorWindow
{
    GameObject root;
    int startIndex = 1;

    // CircleAuthoring.radius 값을 일괄로 설정할 값
    float circleRadius = 10f;

    [MenuItem("Tools/Section Indexer")]
    static void ShowWindow()
    {
        GetWindow<AutoSectionIndexer>("Section Indexer");
    }

    void OnGUI()
    {
        root = (GameObject)EditorGUILayout.ObjectField("Root", root, typeof(GameObject), true);

        // 시작 인덱스 입력
        startIndex = EditorGUILayout.IntField("Start Index", startIndex);

        // 반지름 값 입력
        circleRadius = EditorGUILayout.FloatField("Circle Radius", circleRadius);

        if (GUILayout.Button("Section Index"))
        {
            if (root == null)
                return;

            // Undo 지원
            Undo.RegisterFullObjectHierarchyUndo(root, "Auto Section Indexing");

            SectionIndexing();
        }
    }

    void SectionIndexing()
    {
        int index = startIndex;
        DFS(root.transform, ref index);
        Debug.Log($"Section Indexing 완료. 마지막 Index = {index - 1}");
    }

    void DFS(Transform current, ref int index)
    {
        if (current != root.transform)
        {
            var section = current.GetComponent<SceneSectionComponent>();
            var circle = current.GetComponent<CircleAuthoring>();

            if (section == null)
                section = current.gameObject.AddComponent<SceneSectionComponent>();
            if (circle == null)
                circle = current.gameObject.AddComponent<CircleAuthoring>();

            section.SectionIndex = index++;
            circle.radius = circleRadius;       
            
            
            EditorUtility.SetDirty(section);
            EditorUtility.SetDirty(circle);
        }

        foreach (Transform child in current)
            DFS(child, ref index);
    }

}
