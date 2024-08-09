using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class SampleButton : MonoBehaviour
{
    [SerializeField] private PlacementSystem placementSystem;
    [SerializeField] private ObjectsDatabaseSO objectsDatabase;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonParent;

    private void Start()
    {
        PopulateObjectList();
    }
    private void PopulateObjectList()       // ������Ʈ �����ͺ��̽��� ����� ������Ʈ�鿡 ���� UI ��ư�� �������� �����ϰ�, �� ��ư�� Ŭ�� �� �� Ư�� ������ �����ϵ��� �����ϴ� ���� 
    {
        foreach(var objectData in objectsDatabase.objectData)       // ������Ʈ �����ͺ��̽��� ����� �� ������Ʈ�� ���� �ݺ��۾��� ����
        {
            GameObject newButton = Instantiate(buttonPrefab, buttonParent);
            newButton.name = objectData.Name;

            Image iconImage = newButton.GetComponentInChildren<Image>();
            if(iconImage != null )
            {
                iconImage.sprite = objectData.Icon;
            }

            Button buttonComponent = newButton.GetComponent<Button>();
            if(buttonComponent != null)
            {
                buttonComponent.onClick.AddListener(() => OnButtonClicked(objectData, objectData.ID));
            }
        }
    }

    private void OnButtonClicked(ObjectData objectData, int id)
    {
        Debug.Log("Button clicked : " + objectData.Name);
        placementSystem.StartPlacement(id);
    }
}
