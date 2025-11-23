using UnityEngine;
using System.Collections.Generic;
using Unity.Entities;

public class SectionManager : MonoBehaviour
{
    public GameObject sectionPrefab;
    public int columns;
    public int spawnCnt;

    float spacing;
    // 생성한 sectionPrefab 저장 list
    public List<GameObject> sections = new List<GameObject>();


    public void GenerateSections()
    {
        Transform planeObj = sectionPrefab.transform.GetChild(0);
        spacing = planeObj.transform.localScale.x * 10;

         ClearSections();
        for (int i = 0; i < spawnCnt; i++)
        {
            int x = i % columns;
            int z = i / columns;
            Vector3 position = new Vector3(x * spacing, 0, z * spacing);

            GameObject section = Instantiate(sectionPrefab, position, Quaternion.identity, transform);
            section.name = $"Section{i + 1}";
            sections.Add(section);
        }

        SetSectionIndexes();
    }


    public void SetSectionIndexes()
    {
        for (int i = 0; i < sections.Count; i++)
        {
            SceneSectionComponent sceneSectionComponent = sections[i].GetComponent<SceneSectionComponent>();
            if (sceneSectionComponent != null)
            {
                sceneSectionComponent.SectionIndex = i + 1;
            }
        }
    }

    public void ClearSections()
    {
        foreach (var section in sections)
        {
            if (section != null) DestroyImmediate(section);
        }

        sections.Clear();
    }
}