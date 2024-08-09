using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private LayerMask placementLayerMask;      // Ư�����̾ ���ؼ��� �浹�� �����ϱ� ���� ���̾� ����ũ


    private Vector3 lastPostion;        // ���������� Ŭ���� ��ġ ����


    public event Action Onclicked, OnExit;      // ���콺 Ŭ���� esc Ű �Է¿� �����ϴ� �̺�Ʈ �ڵ鷯
    
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
    public bool IsPointerOverUI()                                   //���콺 �����Ͱ� UI��� ���� �ִ��� Ȯ��
        => EventSystem.current.IsPointerOverGameObject();           // ���� ���õ� UI ��Ұ� �ִ��� ��ȯ

    public Vector3 GetSelectedMapPosition()                         // ���� ���콺 ��ġ�� ������� ���� ��ǥ�� ��ȯ
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;                     // ���콺 ��ġ�� 3D ���̷� ��ȯ �� �� ������ �������� ��Ȯ�ϰ� �����ϱ� ���� ���,
                                                                    // �̰��� ���� ���콺 �����Ͱ� ����Ű�� ������ 3D ���� �������� �ùٸ��� ����� �� �ִ�

        Ray ray = sceneCamera.ScreenPointToRay(mousePos);           
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100, placementLayerMask))
            lastPostion = hit.point;
        return lastPostion;
    }
}
