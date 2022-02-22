using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SolarSharp;
using SolarSharp.GameLogic;

namespace SolarEditor
{
    internal enum GizmoOperation
    { 
        TRANSLATION = 0,
        ROTATION = 1,
        SCALING = 2
    }

    internal enum GizmoMode
    {
        LOCAL = 0,
        WORLD = 1,
    }

    internal class Gizmo
    {
        private GizmoOperation operation;
        private GizmoMode mode;

        public Gizmo()
        {
            operation = GizmoOperation.TRANSLATION;
            mode = GizmoMode.LOCAL;
        }

        public void Operate(Camera camera, List<Entity> entities)
        {
            if (Application.IsKeyJustDown('T')) { operation = GizmoOperation.TRANSLATION; }
            if (Application.IsKeyJustDown('R')) { operation = GizmoOperation.ROTATION; }
            if (Application.IsKeyJustDown('E')) { operation = GizmoOperation.SCALING; mode = GizmoMode.LOCAL; }

            if (Application.IsKeyJustDown(0xC0) && operation != GizmoOperation.SCALING) 
            {
                mode = mode == GizmoMode.LOCAL ? GizmoMode.WORLD : GizmoMode.LOCAL;  
            }

            if (entities.Count > 0)
            {
                Entity entity = entities[0];
                Matrix4 model = entity.ComputeModelMatrix().Transpose;

                ImGui.GizmoSetRect(0, 0, Application.SurfaceWidth, Application.SurfaceHeight);
                ImGui.GizmoManipulate(camera, ref model, (int)operation, (int)mode);

                Vector3 position;
                Quaternion orientation;
                Vector3 scale;
                Matrix4.Decompose(model.Transpose, out position, out orientation, out scale);

                entity.Position = position;
                entity.Orientation = orientation;
                entity.Scale = scale;
            }
        }
    }



}
