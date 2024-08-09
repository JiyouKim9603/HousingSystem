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
    private void PopulateObjectList()       // 오브젝트 데이터베이스에 저장된 오브젝트들에 대해 UI 버튼을 동적으로 생성하고, 각 버튼이 클릭 될 때 특정 동작을 수행하도록 설정하는 역할 
    {
        foreach(var objectData in objectsDatabase.objectData)       // 오브젝트 데이터베이스에 저장된 각 오브젝트에 대해 반복작업을 수행
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
