using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
   [SerializeField]  private InputManager _inputManager;

    private TouchPhases phase;
    // Start is called before the first frame update
   
    private bool begin = false;
    // Update is called once per frame
    void Update()
    {
       // phase = _inputManager.CurrentPhase;
       
        switch (_inputManager.CurrentPhase)
        {
            case TouchPhases.None:
            {
                
            }
                break;
            case TouchPhases.Began:
            {
                if (begin)
                    return;
                begin = true;
                Debug.Log("Yes Began");
            }
                break;
            case TouchPhases.Hold:
            {
                Debug.Log("Yes HOLD");
            }
                break;
            case TouchPhases.Moved:
            {
                Debug.Log("Yes MOVED");
            }
                break;
            case TouchPhases.Ended:
            {
                Debug.Log("Yes ENDED");
                begin = false;
            }
                break;
        }
    }
}
