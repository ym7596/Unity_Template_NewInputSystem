using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomUtils
{
    private Items _currentObject = null;
    private HistoryController _historyController;
    private float gridsize = .5f;
    public GameObject GetObject => _currentObject.gameObject;

    public bool IsObject => _currentObject != null ? true : false;
    public RoomUtils()
    {
        Init();
    }
    
    private void Init()
    {
        _historyController = new HistoryController();
        UIcanvas.Instance.onAction_Redo += Redo;
        UIcanvas.Instance.onAction_Undo += Undo;
    }
    /*
     *  클릭시 오브젝트 설정
     *  우클릭시 되돌아가기
     *  undo redo 시스템
     */

    public void SetDefault()
    {
        if (_currentObject == null)
            return;
       // UpdateTransform();
        _currentObject.SetDefault();
        _currentObject = null;
    }

    public void SetCurrentObject(GameObject obj)
    {
        _currentObject = obj.GetComponent<Items>();
      
        _currentObject.SetClick(true);
        ApplyTranform();
    }

    public void MoveObject(Vector3 pos)
    {
        _currentObject.transform.position = new Vector3(RoundToNearestGrid(pos.x), 0, RoundToNearestGrid(pos.z));
    }
    
    private float RoundToNearestGrid(float pos)
    {
        float xDiff = pos % gridsize;
        pos -= xDiff;
        if (xDiff > (gridsize / 2))
        {
            pos += gridsize;
        }
        return pos;
    }

    public void UpdateTransform()
    {
        ApplyTranform();
    }
    
    
    
#region UndoRedo 시스템
    public void ApplyTranform()
    {
        ItemHistory itemHistory = new ItemHistory(GetObject, _currentObject.transform);
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
    
#endregion
}
