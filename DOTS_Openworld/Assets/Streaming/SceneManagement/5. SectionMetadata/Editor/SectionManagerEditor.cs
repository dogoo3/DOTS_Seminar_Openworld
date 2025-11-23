using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(SectionManager))]
public class SectionManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SectionManager manager = (SectionManager)target;
        base.DrawDefaultInspector();


        if (GUILayout.Button("Sections Spawn && Set Index"))
        {
            manager.GenerateSections();
        }

        if (GUILayout.Button("Sections Delete"))
        {
            manager.ClearSections();
        }
    }


}
#endif