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

            if (!camera.Operate())
            {
                gizmo.Operate(camera, selection.entities);
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

        internal void Gizmo()
        {
            gizmo.Operate(camera, selection.entities);
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
