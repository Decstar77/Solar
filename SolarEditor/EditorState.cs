using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarSharp;

namespace SolarEditor
{
    internal class EditorState
    {
        public string AssetPath;

        internal EditorState(EditorConfig config)
        {
            AssetPath = config.AssetPath; 
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
