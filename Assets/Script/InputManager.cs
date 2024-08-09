using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private LayerMask placementLayerMask;      // 특정레이어에 대해서만 충돌을 감지하기 위한 레이어 마스크


    private Vector3 lastPostion;        // 마지막으로 클릭된 위치 저장


    public event Action Onclicked, OnExit;      // 마우스 클릭과 esc 키 입력에 대응하는 이벤트 핸들러
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Onclicked?.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OnExit?.Invoke();
        }
    }
    public bool IsPointerOverUI()                                   //마우스 포인터가 UI요소 위에 있는지 확인
        => EventSystem.current.IsPointerOverGameObject();           // 현재 선택된 UI 요소가 있는지 반환

    public Vector3 GetSelectedMapPosition()                         // 현재 마우스 위치를 기반으로 월드 좌표를 반환
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;                     // 마우스 위치를 3D 레이로 변환 할 떄 레이의 시작점을 정확하게 설정하기 위해 사용,
                                                                    // 이것을 통해 마우스 포인터가 가리키는 지점을 3D 월드 공간에서 올바르게 계산할 수 있다

        Ray ray = sceneCamera.ScreenPointToRay(mousePos);           
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100, placementLayerMask))
            lastPostion = hit.point;
        return lastPostion;
    }
}
