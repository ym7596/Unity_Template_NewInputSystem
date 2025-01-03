using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIcanvas : SingletonBehave<UIcanvas>
{
    public event Action onAction_Undo;
    public event Action onAction_Redo;

    public void OnButton_Undo()
    {
        onAction_Undo?.Invoke();
    }

    public void OnButton_Redo()
    {
        onAction_Redo?.Invoke();
    }
}
