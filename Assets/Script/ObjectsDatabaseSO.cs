using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObjectsDatabaseSO : ScriptableObject
{
    public List<ObjectData> objectData;
}

[Serializable]      // �� Ŭ������ �ν��Ͻ��� ����ȭ �� �� �ֵ��� ������
public class ObjectData
{
    [field: SerializeField]     //{ get; private set; } ������ [SerializeField] ���� [field: SerializeField] �� ����Ѵ�
    public string Name { get; private set; }
    [field: SerializeField]
    public int ID { get; private set; }
    [field: SerializeField]
    public Sprite Icon { get; private set; }
    [field: SerializeField]
    public Vector2Int Size { get; private set; } = Vector2Int.one;  // �⺻���� {1,1}
    [field: SerializeField]
    public GameObject Prefab { get; private set; }
}