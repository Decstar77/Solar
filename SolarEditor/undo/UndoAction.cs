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

    internal class CreateEntityAction : UndoAction
    {
        private Entity entity;

        public CreateEntityAction(Entity entity)
        {
            this.entity = entity;
        }

        internal override bool Redo()
        {
            GameSystem.CurrentScene.PlaceEntity(entity);
            return true;
        }

        internal override bool Undo()
        {
            GameSystem.CurrentScene.DeleteEntity(entity.Id);
            return true;
        }
    }

    internal class DeleteEntityAction : UndoAction
    {
        private Entity entity;

        public DeleteEntityAction(Entity entity)
        {
            this.entity = entity;
        }

        internal override bool Redo()
        {
            GameSystem.CurrentScene.DeleteEntity(entity.Id);
            return true;
        }

        internal override bool Undo()
        {
            GameSystem.CurrentScene.PlaceEntity(entity);
            return true;
        }
    }

    internal class SelectionEntityAction : UndoAction
    {
        private List<EntityReference> selection;

        public SelectionEntityAction(List<EntityReference> selection) {
            selection = new List<EntityReference>(selection);
        }

        internal override bool Redo()
        {
            return true;
        }

        internal override bool Undo()
        {
            return true;
        }
    }

}
