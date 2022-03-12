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
        public Gizmo gizmo;

        internal EditorState(EditorConfig config)
        {
            AssetPath = config.AssetPath;
            camera = new FlyCamera();
            currentRoom = new Room();
            selection = new Selection();
            gizmo = new Gizmo();

            currentRoom.entities.Add(new Entity());
            selection.entities.Add( currentRoom.entities[0] );
        }
      
        internal bool Update()
        {
            if (Application.IsKeyJustDown(0x1B))
            {
                Application.Quit();
            }

            bool handled = false;

            if (!handled)
            {
                handled = camera.Operate();
            }

            if (!handled)
            {
                handled = gizmo.Operate(camera, selection.entities);
            }

            if (!handled)
            {
                if (Application.IsKeyDown(KeyCode.LEFT_CONTROL) && Application.IsKeyJustDown(KeyCode.D))
                {
                    Entity entity = new Entity();
                    entity.Position = selection.entities[0].Position + Vector3.UnitX;
                    entity.Orientation = selection.entities[0].Orientation;
                    entity.Scale = selection.entities[0].Scale;

                    currentRoom.entities.Add(entity);
                    selection.entities[0] = entity;

                    handled = true;
                }
            }

            if (!handled && Application.IsMouseJustDown(1))
            {
                Ray ray = camera.ShootRayFromMousePos();
 


                //for (int i = 0; i < currentRoom.entities.Count; i++)
                //{
                //    Entity entity = currentRoom.entities[i];
                //    BoundingBox boundingBox = entity.GetBoundingBox();
                //
                //    RaycastInfo info;
                //    if (Raycast.BoundingBox(ray, boundingBox, out info))
                //    {
                //
                //    }
                //
                //    Debug.DrawBoundingBox(boundingBox);
                //    
                //}

                handled = true;
            }





            return false;
        }

        internal void Render(RenderPacket renderPacket)
        {
            if (currentRoom != null)
            {
                foreach (Entity entity in currentRoom.entities)
                {
                    renderPacket.renderEntries.Add(new RenderEntry(entity.Position, entity.Orientation, entity.Scale));
                }                
            }

            renderPacket.viewMatrix = camera.GetViewMatrix();
            renderPacket.projectionMatrix = camera.GetProjectionMatrix();
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
