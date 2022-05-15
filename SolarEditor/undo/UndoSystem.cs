using SolarSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEditor
{
    internal class UndoSystem
    {
        public Stack<UndoAction> undoActions = new Stack<UndoAction>();
        private Stack<UndoAction> redoActions = new Stack<UndoAction>();

        public bool Enabled { get; set; } = true;

        public void Add(UndoAction undoAction)
        {
            if (Enabled)
            {
                Logger.Trace("Adding " + undoAction.GetType().ToString());
                undoActions.Push(undoAction);
                redoActions.Clear();
            }
        }

        public void Undo()
        {
            if (Enabled && undoActions.Count > 0)
            {
                UndoAction action = undoActions.Pop();
                redoActions.Push(action);
                Logger.Trace("Undo: " + action.GetType().ToString());
                action.Undo();
            }
        }

        public void Redo()
        {
            if (Enabled && redoActions.Count > 0)
            {
                UndoAction action = redoActions.Pop();
                undoActions.Push(action);
                Logger.Trace("Redo: " + action.GetType().ToString());
                action.Redo();
            }
        }
    }
}
