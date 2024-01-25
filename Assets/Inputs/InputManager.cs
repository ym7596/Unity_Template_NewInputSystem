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
    Move,
    Held,
    Ended,
    Canceled
}

public enum PressedState
{
    None,       // 터치가 없는 상태
    Single,     // 단일 터치
    Multiple    // 다중 터치
}


// InputManager 클래스 내부에 PressedState를 반환하는 메소드를 추가

public class InputManager : MonoBehaviour
{

    private DduRInput dduRInput;

    private Vector2 _lastPos;
   
    
    public RaycastHit rayhit { get; private set; }
    public Vector2 PrimaryPosition { get; private set; }
    public Vector2 Pos { get; private set; }
    
   

    public TouchPhases CurrentPhase { get; private set; } = TouchPhases.None;
  
    public bool IsUITouched { get; private set; } = false;
    public bool LeftIsClicked => dduRInput.dduRAction.Tab.IsPressed();
    public bool RightIsClicked => dduRInput.dduRAction.Special.IsPressed();
    public bool ScrollIsClicked => dduRInput.dduRAction.Wheel.IsPressed();
  
    public PressedState CurrentPressedState { get; private set; }

    public  Action<bool> ActionScrollWheel;//스크롤 False => 아래 True => 위
   
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
        }
    }

    private void OnEnable()
    {
        dduRInput.dduRAction.Tab.started += OnTabStarted;
        dduRInput.dduRAction.Tab.performed += OnTabPerformed;
        dduRInput.dduRAction.Tab.canceled += OnTabCanceled;

        dduRInput.dduRAction.Position.started += OnPositionStarted;
        dduRInput.dduRAction.Position.performed += OnPositionPerformed;
        dduRInput.dduRAction.Position.canceled += OnPositionCanceled;

        dduRInput.dduRAction.Special.started += OnRightTabStarted;
        dduRInput.dduRAction.Special.performed += OnRightTabPerformed;
        dduRInput.dduRAction.Special.canceled += OnRightTabCanceled;

        dduRInput.dduRAction.Wheel.performed += OnScrollClick;

        dduRInput.dduRAction.Scroll.performed += OnScrollPerformed;
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
        
        dduRInput.dduRAction.Wheel.performed -= OnScrollClick;

        dduRInput.dduRAction.Scroll.performed -= OnScrollPerformed;
        dduRInput.Disable();
    }
    private bool IsTouchOrClickActive()
    {
        // 터치가 활성화되었는지 확인
        bool isTouchActive = Input.touchCount > 0;
    
        // 마우스 클릭이 활성화되었는지 확인 (0은 기본적으로 왼쪽 버튼)
        bool isClickActive = Input.GetMouseButton(0);

        return isTouchActive || isClickActive;
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
        if (context.interaction is HoldInteraction)
        {
            if (CurrentPhase == TouchPhases.Held)
                return;
            _lastPos = Pos;
            PhaseChange(TouchPhases.Held);
            
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

#region SpecialTab (Right Button)

    private void OnRightTabStarted(InputAction.CallbackContext context)
    {
        
    }
    private void OnRightTabPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("rightclick");
    }
    private void OnRightTabCanceled(InputAction.CallbackContext context)
    {
        
    }

#endregion

#region Scroll

    private void OnScrollClick(InputAction.CallbackContext context)
    {
     
    }

    private void OnScrollPerformed(InputAction.CallbackContext context)
    {
        bool isScroll = !(context.ReadValue<float>() < 0);
        ActionScrollWheel?.Invoke(isScroll);
    }


#endregion
    private void PhaseChange(TouchPhases phase)
    {
        if (IsUITouched)
        {
            Debug.Log("UI!");
            CurrentPhase = TouchPhases.None;
            return;
        }
           
        if(CurrentPhase != phase)
            CurrentPhase = phase;
    }

    private RaycastHit GetRayHit(Vector2 pos)
    {
        RaycastHit hit = default;
        if (Camera.main != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(pos);
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.collider.name);
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
