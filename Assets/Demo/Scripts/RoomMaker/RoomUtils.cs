using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomUtils
{
    private GameObject _currentObject;
    private HistoryController _historyController;

    public GameObject GetCurrentObject => _currentObject;

    public RoomUtils()
    {
        Init();
    }
    
    private void Init()
    {
        _historyController = new HistoryController();
    }
    /*
     *  클릭시 오브젝트 설정
     *  우클릭시 되돌아가기
     *  undo redo 시스템
     */
    public void Began()
    {
        
    }

    public void SetCurrentObject(GameObject obj)
    {
        _currentObject = obj;
        Items item = _currentObject.GetComponent<Items>();
        item.SetClick();
        ApplyTranform(_currentObject.transform);
    }

    public void MoveObject(Vector3 pos)
    {
        _currentObject.transform.position = pos;
    }


    public void ApplyTranform(Transform newTransform)
    {
        ItemHistory itemHistory = new ItemHistory(_currentObject, _currentObject.transform);
        _historyController.ExecuteHistory(itemHistory);
    }

    public void Undo()
    {
        _historyController.Undo();
    }

    public void Redo()
    {
        _historyController.Redo();
    }
}
