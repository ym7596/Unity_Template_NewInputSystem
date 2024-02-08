using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController
{
    private InputManager _inputs;
    private Transform _camTransform;
    
    private float _maxSpeed = 10f;

    private float _speed;

    private float _accelration = 10f;

    private float _damping = 15f;
    
    //rotation
    private float _normalizedConst = .1f;

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
    private Vector3 _movementValueVertical;

    public CameraController(Transform cam, InputManager inputManager)
    {
        _camTransform = cam;
        _inputs = inputManager;
        Init();
    }
    
    private void Init()
    {
        _inputs.OnActionWASD += SetWASD;
        _inputs.OnActionScrollWheel += SetWheelScroll;
        _inputs.OnRightClickedReleased += DragClear;
        _inputs.OnKeyboardListener += GetKeysValue;
    }

    public void Update()
    {
        GetKeyBoardMovement(); //WASD버튼 이동
        GetKeyBoardMovementUp(); //QE 이동
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
        
        _camTransform.Translate(wheelValue,Space.Self);
    }
    private void SetWASD(Vector2 dir)
    {
        _movementValue = dir;
    }

    private void SetQE(bool isUp)
    {
        if (isUp)
            _movementValueVertical = Vector3.up;
        else
        {
            _movementValueVertical = Vector3.down;
        }
    }
    
    private void SetRotateByRightButton()
    {
        if (_inputs.RightIsClicked == false || _inputs.ScrollIsClicked == true)
            return;
        if(_startDrag == Vector3.zero)
            _startDrag = _inputs.PrimaryRightPos;
        Vector3 currentPos = _inputs.Pos;

        Vector3 dragDelta = currentPos - _startDrag;

        float rotX = -dragDelta.y * _normalizedConst;
        float rotY = dragDelta.x * _normalizedConst;

       
        _camTransform.Rotate(Vector3.up, rotY, Space.Self); // y축 회전
        _camTransform.Rotate(Vector3.right, rotX, Space.Self); 
      
      float z = _camTransform.eulerAngles.z;
       _camTransform.Rotate(0,0,-z);
        _startDrag = currentPos;
    }

    private void GetScrollButtonMovement()
    {
        if (_inputs.ScrollIsClicked == false || _inputs.RightIsClicked == true)
            return;

        if (_startDrag == Vector3.zero)
            _startDrag = _inputs.PrimaryWheelPos;
        Vector3 currentPos = _inputs.Pos;
        Vector3 dragDelta = currentPos - _startDrag;

        dragDelta = dragDelta.normalized * _normalizedConst;
        Vector3 moveDir = new Vector3(-dragDelta.x, -dragDelta.y, 0) * _dragSpeed;
        _camTransform.Translate(moveDir,Space.Self);
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
                    SetQE(true);
                    break;
                case "E":
                   SetQE(false);
                    break;
                case "Space":
                    break;
                case "F":
                    break;
                default:
                    break;
            }
        }
        else if (phase == InputActionPhase.Canceled)
        {
            switch (keyName)
            {
                case "Q":
                case "E":
                    _movementValueVertical = Vector3.zero;
                    break; 
            }
        }
    }

#region WASDQE MOVE

    private void GetKeyBoardMovementUp()
    {
        _speed = Mathf.Lerp(_speed, _maxSpeed, Time.deltaTime * _accelration);
        _camTransform.Translate(_movementValueVertical *_speed* Time.deltaTime,Space.World);
    }
    private void GetKeyBoardMovement()
    {
        Vector3 inputValue = _movementValue.x * GetCameraRight(_camTransform)
                             + _movementValue.y * GetCameraForward(_camTransform);
      
        inputValue = inputValue.normalized;

        if (inputValue.sqrMagnitude > 0.1f) 
            _targetPosition += inputValue;
    }
    
    private void UpdateBasePosition()
    {
        if (_targetPosition.sqrMagnitude > 0.1f)
        {
            _speed = Mathf.Lerp(_speed, _maxSpeed, Time.deltaTime * _accelration);
            _camTransform.position += _targetPosition * _speed * Time.deltaTime;
        }
        else
        {
            _horizontalVelocity = Vector3.Lerp(_horizontalVelocity, Vector3.zero, Time.deltaTime * _damping);
            _camTransform.position += _horizontalVelocity * Time.deltaTime;
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
        return transform.TransformDirection(Vector3.forward);
    }

    private Vector3 GetCameraUp(Transform transform)
    {
        return transform.TransformDirection(Vector3.up);
    }
#endregion




}
