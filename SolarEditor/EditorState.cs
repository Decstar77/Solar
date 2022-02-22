using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarSharp;
using SolarSharp.Rendering;
using SolarSharp.GameLogic;

namespace SolarEditor
{
    internal class Selection
    {
        public List<Entity> entities = new List<Entity>();        
    }

    internal class EditorState
    {
        public string AssetPath;
        public FlyCamera camera;
        public Room currentRoom;
        public Selection selection;   

        internal EditorState(EditorConfig config)
        {
            AssetPath = config.AssetPath;
            camera = new FlyCamera();
            currentRoom = new Room();
            selection = new Selection();

            currentRoom.entities.Add(new Entity());
            selection.entities.Add( currentRoom.entities[0] );
        }


        public Ray ray= new Ray(); 
        internal bool Update()
        {
            if (Application.IsKeyJustDown(0x1B))
            {
                Application.Quit();
            }

            if (Application.GetMouseJustDown(1))
            {
                ray = camera.ShootRayFromMousePos();
            }
            Plane plane = new Plane(Vector3.Zero, Vector3.UnitY);

            RaycastInfo info;
            if (Raycast.Plane(ray, plane, out info))
            {
                Debug.DrawPoint(info.point);
            }

            Debug.DrawRay(ray);

            camera.Operate();
            return false;
        }

        internal void Render(RenderPacket renderPacket)
        {
            if (currentRoom != null)
            {
                foreach (Entity entity in currentRoom.entities)
                {
                    renderPacket.renderEntries.Add(new RenderEntry(entity.Position, entity.Orientation, new Vector3(1, 1, 1)));
                }                
            }

            renderPacket.viewMatrix = camera.GetViewMatrix();
            renderPacket.projectionMatrix = camera.GetProjectionMatrix();
        }

        internal void Gizmo()
        {
            if (selection.entities.Count > 0)
            {
                Entity entity = selection.entities[0];
                Matrix4 model = entity.ComputeModelMatrix().Transpose;
                               
                ImGui.GizmoSetRect(0, 0, Application.SurfaceWidth, Application.SurfaceHeight);
                ImGui.GizmoManipulate(camera, ref model, 0, 1);
                
                Vector3 position;
                Quaternion orientation;
                Vector3 scale;
                Matrix4.Decompose(model.Transpose, out position, out orientation, out scale);
                
                entity.Position = position;
            }
        }

        internal bool AddWindow(Window window)
        {
            foreach(Window w in windows)
            {
                if (w.GetType() == window.GetType())
                    return false;
            }

            windows.Add(window);
            return true;
        }

        internal void ShowWindows()
        {
            List<Window> removes = new List<Window>();  
            foreach (Window w in windows)
            {
                w.Show(this);
                if (w.ShouldRemove()) { removes.Add(w);  }
            }
            foreach(Window w in removes) { windows.Remove(w);  Logger.Info("Window removed " + w.GetType().Name); }
        }

        private List<Window> windows = new List<Window>();
    }
}
