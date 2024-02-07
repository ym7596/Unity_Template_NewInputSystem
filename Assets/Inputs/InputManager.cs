using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public enum TouchPhases
{
    None,
    Began,
    Moved,
    Held,
    Canceled
}

public enum PressedState
{
    None,       // 터치가 없는 상태
    Single,     // 단일 터치
    Multiple    // 다중 터치
}

public enum CameraMovement
{
    None,
    Front,
    Back,
    Right,
    Left,
    Up,
    Down
}

// InputManager 클래스 내부에 PressedState를 반환하는 메소드를 추가

public class InputManager : MonoBehaviour
{

    private DduRInput dduRInput;

    private Vector2 _lastPos;

    public InputAction movement;
    
    public RaycastHit rayhit { get; private set; }
    public Vector2 PrimaryPosition { get; private set; }
    public Vector2 PrimaryRightPos { get; private set; }
    public Vector2 PrimaryWheelPos { get; private set; }
    public Vector2 Pos { get; private set; }
    
    public Vector2 DragDelta { get; private set; }
    
 
    public TouchPhases CurrentPhase { get; private set; } = TouchPhases.None;
    public TouchPhases CurrentSpecailPhase { get; private set; } = TouchPhases.None;
    public bool IsUITouched { get; private set; } = false;
    public bool LeftIsClicked => dduRInput.dduRAction.Tab.IsPressed();
    public bool RightIsClicked => dduRInput.dduRAction.Special.IsPressed();
    public bool ScrollIsClicked => dduRInput.dduRAction.WheelButton.IsPressed();
  
    
    public PressedState CurrentPressedState { get; private set; }

