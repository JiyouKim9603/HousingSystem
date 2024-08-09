using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();            // Vector3Int�� Ű�� �ϰ�, �ش� ��ġ�� ��ġ�� ������Ʈ�� ������ ��� �ִ� PlacementData��ü�� ������ ����. � ������Ʈ�� � ��ġ�� ��ġ�Ǿ����� ����

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int id, int placedObjectIndex)      // Ư����ġ�� ������Ʈ�� ��ġ girdPostion : ��ġ�� ���� ��ġ�� ������
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);           // ������Ʈ�� ������ ��� ���� ��ġ�� ���
        PlacementData data = new PlacementData(positionToOccupy, id, placedObjectIndex);

        foreach(var pos in positionToOccupy)
        {
                if (placedObjects.ContainsKey(pos))
                    throw new System.Exception($"Dictionary already contains this cell position {pos}");
                placedObjects[pos] = data;  // ��� ��ġ�� ����ִٸ� �ش� ��ġ�� placementData ��ü ����
            
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPostion, Vector2Int objectSize)      // ������Ʈ�� �����ϰ� �� ��� ���� ��ǥ�� ����Ͽ� ����Ʈ�� ��ȯ
    {
        List<Vector3Int> returnVal = new();
        for(int x = 0; x < objectSize.x; x++)
        {
            for(int y = 0; y< objectSize.y; y++)
            {
                returnVal.Add(gridPostion + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)        // Ư����ġ�� ������Ʈ�� ��ġ�� �� �ִ��� Ȯ���ϴ� ���� 
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach(var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                return false;
        }
        return true;
    }

    public class PlacementData                  // ������Ʈ�� ���� �󿡼� �����ϴ� ������ ���õ� ������ ���� 
    {
        public List<Vector3Int> occupiedPositions;      // ������Ʈ�� �����ϴ� ��� ���� ��ǥ�� ����Ʈ�� ���� 
        public int id { get; private set; }
        public int placedObjectIndex { get; private set; }      // ������Ʈ�� ��ġ�� ������ �ε����� ��Ÿ��
        
        public PlacementData(List<Vector3Int> occupiedPositions, int id, int placedObjectIndex)
        {
            this.occupiedPositions = occupiedPositions;
            this.id = id;
            this.placedObjectIndex = placedObjectIndex;
        }
    }
}
