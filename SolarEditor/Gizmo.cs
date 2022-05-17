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

        private ImGizmoOperation operation = ImGizmoOperation.TRANSLATE;
        private ImGizmoMode mode = ImGizmoMode.LOCAL;

        public bool Operate(Camera camera, List<Entity> entities, UndoSystem undoSystem)
        {
            ImGizmo.Enable(true);
            ImGizmo.SetRect(0, 0, Window.SurfaceWidth, Window.SurfaceHeight);

            if (Input.IskeyJustDown(KeyCode.W))
            {
                operation = ImGizmoOperation.TRANSLATE;
            }
            else if (Input.IskeyJustDown(KeyCode.R))
            {
                operation = ImGizmoOperation.ROTATE;
            }
            else if (Input.IskeyJustDown(KeyCode.E))
            {
                operation = ImGizmoOperation.SCALE;
                mode = ImGizmoMode.LOCAL;
            }

            if (Input.IskeyJustDown(KeyCode.TLDA))
            {
                if (operation != ImGizmoOperation.SCALE)
                {
                    mode = mode == ImGizmoMode.LOCAL ? ImGizmoMode.WORLD : ImGizmoMode.LOCAL;
                }
            }

            if (entities.Count > 0)
            {
                Matrix4 modelMatrix = entities[0].ComputeTransformMatrix();
                Matrix4 inputMatrix = modelMatrix.Transpose;
                Matrix4 deltaMatrix = Matrix4.Identity;

                float[] snap = new float[] { 0.0f, 0.0f, 0.0f };
                if (Input.IsKeyDown(KeyCode.CTRL_L)) {
                    if (operation == ImGizmoOperation.TRANSLATE || operation == ImGizmoOperation.SCALE)
                    {
                        snap = new float[] { 1.0f, 1.0f, 1.0f };
                    }
                    else
                    {
                        snap = new float[] { 15.0f, 15.0f, 15.0f };
                    }
                }

                bool action = !ImGui.IsWindowHovered(ImGuiHoveredFlags.AnyWindow) && !ImGui.IsAnyItemHovered();
                if (ImGizmo.Manipulate(
                    camera.GetProjectionMatrix().Transpose, camera.GetViewMatrix().Transpose, 
                    ref inputMatrix, ref deltaMatrix, operation, mode, snap))                    
                {
                    if (action)
                    {
                        Vector3 pos;
                        Quaternion rot;
                        Vector3 scl;
                        Matrix4.Decompose(inputMatrix.Transpose, out pos, out rot, out scl);

                        Vector3 delta = pos - entities[0].Position;

                        entities[0].Orientation = rot;
                        entities[0].Scale = scl;

                        foreach (var entity in entities)
                        {
                            entity.Position += delta;
                        }
                    }
                }

                if (action)
                {
                    if (Input.IsMouseButtonJustDown(MouseButton.MOUSE1) && ImGizmo.GizmoIsUsing())
                    {
                        isUsing = true;
                        oldM = modelMatrix;
                    }

                    if (Input.IsMouseButtonJustUp(MouseButton.MOUSE1) && isUsing)
                    {
                        isUsing = false;
                        undoSystem.Add(new TransformAction(entities[0].Reference, oldM, modelMatrix));
                    }
                }
            }

            return isUsing;
        }
    }
}
