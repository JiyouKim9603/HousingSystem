using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject mouseIndicator, cellIndicator;       // ���콺 Ŀ�� ��ġ�� �ð������� ��Ÿ����
                                                            // ������Ʈ ���� ���õ� ���� ��Ÿ���� ������Ʈ 
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDatabaseSO database;         // ��ġ������ ������Ʈ �����͸� ����ִ� ������ ���̽� ScriptableObject
    private int selectedObjectIndex = -1;       // ���� ���õ� ������Ʈ�� �ε���. -1�� �ƹ��͵� ���õ��� �ʾ����� ��Ÿ�� 

    [SerializeField]
    private GameObject gridVisualization;       // ��ġ �ð�ȭ�� ���� �׸���. ��ġ ��尡 Ȱ��ȭ�Ǹ� ǥ�õ� 

    private GridData floorData, funitureData;   // �ٴڰ� ���� �����͸� �����ϴ� ��ü��. ������Ʈ�� ��ġ�� ��ġ�� ���õ� �����͸� ����

    private Renderer previewRenderer;           // cellIndicator ������Ʈ�� ������. ��ġ�� �� �ִ��� ���ο� ���� ������ �����ϴµ� ���

    private List<GameObject> placeGameObject = new();   // ��ġ�� ���� ������Ʈ ����� ����

    private void Start()
    {
        gridVisualization.SetActive(false);
        floorData = new();
        funitureData = new();
        previewRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    public void StartPlacement(int ID)      // Ư�� ID�� ������Ʈ�� ��ġ�� ������ 
    {
        StopPlacement();        // ������ �������̴� ��ġ�۾��� ����. �̹̼��õ� ������Ʈ ��ġ�� ���õ� ��� ������ �ʱ�ȭ 
        selectedObjectIndex = database.objectData.FindIndex(data => data.ID == ID);     // ����Ʈ������ ID�� �Ű������� ���޵� ���� ��ġ�ϴ� ������Ʈ�� ã�� ����.
                                                                                        // ���� ã�� ��� �� ������Ʈ�� �ε����� selectedObjectIndex�� ������ �ȴ�
        if(selectedObjectIndex <0)      // ������ ã�� ������Ʈ�� �ε����� ��ȿ���� Ȯ��.
                                        // ������ �����ϴ� �׸��� ������, -1�� ��ȯ. ��, selectedObjectIndex�� -1�̶�°��� ID�� �ش��ϴ� ������Ʈ�� ������ ���̽��� �������� �ʴ´ٴ� �ǹ�
        {
            Debug.Log($"No ID found {ID}");     // -1�� ��� ID�� �����ͺ��̽��� ������ �˸�
            return;
        }
        gridVisualization.SetActive(true);  // �׸��带 �ð�ȭ��
        cellIndicator.SetActive(true);      //���õ� ������Ʈ�� ��ġ�� �׸��� ���� ǥ��
        inputManager.Onclicked += PlaceStructure;   // inputManager�� Onclick �̺�Ʈ�� PlaceStructure �޼��带 ���� ��Ŵ
                                                    // ����ڰ� ���콺�� Ŭ���Ҷ����� PlaceStructure �޼��尡 ȣ��Ǿ� ���õ� ������Ʈ�� ���� ���콺 ��ġ�� �׸��忡 ��ġ
        inputManager.OnExit += StopPlacement;       // inputManager�� OnExit �̺�Ʈ�� StopPlacement �޼��带 ���� ��Ŵ
                                                    // ����ڰ� ��ġ��带 �����ϰų� �ش� UI���� ����� StopPlacement �޼��尡 ȣ��Ǿ� ��ġ �۾��� ���� �� �ʱ�ȭ
    }

    private void PlaceStructure()                   // ����ڰ� ������Ʈ�� �׸��忡 ��ġ�ϴ� ������ ó���ϴ� �ٽ� �޼��� 
    {
        if(inputManager.IsPointerOverUI())          // ���콺�� UI ���� �÷����� �������� Ȯ��
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();      // ������� ������ ��ġ�� ���ӿ��� ��ǥ�� ��ȯ, mousePosition�� ����
                                                                            // inputManager.GetSelectedMapPosistion �޼���� ���콺�� ����Ű�� �ִ� ���� �ʳ��� ��ǥ ��ȯ
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);          // ������ ���� ������ ǥ������ �׸��� ��ǥ�� ��ȯ
                                                                            // gird.WorldToCell(mousePosition)�� �����·Ḧ �׸��� ��ǥ�� ��ȯ�ϴ� Unity ���� �޼���
                                                                            // �� ��ȯ�� ��ǥ�� ������Ʈ�� ��ġ�� �׸��� ���� ��ġ�� ��Ÿ��

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);     // CheckPlacementValidity �޼��带 ȣ���Ͽ� ���� ���õ� ������Ʈ�� gridPosition�� ��ġ �������� Ȯ��
        if (placementValidity == false) return;     // �Ұ��� �ϴٸ� ����

        GameObject newObject = Instantiate(database.objectData[selectedObjectIndex].Prefab);    // ���õ� ������Ʈ�� ������ �ν��Ͻ�ȭ
                                                                                                // database.objectData[selectedObjectIndex].Prefab�� ���õ� ������Ʈ�� ���� ������ ����
        newObject.transform.position = grid.CellToWorld(gridPosition);      // ���õ� ������Ʈ ��ġ�� girdPosition�� �°� ����
        placeGameObject.Add( newObject );           // placeGameObject�� ����  

        GridData selectedData = database.objectData[selectedObjectIndex].ID == 0 ? floorData : funitureData;    // ��ġ�� ������Ʈ�� ������ ���� � �׸��� �����͸� ������� ����
                                                                                                                // ID�� 0�̸� �ٴڰ� ���õ� ������ (floorData)�� ����ϰ� �׷��� ������ (FuniotureData)�� ���
        selectedData.AddObjectAt(                   // ���õ� ������Ʈ�� ��ġ ������ selectedData�� �߰� 
            gridPosition,   // ������Ʈ�� ���۵Ǵ� �׸��� ��ġ
            database.objectData[selectedObjectIndex].Size,  //������Ʈ�� �����ϴ� �׸��� ���� ũ��
            database.objectData[selectedObjectIndex].ID,    // ������Ʈ�� ID
            placeGameObject.Count - 1);             // ����Ʈ������ ������Ʈ �ε��� .����Ʈ�� �߰��� �����̹Ƿ� ������ �ε��� 


        // ���
        // PlaceStructure �޼���� ����ڰ� ������Ʈ�� ��ġ�Ϸ��� ��ġ�� ��ȿ���� Ȯ���ϰ�, �� ��ġ�� ������Ʈ�� ������ ��, �׸��� �����Ϳ� ��ġ ������ �����.
        // �� �������� UI�� �浹�� �����ϰ�, ������Ʈ�� �׸��� ���� ��Ȯ�� ��ġ�ǵ��� ����.
        // �̸� ���� ����ڴ� ���� ������ ������Ʈ�� ���������� ��ġ�� �� ������, �ý����� ��ġ�� ������Ʈ�� ȿ�������� ����
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)       // ��ġ�Ϸ��� ������Ʈ�� �������� ���� ������ �׸��� �����͸� ���� 
    {
        GridData selectedData = database.objectData[selectedObjectIndex].ID == 0 ? floorData : funitureData;    // ID�� 0�̸� floorData, �ƴϸ� furnitureData ����

        return selectedData.CanPlaceObjectAt(gridPosition, database.objectData[selectedObjectIndex].Size);      // ���õ� ��ġ(gridPosition)�� ������Ʈ�� ��ġ�� �� �ִ��� ���� Ȯ�� 
    }

    private void StopPlacement()        // ������Ʈ ��ġ�� ���� �� �����ϴ� ���� 
    {
        selectedObjectIndex = -1;   // ���õ� ������Ʈ�� �ε����� �ʱ�ȭ
        gridVisualization.SetActive(false); // �׸��� �ð�ȭ ����
        cellIndicator.SetActive(false);     // �� �ð�ȭ ����
        inputManager.Onclicked -= PlaceStructure;   // PlaceStructure �޼��带 Ŭ���̺�Ʈ���� ����
        inputManager.OnExit -= StopPlacement;       // StopPlacement �޼��带 �����̺�Ʈ���� ����
    }

    private void Update()       // �ǽð����� ���콺 ��ġ�� �����ϰ� ��ġ���� ���θ� �ð������� ǥ��
    {
        if (selectedObjectIndex < 0) return;        // ���õ� ������Ʈ�� ���� ���, �޼����� ������ �κ��� �������� �ʰ� ����

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();      // ���� ���콺 Ŀ���� ���� ��ǥ ��ġ�� ������
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);          // ���콺 ��ġ�� �׸��� �� ��ǥ�� ��ȯ

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);     // ���� ������ ��ġ���� ������Ʈ�� ��ġ�� �� �ִ��� Ȯ��
        previewRenderer.material.color = placementValidity ? Color.white : Color.red;           // ��ġ���� ���θ� �ð������� ǥ�� ��ġ���� == white �Ұ��� == red

        mouseIndicator.transform.position = mousePosition;          // ���콺 �ε��������� ��ġ�� ���콺 Ŀ�� ��ġ�� ������Ʈ
        cellIndicator.transform.position = grid.CellToWorld(gridPosition);  // �� �ε��������� ��ġ�� �� ��ġ�� ������Ʈ
    }
}
