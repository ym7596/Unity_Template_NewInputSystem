using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public enum TouchPhases
{
    None = 0,
    Began,
    Hold,
    Moved,
    Ended
}
public class InputManager : MonoBehaviour
{

    private DduRInput dduRInput;

    private Vector2 _lastPos;
    
    public RaycastHit rayhit { get; private set; }
    public Vector2 Pos { get; private set; }
    
    public TouchPhases CurrentPhase { get; private set; }

    public bool IsUITouched { get; private set; } = false;
    // Start is called before the first frame update
    void Awake()
    {
        dduRInput = new DduRInput();
        CurrentPhase = TouchPhases.None;
    }

    private void LateUpdate()
    {
        
#if UNITY_ANDROID
    if (Input.touchCount == 0 && CurrentPhase == TouchPhases.Ended)
    {
       CurrentPhase = TouchPhases.None;
        // Android에서의 터치 처리 코드
    }
#elif UNITY_STANDALONE
        if (!Input.GetMouseButtonDown(0) && CurrentPhase == TouchPhases.Ended)
        {
            CurrentPhase = TouchPhases.None;
            // PC에서의 마우스 처리 코드
        }
#endif
    }

    private void OnEnable()
    {
        dduRInput.Enable();

        dduRInput.dduRAction.Tab.started += OnTabStarted;
        dduRInput.dduRAction.Tab.performed += OnTabPerformed;
        dduRInput.dduRAction.Tab.canceled += OnTabCanceled;

        dduRInput.dduRAction.Position.started += OnPositionStarted;
        dduRInput.dduRAction.Position.performed += OnPositionPerformed;
        dduRInput.dduRAction.Position.canceled += OnPositionCanceled;
    }

    private void OnDisable()
    {
        dduRInput.Disable();
        dduRInput.dduRAction.Tab.started -= OnTabStarted;
        dduRInput.dduRAction.Tab.performed -= OnTabPerformed;
        dduRInput.dduRAction.Tab.canceled -= OnTabCanceled;
        
        dduRInput.dduRAction.Position.started -= OnPositionStarted;
        dduRInput.dduRAction.Position.performed -= OnPositionPerformed;
        dduRInput.dduRAction.Position.canceled -= OnPositionCanceled;
    }
    
    
#region Tab

    private void OnTabStarted(InputAction.CallbackContext context)
    {
        IsUITouched = IsUITouch();
       
        if (CurrentPhase == TouchPhases.Began)
            return;
        
        PhaseChange(TouchPhases.Began);
    }

    private void OnTabPerformed(InputAction.CallbackContext context)
    {
        if (context.interaction is HoldInteraction)
        {
            if (CurrentPhase == TouchPhases.Hold)
                return;
            _lastPos = Pos;
            PhaseChange(TouchPhases.Hold);
            
        }
    }
    
    private void OnTabCanceled(InputAction.CallbackContext context)
    {
        if (CurrentPhase == TouchPhases.Ended)
            return;
        PhaseChange(TouchPhases.Ended);
    }

#endregion

#region Position

    private void OnPositionStarted(InputAction.CallbackContext context)
    {
       
    }
    private void OnPositionPerformed(InputAction.CallbackContext context)
    {
        Pos = context.ReadValue<Vector2>();
        if(CurrentPhase == TouchPhases.Hold && _lastPos != Pos)
            PhaseChange(TouchPhases.Moved);
   
    }
    private void OnPositionCanceled(InputAction.CallbackContext context)
    {
      
    }


#endregion

  
    
    private void PhaseChange(TouchPhases phase)
    {
        if (IsUITouched)
        {
            Debug.Log("UI!");
            return;
        }
           
        CurrentPhase = phase;
        switch (CurrentPhase)
        {
           case TouchPhases.Ended:
            {
                
                rayhit = GetRayHit(Pos);
               // PhaseChange(TouchPhases.None);
            }
                break;
           default:
               break;
        }
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
            }

            return hit;
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
