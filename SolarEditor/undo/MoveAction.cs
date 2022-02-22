using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SolarSharp;
using SolarSharp.GameLogic;
namespace SolarEditor
{
    internal class MoveAction : Action
    {
        public MoveAction(Entity entity, Vector3 lastPos, Vector3 currentPos)
        {

        }

        bool Action.Redo()
        {
            throw new NotImplementedException();
        }

        bool Action.Undo()
        {
            throw new NotImplementedException();
        }
    }
}
