using UnityEditor;
using UnityEngine;
using Unity.Entities;

public class AutoSectionIndexer : EditorWindow
{
    GameObject root;
    int startIndex = 1;

    [MenuItem("Tools/Simple Section Indexer")]
    static void ShowWindow()
    {
        GetWindow<AutoSectionIndexer>("Section Indexer");
    }

    void OnGUI()
    {
        /*
         "Root" 라벨
         root - 넣을 루트
         typeof(GameObject)
         true 씬 안의 오브젝트를 허용할지 여부
         */
        root = (GameObject)EditorGUILayout.ObjectField("Root", root, typeof(GameObject), true);

        if (GUILayout.Button("Section Index"))
        {
            if (root == null)
            {
                return;
            }

            SectionIndexing();
        }
    }

    void SectionIndexing()
    {
        int index = startIndex;

        DFS(root.transform, ref index);
    }

    void DFS(Transform current, ref int index)
    {
        var section = current.GetComponent<SceneSectionComponent>();
        if (section != null)
            section.SectionIndex = index++;

        foreach (Transform child in current)
        {
            DFS(child, ref index); 
        }
    }
}
