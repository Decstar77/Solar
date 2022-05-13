using SolarSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEditor
{
    internal static class UndoSystem
    {
        public static Stack<UndoAction> undoActions = new Stack<UndoAction>();
        private static Stack<UndoAction> redoActions = new Stack<UndoAction>();

        public static void Add(UndoAction undoAction)
        {
            Logger.Trace("Adding " + undoAction.GetType().ToString());
            undoActions.Push(undoAction);
            redoActions.Clear();
        }

        public static void Undo()
        {
            if (undoActions.Count > 0)
            {
                UndoAction action = undoActions.Pop();
                redoActions.Push(action);
                Logger.Trace("Undo: " + action.GetType().ToString());
                action.Undo();
            }
        }

        public static void Redo()
        {
            if (redoActions.Count > 0)
            {
                UndoAction action = redoActions.Pop();
                undoActions.Push(action);
                Logger.Trace("Redo: " + action.GetType().ToString());
                action.Redo();
            }
        }
    }
}
