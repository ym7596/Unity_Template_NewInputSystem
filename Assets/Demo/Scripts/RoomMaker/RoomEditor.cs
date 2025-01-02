using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEditor : MonoBehaviour
{
    [SerializeField]  private InputManager _inputManager;
    [SerializeField] private Transform _camTransform;

    private RoomUtils _roomUtils;
    private CameraController _camController;
    private TouchPhases phase;
    // Start is called before the first frame update

    private GameObject testTarget;
    private Items _targetItem;
   
    private bool begin = false;

    private void Start()
    {
        _camController = new CameraController(_camTransform,_inputManager);
        _roomUtils = new RoomUtils();
    }

    private void OnEnable()
    {
        _inputManager.OnLeftMouseState += SetPhase;
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

    // Update is called once per frame
    void Update()
    { 
        _camController.Update();
        if (phase == TouchPhases.None)
            return;

        
      
    }
}
