using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController
{
    private InputManager inputs;
    private Transform camTransform;
    
    private float maxSpeed = 10f;

    private float speed;

    private float accelration = 10f;

    private float _damping = 15f;
    
    //rotation
    private float normalizedConst = .1f;

    private float _dragSpeed = 2f;
    //screen edge motion

    //value set in various functions
    private Vector3 _targetPosition;
    
    
    //use to track and maintain velocity w/o a rigidbody
    private Vector3 _horizontalVelocity;
    private Vector3 _lastPosition;
    
    //dragging action started
    private Vector3 _startDrag = Vector3.zero;
   
    private Vector2 _movementValue;

    public CameraController(Transform cam, InputManager inputManager)
    {
        camTransform = cam;
        inputs = inputManager;
        Init();
    }
    
    private void Init()
    {
        inputs.OnActionWASD += SetWASD;
        inputs.OnActionScrollWheel += SetWheelScroll;
        inputs.OnRightClickedReleased += DragClear;
        inputs.OnKeyboardListener += GetKeysValue;
    }

    public void Update()
    {
        GetKeyBoardMovement(); //WASD버튼 이동
        GetScrollButtonMovement(); //스크롤버튼 드래그 이동
        SetRotateByRightButton(); // 우클릭 회전
      
        UpdateBasePosition();
    }

    private void DragClear()
    {
        _startDrag = Vector3.zero; 
    }

    private void SetWheelScroll(float value)
    {
        float val = value / 120;
        Vector3 wheelValue = Vector3.forward * val;
        
        camTransform.Translate(wheelValue,Space.Self);
    }
    private void SetWASD(Vector2 dir)
    {
        _movementValue = dir;
    }
    
    private void SetRotateByRightButton()
    {
        if (inputs.RightIsClicked == false || inputs.ScrollIsClicked == true)
            return;
        if(_startDrag == Vector3.zero)
            _startDrag = inputs.PrimaryRightPos;
        Vector3 currentPos = inputs.Pos;

        Vector3 dragDelta = currentPos - _startDrag;

        float rotX = -dragDelta.y * normalizedConst;
        float rotY = dragDelta.x * normalizedConst;

       
        camTransform.Rotate(Vector3.up, rotY, Space.Self); // y축 회전
        camTransform.Rotate(Vector3.right, rotX, Space.Self); 
      
      float z = camTransform.eulerAngles.z;
       camTransform.Rotate(0,0,-z);
        _startDrag = currentPos;
    }

    private void GetScrollButtonMovement()
    {
        if (inputs.ScrollIsClicked == false || inputs.RightIsClicked == true)
            return;

        if (_startDrag == Vector3.zero)
            _startDrag = inputs.PrimaryWheelPos;
        Vector3 currentPos = inputs.Pos;
        Vector3 dragDelta = currentPos - _startDrag;

        dragDelta = dragDelta.normalized * normalizedConst;
        Vector3 moveDir = new Vector3(-dragDelta.x, -dragDelta.y, 0) * _dragSpeed;
        camTransform.Translate(moveDir,Space.Self);
        //좌우 X축 -+  상하 Y축 +-
        
        _startDrag = currentPos;
    }

    private void GetSpaceAndLeftButtonMovement()
    {
        
    }

    private void GetKeysValue(InputActionPhase phase, string keyName)
    {
        if (phase == InputActionPhase.Performed)
        {
            switch (keyName)
            {
                case "Q":
                    break;
                case "E":
                    break;
                case "Space":
                    break;
                case "F":
                    break;
                default:
                    break;
            }
        }
    }

#region WASD MOVE
    private void GetKeyBoardMovement()
    {
        Vector3 inputValue = _movementValue.x * GetCameraRight(camTransform)
                             + _movementValue.y * GetCameraForward(camTransform);
      
        inputValue = inputValue.normalized;

        if (inputValue.sqrMagnitude > 0.1f)
            _targetPosition += inputValue;
    }
    
    private void UpdateBasePosition()
    {
        if (_targetPosition.sqrMagnitude > 0.1f)
        {
            speed = Mathf.Lerp(speed, maxSpeed, Time.deltaTime * accelration);
            camTransform.position += _targetPosition * speed * Time.deltaTime;
        }
        else
        {
            _horizontalVelocity = Vector3.Lerp(_horizontalVelocity, Vector3.zero, Time.deltaTime * _damping);
            camTransform.position += _horizontalVelocity * Time.deltaTime;
        }

        _targetPosition = Vector3.zero;
    }
    
    private Vector3 GetCameraRight(Transform transform)
    {
        Vector3 right = transform.right;
        right.y = 0;
        return right;
    }

    private Vector3 GetCameraForward(Transform transform)
    {
        return transform.transform.TransformDirection(Vector3.forward);
    }
#endregion




}
