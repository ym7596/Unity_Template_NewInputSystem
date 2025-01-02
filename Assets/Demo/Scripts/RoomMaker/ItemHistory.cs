using UnityEngine;

public class TransformInfo
{
    public Vector3 pos;
    public Vector3 rot;
    public Vector3 scl;

    public TransformInfo(Transform t)
    {
        pos = t.position;
        scl = t.localScale;
        rot = t.eulerAngles;
    }
}

public class ItemHistory : IHistory
{
    private GameObject _target;
    
    private Vector3 previousPosition;
    private Vector3 previousScale;
    private Quaternion previousRotation;

    private Vector3 newPosition;
    private Vector3 newScale;
    private Quaternion newRotation;

    public ItemHistory(GameObject target, Transform t)
    {
        this._target = target;

        this.previousPosition = target.transform.position;
        this.previousScale = target.transform.localScale;
        this.previousRotation = target.transform.localRotation;

        newPosition = t.position;
        newRotation = t.localRotation;
        newScale = t.localScale;

    }
    
    public void Undo()
    {
        _target.transform.position = newPosition;
        _target.transform.localScale = newScale;
        _target.transform.rotation = newRotation;
    }

    public void Execute()
    {
        _target.transform.position = previousPosition;
        _target.transform.localScale = previousScale;
        _target.transform.rotation = previousRotation;
    }
}