    public Action<float> OnActionScrollWheel;//스크롤 False => 아래 True => 위
    public Action OnActionFlyToObject;
    public Action OnRightClickedReleased;
    public Action<Vector2> OnActionWASD;
    public Action<Vector2> OnActionQE;
    public Action<InputActionPhase, string> OnKeyboardListener;
    private void UpdatePressedState()
    {
        if (Input.touchCount > 1)
        {
            CurrentPressedState = PressedState.Multiple;
        }
        else if (Input.touchCount == 1 || Input.GetMouseButton(0))
        {
            CurrentPressedState = PressedState.Single;
        }
        else
        {
            CurrentPressedState = PressedState.None;
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        dduRInput = new DduRInput();
        dduRInput.Enable();
        CurrentPhase = TouchPhases.None;
    }

    private void Update()
    {
        if (!IsTouchOrClickActive())
        {
            UpdatePressedState();
            PhaseChange(TouchPhases.None);
            SpecialChange(TouchPhases.None);
        }
    }

    private void OnEnable()
    {
        movement = dduRInput.dduRAction.Move;
        dduRInput.dduRAction.Tab.started += OnTabStarted;
        dduRInput.dduRAction.Tab.performed += OnTabPerformed;
        dduRInput.dduRAction.Tab.canceled += OnTabCanceled;

        dduRInput.dduRAction.Position.started += OnPositionStarted;
        dduRInput.dduRAction.Position.performed += OnPositionPerformed;
        dduRInput.dduRAction.Position.canceled += OnPositionCanceled;

        dduRInput.dduRAction.Special.started += OnRightTabStarted;
        dduRInput.dduRAction.Special.performed += OnRightTabPerformed;
        dduRInput.dduRAction.Special.canceled += OnRightTabCanceled;

        dduRInput.dduRAction.Drag.performed += OnDragPerformed;
   
        dduRInput.dduRAction.WheelButton.started += OnScrollBtnStarted;
        dduRInput.dduRAction.WheelButton.canceled += OnScrollBtnCanceled;

        dduRInput.dduRAction.Scroll.performed += OnScrollPerformed;
        dduRInput.dduRAction.Scroll.canceled += OnScrollCanceled;

        dduRInput.dduRAction.Move.performed += OnWASDPerformed;
        dduRInput.dduRAction.Move.canceled += OnWASDCanceled;

        dduRInput.dduRAction.Keys.performed += OnKeysPerformed;
        dduRInput.dduRAction.Keys.canceled += OnKeysPerformed;
        
        dduRInput.Enable();
    }

    private void OnDisable()
    {
        dduRInput.dduRAction.Tab.started -= OnTabStarted;
        dduRInput.dduRAction.Tab.performed -= OnTabPerformed;
        dduRInput.dduRAction.Tab.canceled -= OnTabCanceled;
        
        dduRInput.dduRAction.Position.started -= OnPositionStarted;
        dduRInput.dduRAction.Position.performed -= OnPositionPerformed;
        dduRInput.dduRAction.Position.canceled -= OnPositionCanceled;
        
        dduRInput.dduRAction.Special.started -= OnRightTabStarted;
        dduRInput.dduRAction.Special.performed -= OnRightTabPerformed;
        dduRInput.dduRAction.Special.canceled -= OnRightTabCanceled;

        dduRInput.dduRAction.WheelButton.started -= OnScrollBtnStarted;
        dduRInput.dduRAction.WheelButton.canceled -= OnScrollBtnCanceled;
        
        dduRInput.dduRAction.Scroll.performed -= OnScrollPerformed;
        dduRInput.dduRAction.Scroll.canceled -= OnScrollCanceled;
        
        dduRInput.dduRAction.Move.performed -= OnWASDPerformed;
        dduRInput.dduRAction.Move.canceled -= OnWASDCanceled;
        
        dduRInput.dduRAction.Keys.performed -= OnKeysPerformed;
        dduRInput.dduRAction.Keys.canceled -= OnKeysPerformed;
        
        dduRInput.Disable();
    }
    private bool IsTouchOrClickActive()
    {
        // 터치가 활성화되었는지 확인
        bool isTouchActive = Input.touchCount > 0;
    
        // 마우스 클릭이 활성화되었는지 확인 (0은 기본적으로 왼쪽 버튼)
        bool isClickActive = Input.GetMouseButton(0);
        bool isRClickActive = Input.GetMouseButton(1);

        return isTouchActive || isClickActive || isRClickActive;
    }
    
#region Tab

    private void OnTabStarted(InputAction.CallbackContext context)
    {
        IsUITouched = IsUITouch();
        UpdatePressedState();
        PhaseChange(TouchPhases.Began);
    }

    private void OnTabPerformed(InputAction.CallbackContext context)
    {
        UpdatePressedState();
        PhaseChange(TouchPhases.Moved);
        if (context.interaction is HoldInteraction)
        {
            if (CurrentPhase == TouchPhases.Moved)
                return;
            _lastPos = Pos;
            PhaseChange(TouchPhases.Moved);
            
        }
    }
    
    private void OnTabCanceled(InputAction.CallbackContext context)
    {
        rayhit = GetRayHit(Pos);

        PhaseChange(TouchPhases.Canceled);
    }

#endregion

#region Position

    private void OnPositionStarted(InputAction.CallbackContext context)
    {
        PrimaryPosition = context.ReadValue<Vector2>();
    }
    private void OnPositionPerformed(InputAction.CallbackContext context)
    {
        Pos = context.ReadValue<Vector2>();
    }
    private void OnPositionCanceled(InputAction.CallbackContext context)
    { }
#endregion

#region DragDelta

    private void OnDragPerformed(InputAction.CallbackContext context)
    {
        DragDelta = context.ReadValue<Vector2>();
   //     Debug.Log($"DragDelta X : {DragDelta.x}");
      //  Debug.Log($"DragDelta X : {DragDelta.y}");
    }

#endregion

#region Key Mapping

    private void OnPressedKeyCord_F(InputAction.CallbackContext context)
    {
        Debug.Log("F Key Pressed!");
        OnActionFlyToObject?.Invoke();
    }


#endregion

#region SpecialTab (Right Button)

private void OnRightTabStarted(InputAction.CallbackContext context)
{
    PrimaryRightPos = Pos;
    SpecialChange(TouchPhases.Began);
}
private void OnRightTabPerformed(InputAction.CallbackContext context)
{
    Debug.Log("rightclick");
    SpecialChange(TouchPhases.Moved);
}
private void OnRightTabCanceled(InputAction.CallbackContext context)
{
    OnRightClickedReleased?.Invoke();
    SpecialChange(TouchPhases.Canceled);
}

#endregion

#region Scroll (include ScrollButton)


    private void OnScrollPerformed(InputAction.CallbackContext context)
    {
        bool isScroll = !(context.ReadValue<float>() < 0);
        OnActionScrollWheel?.Invoke(context.ReadValue<float>());
    }

    private void OnScrollCanceled(InputAction.CallbackContext context)
    {
        OnActionScrollWheel?.Invoke(0);
        
    }

    private void OnScrollBtnStarted(InputAction.CallbackContext context)
    {
        PrimaryWheelPos = Pos;
    }
    
    private void OnScrollBtnCanceled(InputAction.CallbackContext context)
    {
       // PrimaryWheelPos = Vector2.zero;
        OnRightClickedReleased?.Invoke();
    }

#endregion

#region WASDQE

    private void OnWASDPerformed(InputAction.CallbackContext context)
    {
        var keyValue = context.ReadValue<Vector2>();
        OnActionWASD?.Invoke(keyValue);
        
     
        //W : (0,1)
        //A : (-1,0)
        //S : (0,-1)
        //D : (1,0)
    }
    
    private void OnWASDCanceled(InputAction.CallbackContext context)
    {
        OnActionWASD?.Invoke(Vector2.zero);
    }

    private void OnKeysPerformed(InputAction.CallbackContext context)
    {
        var phase = context.phase;
        var key = context.control.displayName;
     
        OnKeyboardListener?.Invoke(phase,key);
    }

  

   

#endregion
    private void PhaseChange(TouchPhases phase)
    {
        if (IsUITouched)
        {
            Debug.Log("UI!");
            CurrentPhase = TouchPhases.None;
            IsUITouched = false;
            return;
        }
           
        if(CurrentPhase != phase)
            CurrentPhase = phase;
    }

    private void SpecialChange(TouchPhases phase)
    {
        CurrentSpecailPhase = phase;
        /*if (CurrentSpecailPhase != phase || phase == TouchPhases.Moved)
            CurrentSpecailPhase = phase;*/
    }

    private RaycastHit GetRayHit(Vector2 pos)
    {
        RaycastHit hit = default;
        if (Camera.main != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(pos);
            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log(hit.collider.name);
                return hit;
            }
        }
        return hit;
    }

    private bool IsUITouch()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Pos
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        
        EventSystem.current.RaycastAll(pointerData,raycastResults);

        if (raycastResults.Count > 0)
        {
            return true;
        }

        return false;
    }
}
