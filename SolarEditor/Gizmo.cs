using SolarSharp;
using SolarSharp.Core;
using SolarSharp.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarEditor
{
    internal class Gizmo
    {
        private bool isUsing = false;
        private Matrix4 oldM = Matrix4.Identity;

        public bool Operate(Camera camera, Entity entity)
        {
            ImGizmo.Enable(true);
            ImGizmo.SetRect(0, 0, Window.SurfaceWidth, Window.SurfaceHeight);

            Matrix4 modelMatrix = entity.ComputeModelMatrix();
            Matrix4 inputMatrix = modelMatrix.Transpose;
            if (ImGizmo.Manipulate(camera.GetProjectionMatrix().Transpose, camera.GetViewMatrix().Transpose, ref inputMatrix, ImGizmoOperation.TRANSLATE, ImGizmoMode.LOCAL))
            {
                Vector3 pos;
                Quaternion rot;
                Vector3 scl;
                Matrix4.Decompose(inputMatrix.Transpose, out pos, out rot, out scl);

                entity.Position = pos;
                entity.Orientation = rot;
                entity.Scale = scl;
            }

            if (Input.IsMouseButtonJustDown(MouseButton.MOUSE1) && ImGizmo.GizmoIsUsing())
            {
                isUsing = true;
                oldM = modelMatrix;
            }

            if (Input.IsMouseButtonJustUp(MouseButton.MOUSE1) && isUsing)
            {
                isUsing = false;

                UndoSystem.Add(new TransformAction(entity.Reference, oldM, modelMatrix));
            }

            return isUsing;
        }
    }
}
