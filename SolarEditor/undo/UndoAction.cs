using SolarSharp;
using SolarSharp.Assets;
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

    internal class CreateEntityUndoAction : UndoAction
    {
        private List<EntityAsset> entityAssets;
        private GameScene scene;
        private Selection selection;

        public CreateEntityUndoAction(GameScene scene, Selection selection, List<Entity> entities)
        {
            entityAssets = new List<EntityAsset>(entities.Select(x => x.CreateEntityAsset()));
            this.scene = scene;
            this.selection = selection;
        }

        internal override bool Redo()
        {
            entityAssets.ForEach(x => scene.CreateEntity(x));
            selection.Set(entityAssets.Select(x => x.reference).ToList(), false);
            return true;
        }

        internal override bool Undo()
        {
            selection.Clear(false);
            entityAssets.ForEach(x => scene.DestroyEntity(x.reference));
            return true;
        }
    }

    internal class DeleteEntityUndoAction : UndoAction
    {
        private List<EntityAsset> entityAssets;
        private GameScene scene;
        private Selection selection;

        public DeleteEntityUndoAction(GameScene scene, Selection selection, List<Entity> entities)
        {
            entityAssets = new List<EntityAsset>(entities.Select(x => x.CreateEntityAsset()));
            this.scene = scene;
            this.selection = selection;
        }

        internal override bool Redo()
        {
            selection.Clear(false);
            entityAssets.ForEach(x => scene.DestroyEntity(x.reference));
            return true;
        }

        internal override bool Undo()
        {
            entityAssets.ForEach(x => scene.CreateEntity(x));
            selection.Set(entityAssets.Select(x => x.reference).ToList(), false);
            return true;
        }
    }

    internal class SelectionUndoAction : UndoAction
    {
        private List<EntityReference> lastSelection;
        private List<EntityReference> newSelection;
        private Selection selection;

        public SelectionUndoAction(Selection selection, List<EntityReference> lastSelection, List<EntityReference> newSelection) {
            this.lastSelection = lastSelection;
            this.newSelection = newSelection;
            this.selection = selection;
        }

        internal override bool Redo()
        {
            selection.Set(newSelection, false);
            return true;
        }

        internal override bool Undo()
        {
            selection.Set(lastSelection, false);
            return true;
        }
    }

}
