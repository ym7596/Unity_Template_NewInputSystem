using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHistory
{
    void Undo();
    void Execute();
}
