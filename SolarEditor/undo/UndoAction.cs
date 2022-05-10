using SolarSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEditor
{
    internal abstract class UndoAction
    {
        internal abstract bool Redo();
        internal abstract bool Undo();
    }

    internal class TransformAction : UndoAction
    {
        private EntityReference entity = EntityReference.Invalid;
        private Matrix4 lastM = Matrix4.Identity;
        private Matrix4 newM = Matrix4.Identity;

        internal TransformAction(EntityReference entity, Matrix4 lastM, Matrix4 newM)
        {
            this.entity = entity;
            this.lastM = lastM;
            this.newM = newM;
        }

        internal override bool Redo()
        {
            entity.GetEntity()?.SetTransform(newM);
            return true;
        }

        internal override bool Undo()
        {
            entity.GetEntity()?.SetTransform(lastM);
            return true;
        }
    }
}
