using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEditor
{
    internal static class UndoSystem
    {
        private static Stack<UndoAction> undoActions = new Stack<UndoAction>();
        private static Stack<UndoAction> redoActions = new Stack<UndoAction>();

        public static void Add(UndoAction undoAction)
        {
            undoActions.Push(undoAction);
            redoActions.Clear();
        }

        public static void Undo()
        {
            if (undoActions.Count > 0)
            {
                UndoAction action = undoActions.Pop();
                redoActions.Push(action);
                action.Undo();
            }
        }

        public static void Redo()
        {
            if (redoActions.Count > 0)
            {
                UndoAction action = redoActions.Pop();
                undoActions.Push(action);
                action.Redo();
            }
        }
    }
}
