using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();            // Vector3Int를 키로 하고, 해당 위치에 배치된 오브젝트의 정보를 담고 있는 PlacementData객체를 값으로 지정. 어떤 오브젝트가 어떤 위치에 배치되었는지 관리

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int id, int placedObjectIndex)      // 특정위치에 오브젝트를 배치 girdPostion : 배치될 격자 위치의 시작점
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);           // 오브젝트가 차지할 모든 격자 위치를 계산
        PlacementData data = new PlacementData(positionToOccupy, id, placedObjectIndex);

        foreach(var pos in positionToOccupy)
        {
                if (placedObjects.ContainsKey(pos))
                    throw new System.Exception($"Dictionary already contains this cell position {pos}");
                placedObjects[pos] = data;  // 모든 위치가 비어있다면 해당 위치에 placementData 객체 저장
            
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPostion, Vector2Int objectSize)      // 오브젝트가 차지하게 될 모든 격자 좌표를 계산하여 리스트로 반환
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

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)        // 특정위치에 오브젝트를 배치할 수 있는지 확인하는 역할 
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach(var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                return false;
        }
        return true;
    }

    public class PlacementData                  // 오브젝트가 격자 상에서 차지하는 공간과 관련된 정보를 저장 
    {
        public List<Vector3Int> occupiedPositions;      // 오브젝트가 차지하는 모든 격자 좌표를 리스트로 저장 
        public int id { get; private set; }
        public int placedObjectIndex { get; private set; }      // 오브젝트가 배치된 순서나 인덱스를 나타냄
        
        public PlacementData(List<Vector3Int> occupiedPositions, int id, int placedObjectIndex)
        {
            this.occupiedPositions = occupiedPositions;
            this.id = id;
            this.placedObjectIndex = placedObjectIndex;
        }
    }
}
