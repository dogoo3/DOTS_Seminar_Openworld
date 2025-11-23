using Unity.Entities;
using UnityEditor;
using UnityEngine;

public class AutoSectionIndexer : EditorWindow
{
    GameObject root;
    int startIndex = 1;

    [MenuItem("Tools/Section Indexer")]
    static void ShowWindow()
    {
        GetWindow<AutoSectionIndexer>("Section Indexer");
    }

    void OnGUI()
    {
        root = (GameObject)EditorGUILayout.ObjectField("Root", root, typeof(GameObject), true);

        // 시작 인덱스도 에디터에서 입력
        startIndex = EditorGUILayout.IntField("Start Index", startIndex);

        if (GUILayout.Button("Section Index"))
        {
            if (root == null)
            {
                EditorUtility.DisplayDialog("Section Indexer",
                    "Root GameObject를 넣어줘.", "OK");
                return;
            }

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
        // 루트가 아니라면
        if (current != root.transform)
        {
            // 우선 검사
            var section = current.GetComponent<SceneSectionComponent>();


            // 없으면 추가
            if (section == null)
                section = current.gameObject.AddComponent<SceneSectionComponent>();

            // 현재 sectionIndex = index 대입 후 +1
            section.SectionIndex = index++;

            EditorUtility.SetDirty(section);
        }

        // 6. 현재 오브젝트의 모든 자식 트랜스폼에 대해
        //    다시 DFS를 호출해서 똑같은 작업을 반복한다
        foreach (Transform child in current)
        {
            DFS(child, ref index);
        }
    }


}
