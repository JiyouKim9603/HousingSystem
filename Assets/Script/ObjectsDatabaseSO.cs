using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObjectsDatabaseSO : ScriptableObject
{
    public List<ObjectData> objectData;
}

[Serializable]      // 이 클래스의 인스턴스를 직렬화 할 수 있도록 도와줌
public class ObjectData
{
    [field: SerializeField]     //{ get; private set; } 쓰려면 [SerializeField] 말고 [field: SerializeField] 를 써야한다
    public string Name { get; private set; }
    [field: SerializeField]
    public int ID { get; private set; }
    [field: SerializeField]
    public Sprite Icon { get; private set; }
    [field: SerializeField]
    public Vector2Int Size { get; private set; } = Vector2Int.one;  // 기본값은 {1,1}
    [field: SerializeField]
    public GameObject Prefab { get; private set; }
}