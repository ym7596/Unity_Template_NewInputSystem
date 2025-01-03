using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEditor : MonoBehaviour
{
    [SerializeField]  private InputManager _inputManager;
    [SerializeField] private Camera _camTransform;

    private RoomUtils _roomUtils;
    private CameraController _camController;
    private TouchPhases phase;
    // Start is called before the first frame update

    private GameObject testTarget;
    private Items _targetItem;

    private Vector3 _touchPos = Vector3.zero;
    
    private bool begin = false;
    

    private void Start()
    {
        _camController = new CameraController(_camTransform.transform,_inputManager);
        _roomUtils = new RoomUtils();
    }

    private void OnEnable()
    {
        _inputManager.OnLeftMouseState += SetPhase;
        _inputManager.OnRightClickedReleased += SetCancel;
    }

    private void SetCancel()
    {
        _roomUtils.SetDefault();
    }

    private void SetPhase(TouchPhases phase)
    {
        this.phase = phase;
        switch (phase)
        {
            case TouchPhases.None:
            {
                
            }
                break;
            case TouchPhases.Began:
            {

            }
                break;
            case TouchPhases.Moved:
            {

            }
                break;
            case TouchPhases.Ended:
            {
                Debug.Log("end");
                if (_inputManager.rayhit.collider)
                {
                    if (_inputManager.rayhit.collider.gameObject.layer == LayerMask.NameToLayer("Item"))
                    {
                       _roomUtils.SetCurrentObject(_inputManager.rayhit.collider.gameObject);
                    }
                }
                else
                {
                    testTarget = null;
                }
            }
                break;
        }
    }
    
    void Update()
    { 
        _camController.Update();
        /*if (phase == TouchPhases.None && _roomUtils.GetObject == null)
            return;*/
        if (_roomUtils.IsObject == false)
            return;
        _touchPos = _inputManager.Pos;

        var ray = _camTransform.ScreenPointToRay(_touchPos);
        if (_roomUtils.GetObject)
        {

            RaycastHit _hit;

            int layerIndex = LayerMask.NameToLayer("Ground");
            if (layerIndex == -1)
            {
                Debug.LogError("Layer 'Ground' does not exist or is not set properly.");
                return;
            }

            if (Physics.Raycast(ray, out _hit, 500, 1 << layerIndex))
            {
                _roomUtils.MoveObject(_hit.point);
            }
            else
            {
                Debug.Log("Raycast did not hit any object in 'Ground' layer.");
            }
        }

    }
}
