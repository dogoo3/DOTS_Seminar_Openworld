using UnityEngine;
using UnityEditor;
using Unity.Entities; 
using System.Collections.Generic;
using UnityEngine.SceneManagement;  


public class SettingSectionIndex : EditorWindow
{
    public GameObject[] objs; // 섹션으로 사용할 프리팹 배열  

    SerializedObject so; // 에디터에서 SerializedObject를 사용하여 필드를 관리  


    public int columns; // 섹션을 배치할 그리드의 열 개수  
    public int spawnCnt; // 생성할 섹션의 총 개수  
    float spacing; // 섹션 간 간격(거리)  

    private List<GameObject> generatedSections = new List<GameObject>(); // 생성된 섹션들을 저장하는 리스트  

    // Unity 에디터 메뉴에 "MyDotsProject/SettingSectionIndex" 항목 추가  
    [MenuItem("MyDotsProject/SettingSectionIndex")]
    private static void ShowWindow()
    {
        // 커스텀 에디터 창 생성  
        var window = GetWindow<SettingSectionIndex>();
        window.titleContent = new GUIContent("SettingSectionIndex"); // 에디터 창 제목 설정  
        window.maxSize = new Vector2(300.0f, 500.0f); // 에디터 창의 최대 크기 설정  
        window.minSize = new Vector2(300.0f, 500.0f); // 에디터 창의 최소 크기 설정  
        window.Show(); // 에디터 창 표시  
    }

    // 에디터 창이 활성화될 때 호출  
    private void OnEnable()
    {
        // 현재 스크립트를 SerializedObject로 감싸서 에디터에서 관리 가능하게 설정  
        ScriptableObject target = this;
        so = new SerializedObject(target);
    }

    // 에디터 창의 사용자 인터페이스(UI)를 그리는 함수  
    private void OnGUI()
    {
        // 단순 텍스트 출력
        GUILayout.Label("Array:"); 

      
        so.Update(); // SerializedObject 업데이트  
        SerializedProperty titleProperty = so.FindProperty("objs"); // "objs" 배열 속성을 찾음  
        EditorGUILayout.PropertyField(titleProperty, true); // 배열 필드를 에디터에 표시  
        so.ApplyModifiedProperties(); // 변경사항을 적용  

        // 열 개수와 생성 개수 입력 필드  
        columns = EditorGUILayout.IntField("Columns", columns); // 그리드 열 개수 입력  
        spawnCnt = EditorGUILayout.IntField("Spawn Count", spawnCnt); // 생성할 섹션 개수 입력  

        // 섹션 생성 버튼  
        if (GUILayout.Button("Generate Sections"))
            GenerateSections();

        // 섹션 삭제 버튼  
        if (GUILayout.Button("Clear Sections"))
            ClearSections();
    }

    // 섹션을 생성하는 함수  
    public void GenerateSections()
    {
        if (objs == null || objs.Length == 0)
            return;

        // "SectionMetadataSubscene"이라는 이름의 서브씬 가져오기  
        Scene subScene = SceneManager.GetSceneByName("SectionMetadataSubscene");

        // 첫 번째 프리팹의 첫 번째 자식 Transform을 기준으로 섹션 간 간격(spacing)을 계산  
        Transform planeObj = objs[0].transform.GetChild(0); // 첫 번째 프리팹의 첫 번째 자식 Transform 가져오기  
        spacing = planeObj.transform.localScale.x * 10; // 자식 Transform의 x축 스케일을 기준으로 간격 설정  

        ClearSections();

        // 섹션을 spawnCnt 개수만큼 생성  
        for (int i = 0; i < spawnCnt; i++)
        {
            int x = i % columns; 
            int z = i / columns; 
            Vector3 position = new Vector3(x * spacing, 0, z * spacing); // 섹션의 위치 계산  

            GameObject section = Instantiate(objs[i % objs.Length], position, Quaternion.identity);

            // SceneSectionComponent의 SectionIndex 속성 설정  
            SceneSectionComponent sceneSectionComponent = section.GetComponent<SceneSectionComponent>();
            sceneSectionComponent.SectionIndex = i + 1; // 섹션의 인덱스를 현재 반복 횟수로 설정  
            section.name = $"Section_{i + 1}";

            // 생성된 섹션을 지정된 서브씬으로
            SceneManager.MoveGameObjectToScene(section, subScene);
            generatedSections.Add(section);
        }
    }


    // 생성된 섹션을 모두 삭제하는 함수  
    public void ClearSections()
    {
        // 생성된 섹션 리스트를 순회하며 삭제  
        foreach (GameObject section in generatedSections)
        {
            if (section != null)
                DestroyImmediate(section); // 에디터 환경에서 즉시 삭제  
        }

        // 리스트 초기화  
        generatedSections.Clear();
    }
}
