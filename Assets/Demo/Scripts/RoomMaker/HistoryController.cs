using System.Collections.Generic;


public class HistoryController
{

    private Stack<IHistory> _undoStack = new Stack<IHistory>();
    private Stack<IHistory> _redoStack = new Stack<IHistory>();

    public void ExecuteHistory(IHistory history)
    {
        history.Execute();
        _undoStack.Push(history);
        _redoStack.Clear();
    }

    public void Undo()
    {
        if (_undoStack.Count > 0)
        {
            IHistory history = _undoStack.Pop();
            history.Undo();
            _redoStack.Push(history);
        }
    }

    public void Redo()
    {
        if (_redoStack.Count > 0)
        {
            IHistory history = _redoStack.Pop();
            history.Execute();
            _undoStack.Push(history);
        }
    }
}
