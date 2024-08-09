using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject mouseIndicator, cellIndicator;       // 마우스 커서 위치를 시각적으로 나타내는
                                                            // 오브젝트 현재 선택된 셀을 나타내는 오브젝트 
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDatabaseSO database;         // 배치가능한 오브젝트 데이터를 담고있는 데이터 베이스 ScriptableObject
    private int selectedObjectIndex = -1;       // 현재 선택된 오브젝트의 인덱스. -1은 아무것도 선택되지 않았음을 나타냄 

    [SerializeField]
    private GameObject gridVisualization;       // 배치 시각화를 위한 그리드. 배치 모드가 활성화되면 표시됨 

    private GridData floorData, funitureData;   // 바닥과 가구 데이터를 관리하는 객체들. 오브젝트가 배치된 위치와 관련된 데이터를 저장

    private Renderer previewRenderer;           // cellIndicator 오브젝트의 렌더러. 배치할 수 있는지 여부에 따라 색상을 변경하는데 사용

    private List<GameObject> placeGameObject = new();   // 배치된 게임 오브젝트 목록을 저장

    private void Start()
    {
        gridVisualization.SetActive(false);
        floorData = new();
        funitureData = new();
        previewRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    public void StartPlacement(int ID)      // 특정 ID의 오브젝트를 배치를 시작함 
    {
        StopPlacement();        // 이전에 진행중이던 배치작업을 중지. 이미선택된 오브젝트 배치와 관련된 모든 설정을 초기화 
        selectedObjectIndex = database.objectData.FindIndex(data => data.ID == ID);     // 리스트내에서 ID가 매개변수로 전달된 값과 일치하는 오브젝트를 찾는 조건.
                                                                                        // 만약 찾은 경우 그 오브젝트의 인덱스가 selectedObjectIndex에 저장이 된다
        if(selectedObjectIndex <0)      // 위에서 찾은 오브젝트의 인덱스가 유효한지 확인.
                                        // 조건을 만족하는 항목이 없을때, -1을 반환. 즉, selectedObjectIndex가 -1이라는것은 ID에 해당하는 오브젝트가 데이터 베이스에 존재하지 않는다는 의미
        {
            Debug.Log($"No ID found {ID}");     // -1일 경우 ID가 데이터베이스에 없음을 알림
            return;
        }
        gridVisualization.SetActive(true);  // 그리드를 시각화함
        cellIndicator.SetActive(true);      //선택된 오브젝트가 배치될 그리드 셀을 표시
        inputManager.Onclicked += PlaceStructure;   // inputManager의 Onclick 이벤트에 PlaceStructure 메서드를 구독 시킴
                                                    // 사용자가 마우스를 클릭할때마다 PlaceStructure 메서드가 호출되어 선택된 오브젝트를 현재 마우스 위치의 그리드에 배치
        inputManager.OnExit += StopPlacement;       // inputManager의 OnExit 이벤트에 StopPlacement 메서드를 구독 시킴
                                                    // 사용자가 배치모드를 종료하거나 해당 UI에서 벗어낼때 StopPlacement 메서드가 호출되어 배치 작업을 종료 및 초기화
    }

    private void PlaceStructure()                   // 사용자가 오브젝트를 그리드에 배치하는 과정을 처리하는 핵심 메서드 
    {
        if(inputManager.IsPointerOverUI())          // 마우스를 UI 위에 올려놓고 있을때를 확인
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();      // 사용자의 마수으 위치를 게임월드 좌표로 변환, mousePosition에 저장
                                                                            // inputManager.GetSelectedMapPosistion 메서드는 마우스가 가리키고 있는 게임 맵내의 좌표 반환
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);          // 위에서 얻은 마수으 표지션을 그리드 좌표로 변환
                                                                            // gird.WorldToCell(mousePosition)은 월드좌료를 그리드 좌표로 변환하는 Unity 내장 메서드
                                                                            // 이 변환된 좌표는 오브젝트를 배치할 그리드 셀의 위치를 나타냄

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);     // CheckPlacementValidity 메서드를 호출하여 현재 선택된 오브젝트를 gridPosition에 배치 가능한지 확인
        if (placementValidity == false) return;     // 불가능 하다면 종료

        GameObject newObject = Instantiate(database.objectData[selectedObjectIndex].Prefab);    // 선택된 오브젝트의 프리팹 인스턴스화
                                                                                                // database.objectData[selectedObjectIndex].Prefab은 선택된 오브젝트에 대한 프리팹 참조
        newObject.transform.position = grid.CellToWorld(gridPosition);      // 선택된 오브젝트 위치를 girdPosition에 맞게 설정
        placeGameObject.Add( newObject );           // placeGameObject에 저장  

        GridData selectedData = database.objectData[selectedObjectIndex].ID == 0 ? floorData : funitureData;    // 배치될 오브젝트의 유형에 따라 어떤 그리드 데이터를 사용할지 결정
                                                                                                                // ID가 0이면 바닥과 관련된 데이터 (floorData)를 사용하고 그렇지 않으면 (FuniotureData)를 사용
        selectedData.AddObjectAt(                   // 선택된 오브젝트의 배치 정보를 selectedData에 추가 
            gridPosition,   // 오브젝트가 시작되는 그리드 위치
            database.objectData[selectedObjectIndex].Size,  //오브젝트가 차지하는 그리드 셀의 크기
            database.objectData[selectedObjectIndex].ID,    // 오브젝트의 ID
            placeGameObject.Count - 1);             // 리스트에서의 오브젝트 인덱스 .리스트에 추가된 직후이므로 마지막 인덱스 


        // 요약
        // PlaceStructure 메서드는 사용자가 오브젝트를 배치하려는 위치가 유효한지 확인하고, 그 위치에 오브젝트를 생성한 후, 그리드 데이터에 배치 정보를 기록함.
        // 이 과정에서 UI와 충돌을 방지하고, 오브젝트가 그리드 셀에 정확히 배치되도록 관리.
        // 이를 통해 사용자는 게임 내에서 오브젝트를 직관적으로 배치할 수 있으며, 시스템은 배치된 오브젝트를 효율적으로 관리
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)       // 배치하려는 오브젝트으 ㅣ유형에 따라 적절한 그리드 데이터를 선택 
    {
        GridData selectedData = database.objectData[selectedObjectIndex].ID == 0 ? floorData : funitureData;    // ID가 0이면 floorData, 아니면 furnitureData 참조

        return selectedData.CanPlaceObjectAt(gridPosition, database.objectData[selectedObjectIndex].Size);      // 선택된 위치(gridPosition)에 오브젝트를 배치할 수 있는지 여부 확인 
    }

    private void StopPlacement()        // 오브젝트 배치를 중지 및 정리하는 역할 
    {
        selectedObjectIndex = -1;   // 선택된 오브젝트의 인덱스를 초기화
        gridVisualization.SetActive(false); // 그리드 시각화 종료
        cellIndicator.SetActive(false);     // 셀 시각화 종료
        inputManager.Onclicked -= PlaceStructure;   // PlaceStructure 메서드를 클릭이벤트에서 제거
        inputManager.OnExit -= StopPlacement;       // StopPlacement 메서드를 종료이벤트에서 제거
    }

    private void Update()       // 실시간으로 마우스 위치를 추적하고 배치가능 여부를 시각적으로 표기
    {
        if (selectedObjectIndex < 0) return;        // 선택된 오브젝트가 없는 경우, 메서드의 나머지 부분을 실행하지 않고 종료

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();      // 현재 마우스 커서의 월드 좌표 위치를 가져옴
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);          // 마우스 위치를 그리드 셀 좌표로 변환

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);     // 현재 마수으 위치에서 오브젝트를 배치할 수 있는지 확인
        previewRenderer.material.color = placementValidity ? Color.white : Color.red;           // 배치가능 여부를 시각적으로 표시 배치가능 == white 불가능 == red

        mouseIndicator.transform.position = mousePosition;          // 마우스 인디케이터의 위치를 마우스 커서 위치로 업데이트
        cellIndicator.transform.position = grid.CellToWorld(gridPosition);  // 셀 인디케이터의 위치를 셀 위치로 업데이트
    }
}
